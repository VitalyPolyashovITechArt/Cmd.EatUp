using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Cmd.EatUp.Helpers;

namespace Cmd.EatUp.Data
{
    public class DatabaseRepository
    {
        private eatupEntities1 context = new eatupEntities1();

        public void SaveChanges()
        {
            context.SaveChanges();
        }

        public void SaveEmployees(IEnumerable<Employee> employees)
        {
            context.Employees.AddRange(employees);
            context.SaveChanges();
        }

        public int GetIdByName(string userName)
        {
           return context.Employees.Single(x => x.StringId == userName.ToLower()).ProfileId.Value;
        }

        public Employee GetProfile(int id)
        {
            return context.Employees.Single(x => x.ProfileId == id);
        }

        public Meeting GetAcceptedMeeting(int id)
        {
            return GetProfile(id).Meetings.SingleOrDefault(x => x.Time.Date == DateTime.Now.Date);
        }
        public List<Meeting> GetInvitations(int id)
        {
            return GetProfile(id).Invites.Where(x => x.Time.Date == DateTime.Now.Date).ToList();
        }

        public List<Meeting> GetMeetings(int id)
        {
            Employee currentEmployee = GetProfile(id);

            var knownPeople = GetPeopleYouEverMet(id).ToList();


            TimeSpan startTime = currentEmployee.Time.Value.TimeOfDay.Add(TimeSpan.FromMinutes(-30));
            TimeSpan finishTime = currentEmployee.Time.Value.TimeOfDay.Add(TimeSpan.FromMinutes(30));
            var result = context.Meetings.Include("Employees").ToList()
                .Where(x => x.Time.TimeOfDay >= startTime && x.Time.TimeOfDay <= finishTime)
                .OrderByDescending(y =>
                    GetEmployeeWeights(currentEmployee, y.Employees, knownPeople)
                        .Sum(z => z.Value)
                ).Take(10)
                .ToList();

            return result;
        }

        private IEnumerable<Meeting> GetNearestMeetings(DateTime? time)
        {
            
            var allmeetings = context.Meetings.ToList();
            //!!!!!!!!!!!
            return allmeetings.Where(x => x.Time.Date == DateTime.Now.AddDays(1).Date).Where(x => x.Time > DateTime.Now);
        }

        private IEnumerable<Employee> GetAlonePeople(int id)
        {
            Employee currentEmployee = GetProfile(id);
            TimeSpan startTime = currentEmployee.Time.Value.TimeOfDay.Add(TimeSpan.FromMinutes(-30));
            TimeSpan finishTime = currentEmployee.Time.Value.TimeOfDay.Add(TimeSpan.FromMinutes(30));
           var todaysMeetings= GetNearestMeetings(currentEmployee.Time).Select(y => y.Id);
            var allemployees = context.Employees.ToList();
            var result = allemployees.Where(x => x.Time.Value.TimeOfDay >= startTime && x.Time.Value.TimeOfDay <= finishTime);
            result = result.Where(y => !y.Meetings.Any(f => todaysMeetings.Contains(f.Id)));
            return result;
        }

        public IEnumerable<Employee> GetPreferrablePeople(int id)
        {
            var currentEmployee = GetProfile(id);
            var result = GetAlonePeople(id);
            var knownPeople = GetPeopleYouEverMet(id).ToList();
            result = result.OrderByDescending(x => GetEmployeeWeight(currentEmployee, x, knownPeople));
            return result.Take(10).ToList();
        }

        private Dictionary<int, int> GetEmployeeWeights(Employee employee, IEnumerable<Employee> employees, IEnumerable<int> knownPeople )
        {
            Dictionary<int, int> weightDic = employees.ToDictionary(x => x.ProfileId.Value, y => GetEmployeeWeight(employee, y, knownPeople));
           
            return weightDic;
        }

        private int GetEmployeeWeight(Employee employee, Employee targetEmployee, IEnumerable<int> knownPeople )
        {
            int sum = 0;

            if (targetEmployee.Room == employee.Room)
            {
                sum += 5;
            }
            if (knownPeople.Contains(targetEmployee.ProfileId.Value))
            {
                sum += 4;
            }
            if (targetEmployee.ProjectId == employee.ProjectId)
            {
                sum += 4;
            }
            if ( Math.Abs(targetEmployee.Birthday.Value - employee.Birthday.Value) < 2)
            {
                sum += 3;
            }
            if (targetEmployee.DepartmentId == employee.DepartmentId)
            {
                sum += 2;
            }
            if (targetEmployee.Position == employee.Position)
            {
                sum += 1;
            }

            return sum;
        }

        public void JoinMeeting(int id, int meetingId)
        {
            var employee = GetProfile(id);
            var meeting = context.Meetings.Single(x => x.Id == meetingId);
            meeting.Employees.Add(employee);
            if (employee.Invites.Contains(meeting))
            {
                employee.Invites.Remove(meeting);
            }
            context.SaveChanges();
        }

        public void InviteToMeeting(int id, int targetEmployeeId)
        {
            var employee = GetProfile(id);
            var meeting = GetAcceptedMeeting(id);
            var targetEmployee = GetProfile(targetEmployeeId);
            if (meeting == null)
            {
                meeting= new Meeting();
                {
                    meeting.Employees = new Collection<Employee>();
                    meeting.Employees.Add(employee);
                    meeting.PlaceId = employee.PlaceId.Value;
                    meeting.Time = employee.Time.Value;
                    meeting.InvitedEmployees = new List<Employee>();              
                    context.Meetings.Add(meeting);
                }
            }
            meeting.InvitedEmployees.Add(targetEmployee);
            
            
            context.SaveChanges();
        }

        public IEnumerable<Employee> GetAllEmployees()
        {
            return context.Employees;
        }

        public void InviteRandomEmployees(int id)
        {
            var result = GetAlonePeople(id).ToList().Shuffle(3);
            result.ForEach(x => InviteToMeeting(id, x.ProfileId.Value));
        }

        private IEnumerable<int> GetPeopleYouEverMet(int id)
        {
            return GetProfile(id).Meetings.SelectMany(x => x.Employees).Select(y => y.ProfileId.Value);
        }

        public IEnumerable<Place> GetAllPlaces()
        {
            return context.Places.ToList();
        }
    }
}

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

            DateTime startTime = currentEmployee.Time.Value.AddMinutes(-30);
            DateTime finishTime = currentEmployee.Time.Value.AddMinutes(30);
            var result = context.Meetings
                .Where(x => x.Time >= startTime && x.Time <= finishTime)
                .OrderByDescending(y =>
                    GetEmployeeWeights(currentEmployee, y.Employees)
                        .Sum(z => z.Value)
                ).Take(10)
                .ToList();

            return result;
        }

        private IEnumerable<Employee> GetAlonePeople(int id)
        {
            Employee currentEmployee = GetProfile(id);

            TimeSpan startTime = currentEmployee.Time.Value.TimeOfDay.Add(TimeSpan.FromMinutes(-30));
            TimeSpan finishTime = currentEmployee.Time.Value.TimeOfDay.Add(TimeSpan.FromMinutes(30));
            var allmeetings = context.Meetings.ToList();
            //!!!!!!!!!!!
            var todaysMeetings = allmeetings.Where(x => x.Time.Date == DateTime.Now.AddDays(1).Date).Select(y => y.Id);
            var allemployees = context.Employees.ToList();
            var result = allemployees.Where(x => x.Time.Value.TimeOfDay >= startTime && x.Time.Value.TimeOfDay <= finishTime);
            result = result.Where(y => !y.Meetings.Any(f => todaysMeetings.Contains(f.Id)));
            return result;
        }

        public IEnumerable<Employee> GetPreferrablePeople(int id)
        {
            var currentEmployee = GetProfile(id);
            var result = GetAlonePeople(id);
            result = result.OrderByDescending(x => GetEmployeeWeight(currentEmployee, x));
            return result.Take(10).ToList();
        }

        private Dictionary<int, int> GetEmployeeWeights(Employee employee, IEnumerable<Employee> employees)
        {
            Dictionary<int, int> weightDic = employees.ToDictionary(x => x.ProfileId.Value, y => 0);
            employees.Where(x => x.Room == employee.Room).ToList().ForEach(y => weightDic[y.ProfileId.Value]+=5);
            employees.Where(x => x.ProjectId == employee.ProjectId).ToList().ForEach(y => weightDic[y.ProfileId.Value] += 4);
            employees.Where(x => x.DepartmentId == employee.DepartmentId).ToList().ForEach(y => weightDic[y.ProfileId.Value] += 2);
            employees.Where(x => x.Position == employee.Position).ToList().ForEach(y => weightDic[y.ProfileId.Value] += 1);

            return weightDic;
        }

        private int GetEmployeeWeight(Employee employee, Employee targetEmployee)
        {
            int sum = 0;

            if (targetEmployee.Room == employee.Room)
            {
                sum += 5;
            }

            if (targetEmployee.ProjectId == employee.ProjectId)
            {
                sum += 4;
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
    }
}

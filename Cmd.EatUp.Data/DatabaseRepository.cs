using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Core.Objects;
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
            
            return GetProfile(id).Meetings.FirstOrDefault(x => x.Time.Date == DateTime.Now.Date);
        }

		public Meeting GetMeeting(int id)
		{

			return context.Meetings.SingleOrDefault(x => x.Id == id);
		}
        public List<Meeting> GetInvitations(int id)
        {
            return GetProfile(id).Invites.Where(x => x.Time.Date == DateTime.Now.Date).ToList();
        }

        public List<Meeting> GetMeetings(int id)
        {
            Employee currentEmployee = GetProfile(id);

            var knownPeople = GetPeopleYouEverMet(id).ToList();

            TimeSpan time = currentEmployee.Time.Value.Add(TimeSpan.FromMinutes(30));
            TimeSpan? startTime = time.Add(TimeSpan.FromMinutes(-30));
            TimeSpan? finishTime = time.Add(TimeSpan.FromMinutes(30));
            var result = context.Meetings.Where(z => z.PlaceId == currentEmployee.PlaceId)
				.Where(z => z.Time >= DateTime.Now)
				.Where(c => c.Employees.Any())
                .Where(x => EntityFunctions.CreateTime(x.Time.Hour, x.Time.Minute, 0) >= startTime && EntityFunctions.CreateTime(x.Time.Hour, x.Time.Minute, 0) <= finishTime)
                .Where(f => f.Employees.All(m => m.ProfileId != currentEmployee.ProfileId)).ToList()
                .OrderByDescending(y =>
                    GetEmployeeWeights(currentEmployee, y.Employees, knownPeople)
                ).Take(30)
                .ToList();

            return result;
        }

        private IEnumerable<Meeting> GetNearestMeetings()
        {          
            var allmeetings = context.Meetings.ToList();
            return allmeetings.Where(x => x.Time.Date == DateTime.Now.Date).Where(x => x.Time > DateTime.Now);
        }

        private IEnumerable<Employee> GetAlonePeople(int id)
        {
            Employee currentEmployee = GetProfile(id);
            TimeSpan startTime = currentEmployee.Time.Value.Add(TimeSpan.FromMinutes(-30));
            TimeSpan finishTime = currentEmployee.Time.Value.Add(TimeSpan.FromMinutes(30));
            var todaysMeetings= GetNearestMeetings().Select(y => y.Id);
            var allemployees = context.Employees.Where(x => x.ProfileId != id);
            var result = allemployees.Where(y => !y.Meetings.Any(f => todaysMeetings.Contains(f.Id))).ToList();
            result = result.Where(x => x.Time.Value >= startTime && x.Time.Value <= finishTime).ToList();
            return result;
        }

        public IEnumerable<Employee> GetPreferrablePeople(int id)
        {
            var currentEmployee = GetProfile(id);
            var result = GetAlonePeople(id);
            var knownPeople = GetPeopleYouEverMet(id).ToList();
            result = result.OrderByDescending(x => GetEmployeeWeight(currentEmployee, x, knownPeople));
            return result.Take(50).ToList();
        }

        private int GetEmployeeWeights(Employee employee, IEnumerable<Employee> employees, IEnumerable<int> knownPeople )
        {
            Dictionary<int, int> weightDic = employees.ToDictionary(x => x.ProfileId.Value, y => GetEmployeeWeight(employee, y, knownPeople));
           
            return weightDic.Sum(x => x.Value);
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
            //if (targetEmployee.ProjectId == employee.ProjectId)
            //{
            //    sum += 4;
            //}
            if (targetEmployee.Birthday.HasValue && employee.Birthday.HasValue)
            {
                if (Math.Abs(targetEmployee.Birthday.Value - employee.Birthday.Value) <= 1)
                {
                    sum += 3;
                }
            }
            if (targetEmployee.DepartmentId == employee.DepartmentId)
            {
                sum += 3;
            }
            if (targetEmployee.Position == employee.Position)
            {
                sum += 2;
            }

            return sum;
        }

        public void JoinMeeting(int id, int meetingId)
        {
            var employee = GetProfile(id);
            var meeting = context.Meetings.Single(x => x.Id == meetingId);
            employee.Time = meeting.Time.TimeOfDay;
            meeting.Employees.Add(employee);
            if (employee.Invites.Contains(meeting))
            {
                employee.Invites.Remove(meeting);
            }
            context.SaveChanges();
        }

		public void LeaveMeeting(int id, int meetingId)
		{
			var employee = GetProfile(id);
			var meeting = context.Meetings.SingleOrDefault(x => x.Id == meetingId);
			if (meeting != null)
			{
				employee.Time = meeting.Time.TimeOfDay;
				meeting.Employees.Remove(employee);
				if (employee.Invites.Contains(meeting))
				{
					employee.Invites.Remove(meeting);
				}
				context.SaveChanges();
			}
		}

        public Meeting InviteToMeeting(int id, int targetEmployeeId)
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
                    meeting.Time = DateTime.Today.Add(employee.Time.Value);
                    meeting.InvitedEmployees = new List<Employee>();              
                    context.Meetings.Add(meeting);
                }
            }
            meeting.InvitedEmployees.Add(targetEmployee);
            
            
            context.SaveChanges();

	        return meeting;
        }

        public void InviteRandomEmployees(int id)
        {
            var result = GetAlonePeople(id).ToList().Shuffle(5);
            result.ForEach(x => InviteToMeeting(id, x.ProfileId.Value));
        }

        private IEnumerable<int> GetPeopleYouEverMet(int id)
        {
            return GetProfile(id).Meetings.SelectMany(x => x.Employees).Select(y => y.ProfileId.Value);
        }


        public IEnumerable<string> GetAchievements(int id)
        {
            var list = new List<string>();
            var meetings = context.Meetings.ToList();
            if (meetings.OrderByDescending(x => x.Employees.Count)
                .First()
                .Employees.Select(y => y.ProfileId)
                .Contains(id))
            {
                list.Add("BiggestTeam");
            }

            if (meetings.OrderByDescending(x => x.Time.TimeOfDay)
                .First()
                .Employees.Select(y => y.ProfileId)
                .Contains(id))
            {
                list.Add("LatestTeam");
            }

            if (meetings.OrderBy(x => x.Time.TimeOfDay)
               .First()
               .Employees.Select(y => y.ProfileId)
               .Contains(id))
            {
                list.Add("EarliestTeam");
            }
            return list;
        }

        public IEnumerable<Place> GetAllPlaces()
        {
            return context.Places.ToList();
        }

        public void ChangePlaceAndTime(int id, TimeSpan? time, string placeName)
        {
            var profile = GetProfile(id);

            if (!string.IsNullOrEmpty(placeName))
            {
                profile.PlaceId = context.Places.First(place => place.Name == placeName).Id;
            }
            if (time.HasValue)
            {
                profile.Time = time;
            }

            context.SaveChanges();
        }

        public void SetDeviceId(int id, string deviceId)
        {
            GetProfile(id).DeviceId = deviceId;
            context.SaveChanges();
        }
    }
}

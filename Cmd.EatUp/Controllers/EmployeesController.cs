
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Serialization;
using System.Web.Http;
using Cmd.EatUp.Data;
using Cmd.EatUp.Models;
using Cmd.EatUpTests;
using System;
using System.Collections.Generic;


namespace Cmd.EatUp.Controllers
{
    [RoutePrefix("Employees")]
    public class GeneralApiController : ApiController
    {
    public class ResponseObject
    {
        public bool Success { get; set; }

        public string FaultMessage { get; set; }
    }

    public class GetEmployeesResult : ResponseObject
    {
        public IEnumerable<Employee> Employees { get; set; }
    }

    public class AuthenticateResult : ResponseObject
    {
        public string SessionId { get; set; }
    }


        [Route("Authenticate")]
        [HttpGet]
        public int Authenticate(string userName, string password)
        {
            string sessionId = String.Empty;
            var smgAdapter = new SmgAdapter();

                sessionId = smgAdapter.Authenticate(userName, password);

                DatabaseRepository repository = new DatabaseRepository();
            return repository.GetIdByName(userName);

        }

		[Route("PostDeviceId")]
		[HttpGet]
		public object PostDevice(int id, string deviceId)
		{
			ProfileInfoViewModel viewModel = new ProfileInfoViewModel();
			DatabaseRepository repository = new DatabaseRepository();
			var result = repository.GetProfile(id);
			viewModel.FullName = String.Format("{0} {1}", result.FirstName, result.LastName);
            viewModel.StartPreferredTime = result.Time.Value.Add(TimeSpan.FromMinutes(-30));
            viewModel.FinishPreferredTime = result.Time.Value.Add(TimeSpan.FromMinutes(30));
			viewModel.ExactTime = result.Time.Value;
			viewModel.PlaceName = result.Place.Name;
			viewModel.ImagePath = "http://192.168.13.49/content/" + result.ProfileId.Value + ".jpg"; ;
			viewModel.Achievements = repository.GetAchievements(id).ToList();
			viewModel.CurrentMeeting = ConvertToMeetingViewMOdel(repository.GetAcceptedMeeting(id));

			return viewModel;
		}

        [Route("GetProfileInfo")]
        [HttpGet]
        public ProfileInfoViewModel GetProfileInfo(int id)
        {
            ProfileInfoViewModel viewModel = new ProfileInfoViewModel();
           DatabaseRepository repository = new DatabaseRepository();
            var result = repository.GetProfile(id);
            viewModel.FullName = String.Format("{0} {1}", result.FirstName, result.LastName);
            viewModel.StartPreferredTime = result.Time.Value.Add(TimeSpan.FromMinutes(-30));
            viewModel.FinishPreferredTime = result.Time.Value.Add(TimeSpan.FromMinutes(30));
            viewModel.ExactTime = result.Time.Value;
            viewModel.PlaceName = result.Place.Name;
            viewModel.ImagePath = "http://192.168.13.49/content/" + result.ProfileId.Value + ".jpg"; ;
            viewModel.Achievements = repository.GetAchievements(id).ToList();
            viewModel.CurrentMeeting = ConvertToMeetingViewMOdel(repository.GetAcceptedMeeting(id));

            return viewModel;
        }


        private MeetingViewModel ConvertToMeetingViewMOdel(Meeting model)
        {
            if (model == null)
            {
                return null;
            }
            var viewModel = new MeetingViewModel();
            viewModel.Id = model.Id;
            viewModel.PlaceName = model.Place.Name;
            viewModel.Date = model.Time;
            viewModel.Employees = new List<EmployeeViewModel>();
            foreach (var employee in model.Employees)
            {
                viewModel.Employees.Add(ConvertToEmployeeViewMOdel(employee));
            }

            return viewModel;
        }

        private PlacesViewModel ConvertToPlacesViewModel(Place model)
        {
            return new PlacesViewModel()
            {
                PlaceName = model.Name
            };
        }


        private EmployeeViewModel ConvertToEmployeeViewMOdel(Employee model)
        {
            var employeeViewMOdel = new EmployeeViewModel();
            employeeViewMOdel.FullName = String.Format("{0} {1}", model.FirstName, model.LastName);
			employeeViewMOdel.ImageUrl = "http://192.168.13.49/content/" + model.ProfileId.Value + ".jpg";
            employeeViewMOdel.Id = model.ProfileId.Value;
            return employeeViewMOdel;
        }

        private SortableEmployeeViewModel ConvertToSortableEmployeeViewMOdel(Employee model, int index)
        {
            var employeeViewMOdel = new SortableEmployeeViewModel();
            employeeViewMOdel.FullName = String.Format("{0} {1}", model.FirstName, model.LastName);
			employeeViewMOdel.ImageUrl = "http://192.168.13.49/content/" + model.ProfileId.Value + ".jpg";
            employeeViewMOdel.Id = model.ProfileId.Value;
            employeeViewMOdel.Index = index;
            return employeeViewMOdel;
        }

        [Route("GetInvites")]
        [HttpGet]
        public List<MeetingViewModel> GetInvites(int id)
        {
            var repository = new DatabaseRepository();
            return repository.GetInvitations(id).Select(ConvertToMeetingViewMOdel).ToList();
        }

        [Route("GetPreferablePeople")]
        [HttpGet]
        public List<SortableEmployeeViewModel> GetPreferablePeople(int id)
        {
            var repository = new DatabaseRepository();
            var result=  repository.GetPreferrablePeople(id).Select((x, i) => ConvertToSortableEmployeeViewMOdel(x, i)).ToList();
            return result;
        }


        [Route("GetMeetings")]
        [HttpGet]
        public List<MeetingViewModel> GetMeetings(int id)
        {
            var repository = new DatabaseRepository();
            return repository.GetMeetings(id).Select(ConvertToMeetingViewMOdel).ToList();
        }

        [Route("GetPlaces")]
        [HttpGet]
        public IEnumerable<PlacesViewModel> GetPlaces()
        {
            var repository = new DatabaseRepository();
            return repository.GetAllPlaces().Select(ConvertToPlacesViewModel);
        }

        [Route("Join")]
        [HttpGet]
        public object Join(int id, int meetingId)
        {
            var repository = new DatabaseRepository();
            repository.JoinMeeting(id, meetingId);
            return new object();
        }

        [Route("Invite")]
        [HttpGet]
        public object Invite(int id, int targetId)
        {
            var repository = new DatabaseRepository();
            repository.InviteToMeeting(id, targetId);
            return new object();
        }

        [Route("BatchInvite")]
        [HttpGet]
        public object Invite(int id, string targetIds)
        {
            var repository = new DatabaseRepository();
            var targetIdsList = targetIds.Split(',');
            foreach (var targetId in targetIdsList)
            {
                repository.InviteToMeeting(id, Int32.Parse(targetId));
            }
            return new object();
        }

        [Route("InviteRandomEmployees")]
        [HttpGet]
        public object Invite(int id)
        {
            var repository = new DatabaseRepository();
            repository.InviteRandomEmployees(id);
            return new object();
        }

        [Route("ChangePlaceAndTime")]
        [HttpGet]
        public object ChangePlaceAndTime(int profileId, TimeSpan? time, string placeName)
        {
            var repository = new DatabaseRepository();
            repository.ChangePlaceAndTime(profileId, time, placeName);
            return new object();
        }

    }
}


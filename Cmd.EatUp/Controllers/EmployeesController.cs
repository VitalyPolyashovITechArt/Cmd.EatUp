
using System.Linq;
using System.Web.Http;
using Cmd.EatUp.Data;
using Cmd.EatUp.Models;
using Cmd.EatUpTests;
using System;
using System.Collections.Generic;


namespace Cmd.EatUp.Controllers
{
    [RoutePrefix("Employees")]
    public class EmployeesController : ApiController
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
    //    // GET api/<controller>
    //   //[Route("")]
    //    //[HttpGet]

    //[Route("Fill")]
    //[HttpGet]
    //    public void Fill()
    //    {
    //        var adapter = new SmgAdapter();
    //        var sessionId = adapter.Authenticate("vitaly.polyashov", "qwerty6");
    //        var employees = adapter.GetAllEmployees(sessionId);
    //        DatabaseRepository repository = new DatabaseRepository();
    //        //repository.SaveEmployees(employees);
    //    }

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

        [Route("GetProfileInfo")]
        [HttpGet]
        public ProfileInfoViewModel GetProfileInfo(int id)
        {
            ProfileInfoViewModel viewModel = new ProfileInfoViewModel();
           DatabaseRepository repository = new DatabaseRepository();
            var result = repository.GetProfile(id);
            viewModel.FirstName = result.FirstName;
            viewModel.LastName = result.LastName;
            viewModel.StartPreferredTime = result.Time.Value.AddMinutes(-30);
            viewModel.FinishPreferredTime = result.Time.Value.AddMinutes(30);
            viewModel.ImagePath = result.ImagePath;

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
            viewModel.Achievements = new List<EmployeeViewModel>();
            foreach (var employee in model.Employees)
            {
                viewModel.Achievements.Add(ConvertToEmployeeViewMOdel(employee));
            }

            return viewModel;
        }


        private EmployeeViewModel ConvertToEmployeeViewMOdel(Employee model)
        {
            var employeeViewMOdel = new EmployeeViewModel();
            employeeViewMOdel.FullName = String.Format("{0} {1}", model.FirstName, model.LastName);
            employeeViewMOdel.ImageUrl = model.ImagePath;
            employeeViewMOdel.Id = model.ProfileId.Value;
            return employeeViewMOdel;
        }

        [Route("GetPreferrablePeople")]
        [HttpGet]
        public List<EmployeeViewModel> GetPreferrablePeople(int id)
        {
            var repository = new DatabaseRepository();
            return repository.GetPreferrablePeople(id).Select(ConvertToEmployeeViewMOdel).ToList();
        }

        private MeetingViewModel ConvertToMeetingViewModel(Meeting model)
        {
            return new MeetingViewModel()
            {
                Date = model.Time,
                PlaceName = model.Place.Name
            };
        }

        [Route("GetMeetings")]
        [HttpGet]
        public List<MeetingViewModel> GetMeetings(int id)
        {
            var repository = new DatabaseRepository();
            return repository.GetMeetings(id).Select(ConvertToMeetingViewModel).ToList();
        }

        [Route("Join")]
        [HttpGet]
        public void Join(int id, int meetingId)
        {
            var repository = new DatabaseRepository();
            repository.JoinMeeting(id, meetingId);
        }

        [Route("Invite")]
        [HttpGet]
        public void Invite(int id, int targetId)
        {
            var repository = new DatabaseRepository();
            repository.InviteToMeeting(id, targetId);
        }
    }
}



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
        // GET api/<controller>
       //[Route("")]
        //[HttpGet]

    [Route("Fill")]
    [HttpGet]
        public void Fill()
        {
            var adapter = new SmgAdapter();
            var sessionId = adapter.Authenticate("vitaly.polyashov", "qwerty6");
            var employees = adapter.GetAllEmployees(sessionId);
            DatabaseRepository repository = new DatabaseRepository();
            //repository.SaveEmployees(employees);
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

            //repository.GetInvitations()

            return viewModel;
        }

        
    }
}


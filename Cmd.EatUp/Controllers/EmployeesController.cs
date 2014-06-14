using System.Reflection.Emit;
using System.Web.Http;
using Cmd.EatUp.Data;
using Cmd.EatUp.Models;
using Cmd.EatUpTests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.Owin.Host.SystemWeb;
using Microsoft.Owin.Security;

namespace Cmd.EatUp.Controllers
{
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
        public void Fill()
        {
            var adapter = new SmgAdapter();
            var sessionId = adapter.Authenticate("vitaly.polyashov", "qwerty6");
            var employees = adapter.GetAllEmployees(sessionId);
            DatabaseRepository repository = new DatabaseRepository();
            repository.SaveEmployees(employees);
        }

        [System.Web.Http.Route("Authenticate")]
        [System.Web.Http.HttpGet]
        public string Authenticate(string userName, string password)
        {
            string sessionId = String.Empty;
            var smgAdapter = new SmgAdapter();
            try
            {
                sessionId = smgAdapter.Authenticate(userName, password);
                return sessionId;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [System.Web.Mvc.Route("GetProfileInfo/{id}")]

        public ProfileInfoViewModel GetProfileInfo(int id)
        {
            ProfileInfoViewModel viewModel = new ProfileInfoViewModel();
           DatabaseRepository repository = new DatabaseRepository();
            var result = repository.GetProfile(id);
            viewModel.FirstName = result.FirstName;
            return viewModel;
        }

        
    }
}


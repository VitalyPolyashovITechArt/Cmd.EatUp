using Cmd.EatUp.Data;
using Cmd.EatUpTests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Cmd.EatUp.Controllers
{
    public class EmployeesController : ApiController
    {
        // GET api/<controller>
        public void Fill()
        {
            var adapter = new SmgAdapter();
            var sessionId = adapter.Authenticate("vitaly.polyashov", "qwerty6");
            var employees = adapter.GetAllEmployees(sessionId);
            DatabaseRepository repository = new DatabaseRepository();
            repository.SaveEmployees(employees);
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}
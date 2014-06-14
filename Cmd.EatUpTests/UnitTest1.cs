using System;
using System.Diagnostics;
using Cmd.EatUp.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cmd.EatUpTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var adapter = new SmgAdapter();
            var sessionId = adapter.Authenticate("vitaly.polyashov", "qwerty6");
            var employees = adapter.GetAllEmployees(sessionId);
            DatabaseRepository repository = new DatabaseRepository();
            repository.SaveEmployees(employees);
            Debug.WriteLine("Employees count: ", employees.Count);
        }
    }
}

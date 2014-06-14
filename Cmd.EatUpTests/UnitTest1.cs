using System;
using System.Diagnostics;
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
            var sessionId = adapter.Authenticate("vyacheslav.bakhtin", "buypowerball");
            var employees = adapter.GetAllEmployees(sessionId);
            Debug.WriteLine("Employees count: ", employees.Count);
        }
    }
}

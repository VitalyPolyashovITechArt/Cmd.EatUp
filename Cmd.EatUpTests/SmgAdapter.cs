using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Cmd.EatUp.Data;
using Newtonsoft.Json;

namespace Cmd.EatUpTests
{
    public class SmgAdapter
    {
        public string Authenticate(string userName, string password)
        {
            HttpWebRequest request = CreateAuthenticateRequest(userName, password);
            using (WebResponse response = request.GetResponse())
            {
                using (var streamReader = new StreamReader(response.GetResponseStream()))
                {
                    string soapResult = streamReader.ReadToEnd();
                    return ParseSessionId(soapResult);
                }
            }
        }

        private string ParseSessionId(string soapResult)
        {
            dynamic resultObj = JsonConvert.DeserializeObject(soapResult);
            return resultObj.SessionId;
        }

        public List<Employee> GetAllEmployees(string sessionId)
        {
            HttpWebRequest request = CreateGetAllEmployeesRequest(sessionId);
            using (WebResponse response = request.GetResponse())
            {
                using (var streamReader = new StreamReader(response.GetResponseStream()))
                {
                    string soapResult = streamReader.ReadToEnd();
                    return ParseEmployees(soapResult);
                }
            }
        }

        private List<Employee> ParseEmployees(string soapResult)
        {
            var employees = new List<Employee>();
            dynamic resultObj = JsonConvert.DeserializeObject(soapResult);
            foreach (var profile in resultObj.Profiles)
            {
                var employee = new Employee()
                {
                    DepartmentId = profile.DeptId,
                    FirstName = profile.FirstName,
                    Id = profile.ProfileId,
                    ImagePath = profile.Image,
                    LastName = profile.LastName,
                    Room = profile.Room
                };
                employees.Add(employee);
            }
            return employees;
        }

        private HttpWebRequest CreateAuthenticateRequest(string userName, string password)
        {
            var webRequest = (HttpWebRequest)WebRequest.Create(String.Format(@"https://smg.itechart-group.com/MobileServiceNew/MobileService.svc/Authenticate?username={0}&password={1}", userName, password));
            webRequest.Headers.Add(@"SOAP:Action");
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Method = "GET";
            return webRequest;
        }

        private HttpWebRequest CreateGetAllEmployeesRequest(string sessionId)
        {
            var webRequest = (HttpWebRequest)WebRequest.Create(String.Format(@"https://smg.itechart-group.com/MobileServiceNew/MobileService.svc/GetAllEmployees?sessionId={0}", sessionId));
            webRequest.Headers.Add(@"SOAP:Action");
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Method = "GET";
            return webRequest;
        }
    }
}

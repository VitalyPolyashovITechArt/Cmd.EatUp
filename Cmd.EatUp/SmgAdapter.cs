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
                    ProfileId = profile.ProfileId,
                    ImagePath = profile.Image,
                    LastName = profile.LastName,
                    Room = profile.Room,
                    Position = profile.POsition,
                    StringId = (profile.FirstNameEng + "." + profile.LastNameEng).ToString().ToLower()
                };
                employees.Add(employee);
            }
            return employees;
        }

        public void ExtendEmployeeInfo(string sessionId, Employee employee)
        {
            HttpWebRequest request = CreateGetEmployeeDetailsRequest(sessionId, employee.ProfileId.Value);
            using (WebResponse response = request.GetResponse())
            {
                using (var streamReader = new StreamReader(response.GetResponseStream()))
                {
                    string soapResult = streamReader.ReadToEnd();
                    ExtendEmployee(employee, soapResult);
                }
            }
        }

        private void ExtendEmployee(Employee employee, string soapResult)
        {
            dynamic resultObj = JsonConvert.DeserializeObject(soapResult);
            employee.Birthday = EpochToDateTime(resultObj.Birthday);
            employee.Position = resultObj.Rank;
        }

        private DateTime EpochToDateTime(string epochDate)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var ticks = epochDate.Substring(7, epochDate.Length - 15);
            return epoch.AddMilliseconds(double.Parse(ticks));
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

        private HttpWebRequest CreateGetEmployeeDetailsRequest(string sessionId, int profileId)
        {
            var webRequest = (HttpWebRequest)WebRequest.Create(String.Format(@"https://smg.itechart-group.com/MobileServiceNew/MobileService.svc/GetEmployeeDetails?sessionId={0}&profileId={1}", sessionId, profileId));
            webRequest.Headers.Add(@"SOAP:Action");
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Method = "GET";
            return webRequest;
        }
    }
}

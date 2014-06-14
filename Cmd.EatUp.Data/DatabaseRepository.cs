using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cmd.EatUp.Data
{
    public class DatabaseRepository
    {
        private eatupEntities1 context = new eatupEntities1();
        public void SaveEmployees(IEnumerable<Employee> employees)
        {
            context.Employees.AddRange(employees);
            context.SaveChanges();
        }

        public Employee GetProfile(int id)
        {
            return context.Employees.Single(x => x.Id == id);
        }

        public IEnumerable<Employee> GetPreferrablePeople(int id)
        {
            Employee currentEmployee = GetProfile(id);

            DateTime startTime = currentEmployee.Time.Value.AddMinutes(-30);
            return null;
            // context.Meetings.Where(x => x.Time.Day ==  )
        }

    }
}

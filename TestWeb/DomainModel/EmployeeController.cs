using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace TestWeb.DomainModel
{
    public class EmployeeController
    {
        private List<Employee> employees;
        
        public EmployeeController()
        {
            this.employees = new List<Employee>();

            //  Create 5 employees for testing
            this.employees.Add(new Employee()
            {
                FirstName = "Elvis",
                LastName = "Presley",
                DepartmentId = 13,
                StartDate = new DateTime(2012, 3, 4),
                Id = "2"
            });

            this.employees.Add(new Employee()
            {
                FirstName = "James",
                LastName = "Kirk",
                DepartmentId = 1701,
                StartDate = new DateTime(1966, 3, 4),
                Id = "3"
            });

            this.employees.Add(new Employee()
            {
                FirstName = "Dolly",
                LastName = "Parton",
                DepartmentId = 44,
                StartDate = new DateTime(2001, 12, 12),
                Id = "4"
            });

            this.employees.Add(new Employee()
            {
                FirstName = "David",
                LastName = "Coverdale",
                DepartmentId = 13,
                StartDate = new DateTime(184, 3, 22),
                Id = "5"
            });

            this.employees.Add(new Employee()
            {
                FirstName = "John C.",
                LastName = "Riley",
                DepartmentId = 7,
                StartDate = new DateTime(1997, 8, 14)
            });
        }

        public string GetAll()
        {
            return JsonConvert.SerializeObject(this.employees);
        }

        public string CreateEmployee(string jsonEmployee)
        {
            var employee = JsonConvert.DeserializeObject<Employee>(jsonEmployee);
            employee.Id = (this.employees.Count + 1).ToString();
            employees.Add(employee);

            return employee.Id;
        }

        public string Get(string id)
        {
            var employee =  this.employees.FirstOrDefault(x => x.Id == id) ?? new Employee();
            return JsonConvert.SerializeObject(employee);
        }

        public void Delete(string id)
        {
            this.employees.Remove(this.employees.Find(x => x.Id == id));
        }
    }   
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestWeb.DomainModel
{
    public class Employee
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int DepartmentId { get; set; }
        public DateTime StartDate { get; set; }
        public string Id { get; set; }
    }
}
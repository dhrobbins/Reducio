using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using TestWeb.DomainModel;
using Reducio.Data;

namespace TestWeb.Services
{
    /// <summary>
    /// Summary description for APoorWebService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    //[System.Web.Script.Services.ScriptService]
    public class APoorWebService : System.Web.Services.WebService
    {

        [WebMethod]
        public string GetEmployee(string id)
        {
            var employeeController = new EmployeeController();
            return employeeController.Get(id);
        }

        [WebMethod]
        public string GetAllEmployees()
        {
            var employeeController = new EmployeeController();
            return employeeController.GetAll();
        }

        [WebMethod]
        public void DeleteEmployee(string id)
        {
            var employeeController = new EmployeeController();
            employeeController.Delete(id);
        }

        [WebMethod]
        public string CreateEmployee(string jsonEmployee)
        { 
            var employeeController = new EmployeeController();
            return employeeController.CreateEmployee(jsonEmployee);
        }
    }
}

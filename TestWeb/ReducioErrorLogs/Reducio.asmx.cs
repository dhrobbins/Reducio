using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using Reducio.Core;
using Reducio.Data;
using Reducio.Utils;

namespace TestWeb.Services
{
    /// <summary>
    /// Summary description for ReducioData
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class ReducioData : System.Web.Services.WebService
    {

        [WebMethod]
        public void LogError(string jsonError)
        {
            Enforce.That(string.IsNullOrEmpty(jsonError) == false,
                                "ErrorLogging.LogError - jsonError can not be null");

            var errorLoggingController = new ErrorLoggingController();
            errorLoggingController.LogIncident(jsonError, Application["RavenDataController"] as RavenDataController);
        }
    }
}

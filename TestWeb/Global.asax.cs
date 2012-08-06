using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using Raven.Client;
using Raven.Client.Indexes;
using Reducio.Core;
using Reducio.Data;
using System.Configuration;
using Raven.Client.Document;

namespace TestWeb
{
    public class Global : System.Web.HttpApplication
    {

        void Application_Start(object sender, EventArgs e)
        {
            //  Connect to RavenDB
            string connection = ConfigurationManager.ConnectionStrings["RavenDBLocal"].ConnectionString;
            DocumentStore documentStore = new DocumentStore() { Url = connection };
            documentStore.Initialize();

            var dataController = new RavenDataController(documentStore);
            Application["RavenDataController"] = dataController;

            //  Create Indexes
            CreateReducioIndexes(documentStore);
        }

        void Application_End(object sender, EventArgs e)
        {
            //  Code that runs on application shutdown

        }

        void Application_Error(object sender, EventArgs e)
        {
            //  A server side exception was not handled.  Create an incident
            var errorController = new ErrorLoggingController();

            var incident = new Incident();
            incident.OriginalErrorMessage = Server.GetLastError().Message;
            errorController.LogIncident(incident, Application["RavenDataController"] as RavenDataController);

        }

        void Session_Start(object sender, EventArgs e)
        {
            // Code that runs when a new session is started
            Session["UserSessionGuid"] = System.Guid.NewGuid().ToString();
        }

        void Session_End(object sender, EventArgs e)
        {
            // Code that runs when a session ends. 
            // Note: The Session_End event is raised only when the sessionstate mode
            // is set to InProc in the Web.config file. If session mode is set to StateServer 
            // or SQLServer, the event is not raised.

        }

        private void CreateReducioIndexes(DocumentStore documentStore)
        {
            Raven.Client.Indexes.IndexCreation.CreateIndexes(typeof(UnresolvedIncidentIndex).Assembly, documentStore);
            Raven.Client.Indexes.IndexCreation.CreateIndexes(typeof(DistinctIncidentTypeIndex).Assembly, documentStore);
        }

    }
}

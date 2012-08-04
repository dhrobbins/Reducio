using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reducio.Data;
using Reducio.Core;
using NUnit.Framework;
using Raven.Client.Document;
using Newtonsoft.Json;

namespace TestSuite
{
    [TestFixture]
    public class ErrorLoggingTests
    {
        /// <summary>
        /// Should be able to create an indicent and store in Incident document
        /// </summary>
        [Test]
        [Category("ErrorLogging")]
        public void CanLogIncident()
        {
            //  Prep
            var docStore = GetDocStore();
            TruncateIncidentDocuments(docStore);
            int postUpdateCount = 0;

            var newIncident = CreateTransitionError();
            var repository = new RavenDataController(docStore);

            var errorLoggingController = new ErrorLoggingController();
            string json = JsonConvert.SerializeObject(newIncident);
            errorLoggingController.LogIncident(json, repository);

            using (var session = docStore.OpenSession())
            {
                postUpdateCount = session.Query<Incident>()
                                            .Customize(x => x.WaitForNonStaleResults())
                                            .Count();
            }

            Assert.AreEqual(1, postUpdateCount);
        }

        /// <summary>
        /// Should be able to find a previous incident with the same hash title
        /// </summary>
        [Test]
        [Category("ErrorLogging")]
        public void CanFindParentIncident()
        {
            //  Prep
            var docStore = GetDocStore();
            TruncateIncidentDocuments(docStore);

            var parentIncident = CreateTransitionError();
            parentIncident.IncidentType = new IncidentType("ParentTypeError", new DateTime(2012, 1, 31),
                                                "The Parent Type", "Truncate database");

            var repository = new RavenDataController(docStore);
            repository.Save<Incident>(parentIncident);

            //  2 more incidents for our searching
            var incident2 = CreateTransitionError();
            incident2.PageName = "Incident 2";
            repository.Save<Incident>(incident2);

            var incident3 = CreateTransitionError();
            incident3.PageName = "Incident 3";
            repository.Save<Incident>(incident3);

            //  Test
            var errorLoggingController = new ErrorLoggingController();
            var foundParent = errorLoggingController.FindParent(parentIncident.HashedTitle, repository);

            Console.WriteLine(parentIncident.Id);
            Assert.AreEqual(parentIncident.IncidentType.Type, foundParent.IncidentType.Type);
        }

        /// <summary>
        /// Should be able to associate a new incident with the first occurance
        /// of that incident.  That is, when an incident has been logged, find it and save the 
        /// id as ParentId with the secondary incident
        /// </summary>
        [Test]
        [Category("ErrorLogging")]
        public void CanLogIncidentAndAssociateToParent()
        {
            //  Prep
            var docStore = GetDocStore();
            TruncateIncidentDocuments(docStore);

            //  Parent
            var parentIncident = CreateTransitionError();
            parentIncident.IncidentType = new IncidentType("ParentTypeError", new DateTime(2012, 1, 31),
                                                "The Parent Type", "Truncate database");

            var repository = new RavenDataController(docStore);
            repository.Save<Incident>(parentIncident);

            //  2 more incidents for our searching
            var incident2 = CreateTransitionError();
            incident2.PageName = "Incident 2";
            repository.Save<Incident>(incident2);

            var incident3 = CreateTransitionError();
            incident3.PageName = "Incident 3";
            repository.Save<Incident>(incident3);

            //The child incident
            var childincident = CreateTransitionError();
            var errorController = new ErrorLoggingController();
            string childId = errorController.LogIncident(JsonConvert.SerializeObject(childincident), repository);

            var savedChild = repository.Get<Incident>(childId);

            Console.WriteLine("Child Parent Id " + savedChild.ParentIncidentId + " : Parent Id " + parentIncident.Id);
            Assert.AreEqual(parentIncident.Id, savedChild.ParentIncidentId);
        }

        /// <summary>
        /// Should be able to delete an incident
        /// </summary>
        [Test]
        [Category("ErrorLogging")]
        public void CanDeleteIncident()
        {
            //  Prep
            var docStore = GetDocStore();
            TruncateIncidentDocuments(docStore);

            var repository = new RavenDataController(docStore);
            var incident = CreateTransitionError();
            incident.PageName = "FindAnDeleteMe";

            var errorLoggingController = new ErrorLoggingController();
            string id = errorLoggingController
                        .LogIncident(JsonConvert.SerializeObject(incident), repository);

            int postLogCount = 0;

            using (var session = docStore.OpenSession())
            {
                postLogCount = session.Query<Incident>()
                                        .Customize(x => x.WaitForNonStaleResults())
                                        .Count();
            }

            //  Test
            errorLoggingController.DeleteIncident(id, repository);

            int postDeleteCount = 0;
            using (var session = docStore.OpenSession())
            {
                postDeleteCount = session.Query<Incident>()
                                        .Customize(x => x.WaitForNonStaleResults())
                                        .Count();
            }

            Assert.AreEqual(postLogCount, postDeleteCount + 1);

        }

        /// <summary>
        /// Should be able to get all incidents 
        /// </summary>
        [Test]
        [Category("ErrorLogging")]
        public void CanFetchAllIncidents()
        {
            var docStore = GetDocStore();
            TruncateIncidentDocuments(docStore);

            var batchIndicents = Create15Incidents();
            var dataController = new RavenDataController(docStore);
            batchIndicents.ForEach(x => dataController.Save<Incident>(x));

            int totalCount = 0;
            using(var session = docStore.OpenSession())
            {
                totalCount = session.Query<Incident>()
                                    .Customize(x => x.WaitForNonStaleResults())
                                    .Count();
            }

            //  Test
            var allIncidents = dataController.GetAll<Incident>();
            Assert.AreEqual(totalCount, allIncidents.Count);

        }

        /// <summary>
        /// Should be able to fetch only the incidents where IncidentType.Type == Javascript
        /// </summary>
        [Test]
        [Category("ErrorLoggingQuery")]
        public void CanQueryForJavascriptIncidentType()
        {
            var docStore = GetDocStore();
            TruncateIncidentDocuments(docStore);

            var batchIndicents = Create15Incidents();
            var dataController = new RavenDataController(docStore);
            batchIndicents.ForEach(x => dataController.Save<Incident>(x));

            int totalJavascriptCount = 0;

            using(var session = docStore.OpenSession())
            {
                totalJavascriptCount = session.Query<Incident>()
                                                .Customize(x => x.WaitForNonStaleResults())
                                                .Where(x => x.IncidentType.Type == "javascript")
                                                .Count();
            }

            //  Test
            var javascriptIncidents = dataController.GetAllWhere<Incident>(inc => inc.IncidentType.Type == "javascript");
            Assert.AreEqual(totalJavascriptCount, javascriptIncidents.Count);
        }

        [Test]
        [Category("ErrorLoggingQuery")]
        public void CanGetDistincIncidentTypeByIndex()
        {
            var docStore = GetDocStore();
            TruncateIncidentDocuments(docStore);

            var batchIndicents = Create15Incidents();
            var dataController = new RavenDataController(docStore);
            batchIndicents.ForEach(x => dataController.Save<Incident>(x));
            
            Raven.Client.Indexes.IndexCreation.CreateIndexes(typeof(DistinctIncidentTypeIndex).Assembly, docStore);

            var distinctIncidentTypes = dataController
                                        .GetAll<DistinctIncidentTypeIndex.DistinctIncidentType>
                                                ("DistinctIncidentTypeIndex");

            Assert.AreEqual(4, distinctIncidentTypes.Count);                                            
        }

        [Test]
        [Category("ErrorLoggingQuery")]
        public void CanQueryIndexForJavascriptIncidentType()
        {
            var docStore = GetDocStore();
            TruncateIncidentDocuments(docStore);

            var batchIndicents = Create15Incidents();
            var dataController = new RavenDataController(docStore);
            batchIndicents.ForEach(x => dataController.Save<Incident>(x));

            Raven.Client.Indexes.IndexCreation.CreateIndexes(typeof(DistinctIncidentTypeIndex).Assembly, docStore);

            var javascriptIncidentType = dataController.GetAllWhere<DistinctIncidentTypeIndex.DistinctIncidentType>
                                                            (x => x.Name == "javascript",
                                                            "DistinctIncidentTypeIndex");

            Assert.AreEqual(5, javascriptIncidentType[0].Count);
        }

        [Test]
        [Category("ErrorLoggingQuery")]
        public void CanQueryIndexForParentIncidents()
        {
            var docStore = GetDocStore();
            TruncateIncidentDocuments(docStore);

            var batchIncidents = Create15Incidents();
            var dataController = new RavenDataController(docStore);
            batchIncidents.ForEach(x => dataController.Save<Incident>(x));

            var parentIncidents = dataController.GetAll<Incident>("ParentIncidentIndex");
            Assert.AreEqual(2, parentIncidents.Count);
        }

        [Test]
        [Category("ErrorLoggingQuery")]
        public void CanQueryIndexForUnresolvedIncidents()
        {
            var docStore = GetDocStore();
            TruncateIncidentDocuments(docStore);

            var batchIncidents = Create15Incidents();

            //  Mark 5 of these as resolved
            for (int i = 0; i <= 4; i++ )
            {
                batchIncidents[i].Resolved = true;
            }

            var dataController = new RavenDataController(docStore);
            batchIncidents.ForEach(x => dataController.Save<Incident>(x));

            Raven.Client.Indexes.IndexCreation.CreateIndexes(typeof(UnresolvedIncidentIndex).Assembly, docStore);

            var unresolved = dataController.GetAll<Incident>("UnresolvedIncidentIndex");
            Assert.AreEqual(10, unresolved.Count);
        }

        [TestFixtureSetUp]
        public void CreateIncidentTypes()
        {
            var docStore = GetDocStore();
            TruncateIncidentTypes(docStore);

            var dataController = new RavenDataController(docStore);

            var databaseType = new IncidentType("Database", new DateTime(2012, 3, 4), "Database offline", "reboot");
            dataController.Save<IncidentType>(databaseType);

            var javascriptType = new IncidentType("Javascript", new DateTime(2012, 3, 4), "jQuery not loaded", "review script");
            dataController.Save<IncidentType>(javascriptType);

            var nullObjectType = new IncidentType("Null Object Reference", new DateTime(2012, 3, 4), "Not record returned", "check database");
            dataController.Save<IncidentType>(nullObjectType);

            var pageNotFound = new IncidentType("Page Not Found", new DateTime(2012, 3, 4), "404", "check IIS");
            dataController.Save<IncidentType>(pageNotFound);

        }

        #region Helper Methods        

        private Incident CreateTransitionError()
        {
            var newIncident = new Incident();

            newIncident.PageName = "Transitions.aspx";
            newIncident.IncidentDateTime = DateTime.Now;
            newIncident.CurrentDOM = "Here is the DOM stuff";
            newIncident.Title = "NUnit testing generated error";

            return newIncident;
        }

        private List<Incident> Create15Incidents()
        {
            var incidents = new List<Incident>();

            //  4 Database IncidentTypes
            var databaseType = new IncidentType("Database", new DateTime(2012, 3, 4), "Database offline", "reboot");

            int i;
            for (i = 0; i <= 3; i++)
            {
                var incident = CreateTransitionError();
                incident.IncidentType = databaseType;
                incidents.Add(incident);
            }

            //  6 Javascript
            var javascriptType = new IncidentType("Javascript", new DateTime(2012, 3, 4), "jQuery not loaded", "review script");

            for (i = 0; i <= 5; i++)
            {
                var incident = CreateTransitionError();
                incident.IncidentType = javascriptType;
                incident.Title = incident.Title + " - " + i.ToString();
                incidents.Add(incident);
            }

            //  2 Null Object Reference
            var nullObjectType = new IncidentType("Null Object Reference", new DateTime(2012, 3, 4), "Not record returned", "check database");

            for (i = 0; i <= 1; i++)
            {
                var incident = CreateTransitionError();
                incident.IncidentType = nullObjectType;
                incident.PageName = incident.PageName + " - " + i.ToString();
                incidents.Add(incident);
            }

            //  3 Page Not Found
            var pageNotFound = new IncidentType("Page Not Found", new DateTime(2012, 3, 4), "404", "check IIS");

            for (i = 0; i <= 2; i++)
            {
                var incident = CreateTransitionError();
                incident.IncidentType = pageNotFound;
                incident.PageName = incident.PageName + " - " + i.ToString() + " " + incident.Title;
                incidents.Add(incident);
            }

            return incidents;
        }

        private DocumentStore GetDocStore()
        {
            var docStore = new DocumentStore() { Url = "http://localhost:8080" };
            docStore.Initialize();

            return docStore;
        }

        private void TruncateIncidentDocuments(DocumentStore docStore)
        {
            using (var session = docStore.OpenSession())
            {
                var documents = session.Query<Incident>().ToList();
                documents.ForEach(doc => session.Delete<Incident>(doc));
                session.SaveChanges();
            }
        }

        private void TruncateIncidentTypes(DocumentStore docStore)
        {
            using (var session = docStore.OpenSession())
            {
                var documents = session.Query<IncidentType>().ToList();
                documents.ForEach(doc => session.Delete<IncidentType>(doc));
                session.SaveChanges();
            }
        }

        #endregion
    }
}

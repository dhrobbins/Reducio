using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reducio.Data;
using Reducio.Utils;
using Newtonsoft.Json;

namespace Reducio.Core
{
    public class ErrorLoggingController
    {   
        private DateTime unspecifiedIncidentTypeDate;

        public ErrorLoggingController()
        {
            this.unspecifiedIncidentTypeDate = new DateTime(2012, 8, 1);
        }

        public Incident GetIncident(string id, RavenDataController dataController)
        { 
            Enforce.That(string.IsNullOrEmpty(id) == false, 
                        "ErrorLoggingController.GetIncident - id can not be null");

            return dataController.Get<Incident>(id);
        }

        /// <summary>
        /// Accept a json representation of an incident, deserialize, locate a potential parent incident
        /// and related incidents, and save to document storeage
        /// </summary>
        /// <param name="jsonIncident">Incident serialized as Json string</param>
        /// <param name="dataController">Document storeage as RavenDataController</param>
        /// <returns>Logged Incident Id as string</returns>
        public string LogIncident(string jsonIncident, RavenDataController dataController)
        {
            Enforce.That(string.IsNullOrEmpty(jsonIncident) == false,
                        "ErrorLoggingController.GetIncident - jsonIncident can not be null");
            
            var newIncident = JsonConvert.DeserializeObject<Incident>(jsonIncident);
            
            //  When the minimum data is missing catalog this as unserviceable then save
            if (newIncident.CanIncidentBeLogged() == false)
            {
                newIncident.Title = "Unspecified error!";
                newIncident.IncidentDateTime = DateTime.Now;
                newIncident.Catalogged = false;
                newIncident.PageName = "No page name!";

                var unspecifiedIncidentType = new IncidentType("Unspecified", DateTime.Now, "Error logging did not capture results", 
                                                                        "Gather more data");
                newIncident.IncidentType = unspecifiedIncidentType;
            }

            //  Has this occurred before?  Associate to parent / primary occurence
            var parentIncident = FindParent(newIncident.HashedTitle, dataController);
            if(string.IsNullOrEmpty(parentIncident.Id) == false)
            {
                newIncident.ParentIncidentId = parentIncident.Id;
                newIncident.RelatedIncidents = parentIncident.RelatedIncidents;
                newIncident.Resolved = parentIncident.Resolved;
                newIncident.Catalogged = parentIncident.Catalogged;
            }

            dataController.Save<Incident>(newIncident);

            return newIncident.Id;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="repository"></param>
        public void DeleteIncident(string id, RavenDataController dataController)
        {
            Enforce.That(string.IsNullOrEmpty(id) == false, 
                                "ErrorLoggingController.DeleteIncident - id can not be null");
            
            dataController.Delete<Incident>(id);
        }

        /// <summary>
        /// Given a hash of an incident title, find the very first occurence of an incident with the 
        /// same hashed title.
        /// </summary>
        /// <param name="hashTitle">Title hash as string</param>
        /// <param name="repository">Document storage as RavenDataController</param>
        /// <returns>On sucess, the parent incident.  On failure a new Incident with no Id</returns>
        public Incident FindParent(string hashTitle, RavenDataController dataController)
        {
            Enforce.That(string.IsNullOrEmpty(hashTitle) == false, "ErrorLoggingController.FindParent - hashTitle can not be null");
            
            var results = dataController.GetAllWhere<Incident>(x => x.HashedTitle == hashTitle)
                                        .ToList<Incident>();

            if((results == null) | (results.Count == 0))
            {
               return new Incident(); 
            }

            var parent = results.Aggregate((item, comp) => item.IncidentDateTime < comp.IncidentDateTime ? item : comp);
            
            if (parent == null)
            {
                return new Incident();
            }

            return parent;
        }

        public List<Incident> GetParentIncidents(RavenDataController dataController)
        {
            return dataController.GetAllWhere<Incident>(x => string.IsNullOrEmpty(x.ParentIncidentId));
        }
    }
}

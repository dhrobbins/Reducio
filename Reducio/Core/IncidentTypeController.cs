using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client.Document;
using Reducio.Data;
using Reducio.Utils;
using Newtonsoft.Json;

namespace Reducio.Core
{
    public class IncidentTypeController
    {
        #region Members
        #endregion

        #region Properties
        #endregion

        #region CTOR

        public IncidentTypeController(){}

        #endregion

        #region Methods

        /// <summary>
        /// Save an IncidentType to document store
        /// </summary>
        /// <param name="jsonIncidentType">Serilized IncidentType as Json string</param>
        /// <param name="repository">Document Storage as RavenDataController</param>
        /// <returns>Id of saved IncidentType as string</returns>
        public string CreateIncidentType(string jsonIncidentType, RavenDataController dataController)
        {
            Enforce.That(string.IsNullOrEmpty(jsonIncidentType) == false,
                                    "ErrorLoggingConntroller.CreateIncidentType - jsonIncidentType can not be null");

            var incidentType = JsonConvert.DeserializeObject<IncidentType>(jsonIncidentType);
            dataController.Save<IncidentType>(incidentType);

            return incidentType.Id;
        }

        /// <summary>
        /// Fetch an IncidentType for a Id
        /// </summary>
        /// <param name="id">Id as string</param>
        /// <param name="dataController">Document storeage as RavenDataController</param>
        /// <returns>The IncidentType, Id == null on failure</returns>
        public IncidentType GetIncidentType(string id, RavenDataController dataController)
        { 
            Enforce.That(string.IsNullOrEmpty(id), 
                                "IncidentTypeController.GetIncidentType - id can not be null");

            return dataController.Get<IncidentType>(id);
        }

        /// <summary>
        /// Fetch all IncidentTypes
        /// </summary>
        /// <param name="dataController">Document storeage as RavenDataController</param>
        /// <returns></returns>
        public List<IncidentType> GetAllIncidentTypes(RavenDataController dataController)
        {
            return dataController.GetAll<IncidentType>();
        }

        /// <summary>
        /// Delete an IncidentType with the supplied id
        /// </summary>
        /// <param name="id">Id as string</param>
        /// <param name="dataController">Document storeage as RavenDataController</param>
        public void DeleteIncidentType(string id, RavenDataController dataController)
        {
            Enforce.That(string.IsNullOrEmpty(id),
                               "IncidentTypeController.DeleteIncidentType - id can not be null");

            dataController.Delete<IncidentType>(id);
        }
        
        /// <summary>
        /// When an update is performed against an IncidentType,
        /// propogate changes to all incidents with this type
        /// </summary>
        public void CascadeUpdateForIncidentType(IncidentType incidentType, 
                                                        RavenDataController dataController)
        { 
            
        }

        /// <summary>
        /// Remove the parent association from an incident
        /// </summary>
        public void BreakParentRelationship(string parentIncidentId, string incidentId, 
                                                        RavenDataController dataController)
        { 
            
        }

        /// <summary>
        /// A change to a IncidentType.Type on a parent propogates to all of the
        /// children Incidents.  That is, switching IncidentType = database to IncidentType
        /// = javascript updates all Incidents
        /// </summary>
        /// <param name="id"></param>
        /// <param name="incidentTypeId"></param>
        public void CascadeParentIncidentType(string id, string incidentTypeId, 
                                                        RavenDataController dataController)
        {

        }

        /// <summary>
        /// Fetch distribution of IncidentTypes:  count of of each IncidentType
        /// </summary>
        /// <param name="dataController"></param>
        /// <returns>IncidentType.Type and Count as Json string</returns>
        public string GetIncidentTypeDistribution(RavenDataController dataController)
        {
            var distribution = dataController.GetAll<DistinctIncidentTypeIndex
                                                    .DistinctIncidentType>("DistinctIncidentTypeIndex");

            return JsonConvert.SerializeObject(distribution);
        }

        #endregion

    }
}

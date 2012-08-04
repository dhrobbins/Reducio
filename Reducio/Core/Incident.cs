using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using Reducio.Utils;
using Janga.Validation;

namespace Reducio.Core
{
    public class Incident
    {
        public string Id { get; set; }
        public string ParentIncidentId { get; set; }
        public string PageName { get; set; }

        private string title;
        public string Title
        {
            get
            {
                if (string.IsNullOrEmpty(this.title))
                {
                    return string.Empty;
                }
                else
                {
                    return this.title;
                }
            }

            set
            {
                this.title = value;
                this.HashTitle();
            }
        }

        public string HashedTitle { get; set; }
        public DateTime IncidentDateTime { get; set; }
        public string OriginalErrorMessage { get; set; }
        public string CurrentDOM { get; set; }
        public string Notes { get; set; }
        public string ClientData { get; set; }

        public IncidentType IncidentType { get; set; }
        public List<string> RelatedIncidents { get; set; }
        public bool Resolved { get; set; }
        public bool Catalogged { get; set; }
        public string UserSessionGuid { get; set; }

        public Incident()
            : this(string.Empty, string.Empty, string.Empty, DateTime.Now, string.Empty,
                        string.Empty, string.Empty, new IncidentType(),
                       new List<string>(), false, false, string.Empty, string.Empty) { }

        public Incident(string parentIncidentId, string pageName, string title, DateTime incidentDateTime, 
                        string currentDOM, string originalErrorMessage, string notes,
                        IncidentType incidentType, List<string> relatedIncidents, 
                                bool resolved, bool catalogged, string userSessionGuid, string clientData)
        {
            this.PageName = pageName;
            this.ParentIncidentId = parentIncidentId;
            this.Title = title;
            this.IncidentDateTime = incidentDateTime;
            this.OriginalErrorMessage = originalErrorMessage;
            this.CurrentDOM = currentDOM;
            this.Notes = notes;

            this.IncidentType = incidentType;
            this.RelatedIncidents = RelatedIncidents;
            this.Resolved = resolved;
            this.Catalogged = catalogged;
            this.UserSessionGuid = userSessionGuid;
            this.ClientData = clientData;

            if (this.Title.Length > 0)
            {
                HashTitle();
            }
        }

        /// <summary>
        /// Ensure the minimum items ofr logging are enforced.  Need IncidentDateTime, CurrentDOM, PageName
        /// </summary>
        /// <returns>True when valid</returns>
        public bool CanIncidentBeLogged()
        {
            return this.Enforce<Incident>("Incident", true)
                        .When("Title", Compare.NotEqual, string.Empty)
                        .When("HashedTitle", Compare.NotEqual, string.Empty)
                        .When("IncidentDateTime", Compare.NotEqual, DateTime.MinValue)
                        .When("CurrentDOM", Compare.NotEqual, string.Empty)
                        .When("PageName", Compare.NotEqual, string.Empty)
                        .IsValid;
        }

        /// <summary>
        /// Taken from Microsoft samples.  Create a hash of the title with MD5 crypto
        /// </summary>
        private void HashTitle()
        {
            byte[] hashedBytes;
            byte[] titleAsBytes = ASCIIEncoding.ASCII.GetBytes(this.Title.ToLower());

            hashedBytes = new MD5CryptoServiceProvider().ComputeHash(titleAsBytes);
            this.HashedTitle = ByteArrayToString(hashedBytes);
        }

        /// <summary>
        /// Given an array of bytes return as string
        /// </summary>
        /// <returns></returns>
        private string ByteArrayToString(byte[] byteArray)
        {
            int i;
            StringBuilder output = new StringBuilder(byteArray.Length);

            for (i = 0; i < byteArray.Length; i++)
            {
                output.Append(byteArray[i].ToString("X2"));
            }

            return output.ToString();
        }
    }
}

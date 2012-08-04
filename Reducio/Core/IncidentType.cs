using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reducio.Core
{
    public class IncidentType
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public DateTime DateIdentified { get; set; }
        public string Description { get; set; }
        public string Resolution { get; set; }

        public IncidentType() : this(string.Empty, DateTime.Now, string.Empty, string.Empty) { }

        public IncidentType(string type, DateTime dateIdentified, string description, string resolution)
        {
            this.Type = type;
            this.DateIdentified = dateIdentified;
            this.Description = description;
            this.Resolution = Resolution;
        }
    }
}

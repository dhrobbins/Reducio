using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client.Indexes;

namespace Reducio.Core
{
    public class DistinctIncidentTypeIndex : AbstractIndexCreationTask<Incident, DistinctIncidentTypeIndex.DistinctIncidentType>
    {
        public class DistinctIncidentType
        {
            public string Name { get; set; }
            public int Count { get; set; }
        }

        public DistinctIncidentTypeIndex()
        {
            Map = docs => from doc in docs
                          select new
                          {
                              Name = doc.IncidentType.Type.ToLower(),
                              Count = 1
                          };
            Reduce = results => from result in results
                                group result by result.Name into g
                                select new
                                {
                                    Name = g.Key,
                                    Count = g.Sum(x => x.Count)
                                };
        }
    }
}

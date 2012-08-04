using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client.Indexes;
using Raven.Abstractions.Indexing;

namespace Reducio.Core
{
    public class UnresolvedIncidentIndex : AbstractIndexCreationTask<Incident>
    {
        public UnresolvedIncidentIndex()
        {
            Map = docs => from doc in docs
                          where doc.Resolved == false
                          select new { Title = doc.Title, Id = doc.Id };
            Index("Title", FieldIndexing.Analyzed);
            Index("Id", FieldIndexing.Analyzed);
        }
    }
}

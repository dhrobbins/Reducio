using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client.Indexes;
using Raven.Abstractions.Indexing;

namespace Reducio.Core
{
    public class UncataloggedIncidentIndex : AbstractIndexCreationTask<Incident>
    {
        public UncataloggedIncidentIndex()
        {
            Map = docs => from doc in docs
                          where doc.Catalogged == false
                          select new { doc.Title, doc.Id };
        }
    }
}

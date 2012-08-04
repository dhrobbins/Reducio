using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client.Document;
using Raven.Client.Authorization;
using Reducio.Utils;

namespace Reducio.Data
{
    public class RavenDataController
    {
        #region Members

        private DocumentStore documentStore;

        #endregion

        #region Properties

        public string UserId { get; set; }
        public string Operation { get; set; }

        #endregion

        #region CTOR

        public RavenDataController(DocumentStore documentStore)
        {
            this.documentStore = documentStore;

            this.UserId = string.Empty;
            this.Operation = string.Empty;
        }

        public RavenDataController(DocumentStore docStore, string userId, string operation)
        {
            this.documentStore = docStore;

            this.UserId = userId;
            this.Operation = operation;
        }

        #endregion

        #region Methods

        public T Get<T>(string id)
        {
            Enforce.That(string.IsNullOrEmpty(id) == false,
                                "RavenDataController.Get - id string can not be null");
            
            using (var session = this.documentStore.OpenSession())
            {
                return session.Load<T>(id);
            }
        }

        public void Delete<T>(string id)
        {
            Enforce.That(string.IsNullOrEmpty(id) == false,
                                "RavenDataController.Delete - id string can not be null");
            
            using (var session = this.documentStore.OpenSession())
            {
                var deleteItem = session.Load<T>(id);
                session.Delete<T>(deleteItem);
                session.SaveChanges();
            }
        }

        public void Save<T>(T saveItem)
        {
            using (var session = this.documentStore.OpenSession())
            {
                session.Store(saveItem);
                session.SaveChanges();
            }
        }

        public List<T> GetAll<T>()
        {
            using (var session = this.documentStore.OpenSession())
            {
                var items = session.Query<T>()
                                    .Customize(x => x.WaitForNonStaleResults())
                                    .ToList();
                return items;
            }
        }

        public List<T> GetAll<T>(string indexName)
        {
            Enforce.That(string.IsNullOrEmpty(indexName) == false,
                                "RavenDataController.GetAllWhere - indexName string can not be null");
            
            using (var session = this.documentStore.OpenSession())
            {
                var items = session.Query<T>(indexName)
                                    .Customize(x => x.WaitForNonStaleResults())
                                    .ToList();
                return items;
            }
        }

        public List<T> GetAllWhere<T>(string predicateString, string indexName)
        {
            Enforce.That(string.IsNullOrEmpty(predicateString) == false, 
                                "RavenDataController.GetAllWhere - predicateString can not be null");

            Enforce.That(string.IsNullOrEmpty(indexName) == false,
                                "RavenDataController.GetAllWhere - indexName string can not be null");
            
            using (var session = this.documentStore.OpenSession())
            {
                var predicate = new PredicateConstructor<T>();
                if(string.IsNullOrEmpty(indexName))
                {
                    return GetAllWhere(predicate.CompileToExpression(predicateString));
                }
                else
                {
                    return GetAllWhere(predicate.CompileToExpression(predicateString), indexName);
                }
            }
        }

        public List<T> GetAllWhere<T>(System.Linq.Expressions.Expression<Func<T, bool>> predicate, string indexName)
        {
            Enforce.That(string.IsNullOrEmpty(indexName) == false,
                                "RavenDataController.GetAllWhere - indexName string can not be null");

            using (var session = this.documentStore.OpenSession())
            {
                var items = session.Query<T>(indexName)
                                    .Customize(x => x.WaitForNonStaleResults())
                                    .Where<T>(predicate)
                                    .ToList();
                return items;
            }
        }

        public List<T> GetAllWhere<T>(System.Linq.Expressions.Expression<Func<T, bool>> predicate)
        {
            using (var session = this.documentStore.OpenSession())
            {
                var items = session.Query<T>()
                                    .Customize(x => x.WaitForNonStaleResults())
                                    .Where<T>(predicate)
                                    .ToList();
                return items;
            }
        }

        public void TruncateDocuments<T>()
        {
            using (var session = this.documentStore.OpenSession())
            {
                var documents = session.Query<T>().ToList();
                documents.ForEach(doc => session.Delete<T>(doc));
                session.SaveChanges();
            }
        }

        public RavenDataController SecureForOperation(string userId, string operation)
        {
            this.UserId = userId;
            this.Operation = operation;

            return SecureForOperation();
        }

        public RavenDataController SecureForOperation()
        {
            using (var session = this.documentStore.OpenSession())
            {
                session.SecureFor(this.UserId, this.Operation);
                session.SaveChanges();
                return this;
            }
        }

        #endregion
    }
}

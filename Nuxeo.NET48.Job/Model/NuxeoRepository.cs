using System;
using System.Linq;

namespace Nuxeo.NET48.Job.Model
{
    internal class NuxeoRepository
    {
        private NuxeoEntities _db;

        private NuxeoEntities Db => _db ?? (_db = new NuxeoEntities());
        
        private IndexerState AddIndexerState(IndexerState indexerState) => Db.IndexerState.Add(indexerState);

        private IndexerState FindIndexerState(string key) => Db.IndexerState.FirstOrDefault(i => i.Key == key);

        private Order AddOrder(Order order) => Db.Order.Add(order);

        private Order FindOrder(Guid id) => Db.Order.Find(id);

        public int SaveChanges() => _db.SaveChanges();

        public IndexerState GetOrCreateIndexerState(bool reverse)
        {
            var key = reverse ? "reverse" : "forward";
            return FindIndexerState(key) ??
                   AddIndexerState(new IndexerState(key, DateTime.Now));
        }

        public void UpdateAndSaveIndexerState(IndexerState state, DateTime lastIndexed)
        {
            state.LastIndexed = lastIndexed;
            SaveChanges();
        }

        public Order FindOrAddOrder(Order order, Action<Order> added, Action<Order> updated)
        {
            var found = FindOrder(order.OrderId);
            if (found == null)
            {
                found = AddOrder(order);
                added(found);
            } else
                updated(found);

            return found;
        }
    }
}
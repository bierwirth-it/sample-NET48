using System.Data.Entity.Infrastructure;
using System.Data.Entity;

namespace Nuxeo.NET48.Job.Model
{
    public class NuxeoEntities : DbContext
    {
        public NuxeoEntities()
            : base("name=NuxeoEntities")
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

        }

        public virtual DbSet<Order> Order { get; set; }
        public virtual DbSet<IndexerState> IndexerState { get; set; }
    }
}

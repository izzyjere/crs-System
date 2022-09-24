using CRS.Shared;

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRS.Web.Core
{
    public class DatabaseContext  : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options):base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Citizen>(e =>
            {
                e.OwnsOne(e => e.PersonalDetails, p =>
                {
                    p.ToTable("CitizenDetails");
                    p.Property(p => p.Id);
                    p.WithOwner();
                });
            });
            
        }
    }
}

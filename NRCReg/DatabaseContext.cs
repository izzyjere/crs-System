using CRS.Shared;

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NRCReg
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {

        }
        public virtual DbSet<Citizen> Citizens { get; set; }
        public virtual DbSet<NRCNumber> NRCNumbers { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Citizen>(e =>
            {
                e.OwnsOne(e => e.PersonalDetails, p =>
                {
                    p.ToTable("CitizenDetails");
                    p.Property(p => p.CitizenId);
                    p.OwnsOne(p => p.ThumbPrintData, e =>
                    {
                        e.ToTable("ThumbPrintData");
                        e.Property(e => e.OwnerId);
                        e.WithOwner();
                    });
                    p.WithOwner();
                });
            });
        }
    }
}

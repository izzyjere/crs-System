using CRS.Shared;

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRS.Web
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {

        }
        public virtual DbSet<Suspect> Suspects { get; set; }
        public virtual DbSet<Case> Cases { get; set; }
        public virtual DbSet<Judgement> Judgements { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Suspect>(e =>
            {
                e.OwnsMany(e => e.Biometrics, p =>
                {
                    p.ToTable("SuspectBiometrics");
                    p.Property(b => b.OwnerId);
                    p.WithOwner();
                });
            });
        }
        
    }
}

using Entities.Model;
using Microsoft.EntityFrameworkCore;
namespace DataAccess
{
    public class CoreApidbContext : DbContext
    {
        public CoreApidbContext(DbContextOptions<CoreApidbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GeoPoint>().Property(a => a.Latitude).HasPrecision(18, 9);
            modelBuilder.Entity<GeoPoint>().Property(a => a.Longitude).HasPrecision(18, 9);
        }
        public DbSet<Image> Image { get; set; }
        public DbSet<GeoPoint> GeoPoint { get; set; }
        public DbSet<Quest> Quest { get; set; }
        public DbSet<LandUse> LandUse { get; set; }
        public DbSet<LandKind> LandKind { get; set; }
        public DbSet<LandCoverDistance> LandCoverDistance { get; set; }
        public DbSet<Direction> Direction { get; set; }
    }
}

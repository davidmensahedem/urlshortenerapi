using UrlShortnerApi.Options;

namespace UrlShortnerApi.Storage
{
    public class ApplicationDbContext:DbContext
    {
        public DbSet<UrlShortner> UrlShortners { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> o) : base(o) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<UrlShortner>(builder =>
            {
                builder.Property(u => u.Code)
                .HasMaxLength(CoreConstants.ShortUrlCodeLength);

                builder.HasIndex(u => u.Code).IsUnique();
            });
        }
    }
}

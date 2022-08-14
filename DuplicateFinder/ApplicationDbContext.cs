using Microsoft.EntityFrameworkCore;

namespace DuplicateFinder
{
    public sealed class ApplicationDbContext : DbContext
    {
        private static ApplicationDbContext _instance;

        public static ApplicationDbContext GetInstance()
        {
            return _instance ??= new ApplicationDbContext();
        }
        
        private ApplicationDbContext()
        {
            Database.EnsureCreated();
        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string configFolder = Path.Combine(new string[] {
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                ".config",
                "com.ford-rt",
                "DuplicateFinder"
            });

            Directory.CreateDirectory(configFolder);
            
            string dbPath = Path.Combine(new string[] {
                configFolder,
                "db.sqlite"
            });
            
            optionsBuilder.UseSqlite($"Filename={dbPath}");
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Models.FileRecord>().Property(file => file.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Models.FileRecord>().HasIndex(file => file.Path).IsUnique();
            
            modelBuilder.Entity<Models.Duplicate>().Property(duplicate => duplicate.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Models.Duplicate>().Property(duplicate => duplicate.Processed).HasDefaultValue(false);
        }
        
        public DbSet<Models.FileRecord> FileRegistry { get; set; }
        public DbSet<Models.Duplicate> Duplicates { get; set; }
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MyNotesConsoleApp.Data.Entities;

namespace MyNotesConsoleApp.Data
{
    public class NoteAppDbContext : DbContext
    {
        public DbSet<Note> Notes { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<NoteTag> NoteTags { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            IConfigurationBuilder configBuilder = new ConfigurationBuilder().AddJsonFile("appSettings.json");
            IConfiguration configuration = configBuilder.Build();

            optionsBuilder.UseSqlServer(configuration.GetConnectionString("NoteAppConnectionString"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NoteTag>()
                .HasKey(nt => new {nt.NoteId, nt.TagId});
        }
    }
}
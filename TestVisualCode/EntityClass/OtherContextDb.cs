using System;
using Microsoft.EntityFrameworkCore;

namespace TestVisualCode;

internal class OtherContextDb : DbContext
{
    public DbSet<Track> Tracks { get; set; } = null!;
    public OtherContextDb()
    {
        Database.EnsureCreated();
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite($"Data Source={Path.Combine(ToolsEf.PathDirDestination!, ToolsEf.nameDb)}");
    }
     
   protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Track>().Ignore(u=>u.PathSours);
    }
}

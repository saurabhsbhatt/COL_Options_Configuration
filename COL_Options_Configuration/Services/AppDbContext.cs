using System;
using COL_Options_Configuration.Model;
using Microsoft.EntityFrameworkCore;

namespace COL_Options_Configuration.Services;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {        
    }

    public DbSet<Company> Companies { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Company>(entity =>
        {
           entity.HasKey(e=>e.CompanyId);
           entity.Property(e=>e.CompanyName); 
        });
    }
}
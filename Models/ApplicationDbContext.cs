using Microsoft.EntityFrameworkCore;

namespace Expense_Tracker.Models;

public class ApplicationDbContext : DbContext 
{
    public ApplicationDbContext(DbContextOptions options):base(options)
    {
        
    }

    public DbSet<Transcation> Transcations { get; set; }
    public DbSet<Category> Categories { get; set; }

}
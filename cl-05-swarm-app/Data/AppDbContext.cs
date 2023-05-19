using cl_05_swarm_app.Models;
using Microsoft.EntityFrameworkCore;

namespace cl_05_swarm_app.Data;

public class AppDbContext : DbContext
{
    private readonly IConfiguration _configuration;
    
    public AppDbContext(DbContextOptions<AppDbContext> options, IConfiguration configuration) : base(options)
    {
        _configuration = configuration;
    }
    
    public DbSet<Person>? Persons { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(_configuration.GetConnectionString("SqlServer"));
    }
}
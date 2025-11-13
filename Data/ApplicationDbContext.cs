using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Fall2025_Project3._1_Loswald.Models;

namespace Fall2025_Project3._1_Loswald.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

public DbSet<Movie> Movie { get; set; } = default!;

public DbSet<Actor> Actor { get; set; } = default!;
}

using Microsoft.EntityFrameworkCore;
using ProductMVC.Models;

namespace ProductMVC.Contexts;

public class ProniaDbContext:DbContext
{
    public ProniaDbContext(DbContextOptions options):base(options)
    {
    }
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<ProductTag> ProductTags { get; set; }


}

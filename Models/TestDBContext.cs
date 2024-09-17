using Microsoft.EntityFrameworkCore;

namespace Zadanie_5.Models {
  public class TestDBContext : DbContext {
    public DbSet<Klient> Klient { get; set; }
    public TestDBContext(DbContextOptions<TestDBContext> options) : base(options) {}
  }
}
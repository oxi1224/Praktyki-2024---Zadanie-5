using Microsoft.EntityFrameworkCore;

namespace Zadanie_5.Models {
  public class TestDBContext : DbContext {
    public DbSet<Klienci> Klienci { get; set; }
    public TestDBContext(DbContextOptions<TestDBContext> options) : base(options) {}
  }
}
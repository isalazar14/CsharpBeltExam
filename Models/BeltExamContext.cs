using Microsoft.EntityFrameworkCore;

namespace BeltExam.Models
{
  public class BeltExamContext : DbContext
  {
    public BeltExamContext(DbContextOptions options) : base(options) {}

    public DbSet<User> Users {get; set;}

    public DbSet<Thing> Things {get; set;}

    public DbSet<ThingGuest> ThingGuests {get; set;}
  }
}
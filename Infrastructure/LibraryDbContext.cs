using Core.BaseEntities;
using Core.Books;
using Core.Loans;
using Core.Notifications;
using Core.Users;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure;

public class LibraryDbContext : DbContext
{
    public DbSet<Book> Books { get; set; }
    public DbSet<Author> Authors { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<BookCopy> Copies { get; set; }
    
    
    public DbSet<Admin> Admins { get; set; }
    public DbSet<Librarian> Librarians { get; set; }
    public DbSet<Member> Members { get; set; }
    public DbSet<UserPreferences> UserPreferences { get; set; }
    public DbSet<UserInteraction> UserInteractions { get; set; }
    
    
    public DbSet<Loan> Loans { get; set; }
    public DbSet<Fine> Fines { get; set; }
    public DbSet<Reservation> Reservations { get; set; }
    
    
    public DbSet<Notification> Notifications { get; set; }
    
    public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);
      
      modelBuilder.ApplyConfigurationsFromAssembly(typeof(LibraryDbContext).Assembly);
    }
    
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<AuditEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
using Microsoft.EntityFrameworkCore;
using SimpleTodo.Entities;

namespace SimpleTodo.Persistence;

public sealed class TodoDbContext: DbContext
{
    public DbSet<Todo> Todos { get; set; }
    
    public TodoDbContext(DbContextOptions<TodoDbContext> options) : base(options) {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("todo");
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}

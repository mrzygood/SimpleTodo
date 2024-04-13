using Microsoft.EntityFrameworkCore;
using SimpleTodo.Entities;

namespace SimpleTodo.Persistence;

public interface ITodoRepository
{
    Task Add(Todo todo);
    Task Update(Todo todo);
    Task<Todo?> Get(Guid id, CancellationToken ct = default);
    Task<ICollection<Todo>> Get(
        int page,
        int rowsPerPage,
        CancellationToken ct = default);
}

public class TodoRepository : ITodoRepository
{
    private readonly TodoDbContext _context;

    public TodoRepository(TodoDbContext context)
    {
        _context = context;
    }

    public async Task Add(Todo todo)
    {
        _context.Todos.Add(todo);
        await _context.SaveChangesAsync();
    }

    public async Task Update(Todo todo)
    {
        _context.Set<Todo>().Update(todo);
        await _context.SaveChangesAsync();
    }

    public async Task<Todo?> Get(Guid id, CancellationToken ct = default)
    {
        return await _context.Todos.FirstOrDefaultAsync(x => x.Id == id, cancellationToken: ct);
    }

    public async Task<ICollection<Todo>> Get(
        int page,
        int rowsPerPage,
        CancellationToken ct = default)
    {
        return await _context
            .Todos
            .OrderByDescending(x => x.CreatedAt)
            .Skip(page * rowsPerPage)
            .Take(rowsPerPage)
            .ToListAsync(cancellationToken: ct);
    }
}

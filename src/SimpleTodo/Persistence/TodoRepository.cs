using Microsoft.EntityFrameworkCore;
using SimpleTodo.Entities;

namespace SimpleTodo.Persistence;

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
        return await _context
            .Todos
            .FirstOrDefaultAsync(x => x.Id == id && x.RemovedAt == null, ct);
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
            .Where(x => x.RemovedAt == null)
            .ToListAsync(cancellationToken: ct);
    }
}

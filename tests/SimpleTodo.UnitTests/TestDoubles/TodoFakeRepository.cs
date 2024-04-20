using SimpleTodo.Entities;
using SimpleTodo.Persistence;

namespace SimpleTodo.UnitTests.TestDoubles;

public sealed class TodoFakeRepository : ITodoRepository
{
    private readonly ICollection<Todo> _dataStore = new List<Todo>();
    
    public Task Add(Todo todo)
    {
        _dataStore.Add(todo);
        return Task.CompletedTask;
    }

    public Task Update(Todo todo)
    {
        var existingTodo = _dataStore.FirstOrDefault(t => t.Id == todo.Id);
        if (existingTodo is not null)
        {
            _dataStore.Remove(existingTodo);
            _dataStore.Add(todo);
            return Task.CompletedTask;
        }

        throw new Exception("Todo not found");
    }

    public Task<Todo?> Get(Guid id, CancellationToken ct = default)
    {
        var result = _dataStore
            .Where(x => x.RemovedAt is null)
            .FirstOrDefault(t => t.Id == id);
        return Task.FromResult(result);
    }

    public Task<ICollection<Todo>> Get(int page, int rowsPerPage, CancellationToken ct = default)
    {
        var results = _dataStore
            .Skip(page * rowsPerPage)
            .Take(rowsPerPage)
            .Where(x => x.RemovedAt is null)
            .ToList();
        return Task.FromResult((ICollection<Todo>)results);
    }
}

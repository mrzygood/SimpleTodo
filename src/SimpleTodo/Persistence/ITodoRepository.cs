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

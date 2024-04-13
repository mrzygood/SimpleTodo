using SimpleTodo.Entities;
using SimpleTodo.Exceptions;
using SimpleTodo.Persistence;

namespace SimpleTodo.Services;

public interface ITodoService
{
    Task<Guid> Add(CreateTodoDto dto);
    Task Update(UpdateTodoDto dto);
    Task Finish(Guid id);
    Task Remove(Guid id);
    Task<TodoDto> Get(Guid id, CancellationToken ct = default);
    Task<ICollection<TodoDto>> Get(int limit, CancellationToken ct = default);
}

public sealed class TodoService : ITodoService
{
    private readonly ITodoRepository _repository;
    private readonly ILogger<TodoService> _logger;

    public TodoService(ITodoRepository repository, ILogger<TodoService> logger)
    {
        _repository = repository;
        _logger = logger;
    }
    
    public async Task<Guid> Add(CreateTodoDto dto)
    {
        var entity = new Todo
        {
            Id = Guid.NewGuid(),
            Title = dto.Title,
            Description = dto.Description,
            CreatedAt = DateTime.UtcNow
        };
        await _repository.Add(entity);
        _logger.LogInformation("Todo {Id} with title {Title} added", entity.Id, entity.Title);

        return entity.Id;
    }

    public async Task Update(UpdateTodoDto dto)
    {
        var entity = await _repository.Get(dto.Id);
        if (entity!.DoneAt is not null)
        {
            throw new CannotModifyFinishedTodoException(entity.Id);
        }
        
        entity.Title = dto.Title;
        entity.Description = dto.Description;
        await _repository.Update(entity);
        _logger.LogInformation("Todo {Id} title updated to {Title}", entity.Id, entity.Title);
    }

    public async Task Finish(Guid id)
    {
        var entity = await _repository.Get(id);
        entity!.DoneAt = DateTime.UtcNow;
        await _repository.Update(entity);
        _logger.LogInformation("Todo {Id} finished", entity.Id);
    }

    public async Task Remove(Guid id)
    {
        var entity = await _repository.Get(id, CancellationToken.None);
        if (entity!.RemovedAt is not null)
        {
            _logger.LogInformation("Todo {Id} already removed", entity.Id);
            return;
        }
        
        entity.RemovedAt = DateTime.UtcNow;
        await _repository.Update(entity);
        _logger.LogInformation("Todo {Id} removed", entity.Id);
    }

    public async Task<TodoDto> Get(Guid id, CancellationToken ct = default)
    {
        var entity = await _repository.Get(id, ct);
        return new TodoDto
        {
            Id = entity!.Id,
            Title = entity.Title,
            Description = entity.Description,
            DoneAt = entity.DoneAt,
            CreatedAt = entity.CreatedAt
        };
    }

    public async Task<ICollection<TodoDto>> Get(int limit, CancellationToken ct = default)
    {
        var entities = await _repository.Get(0, limit, ct);
        return entities
            .Select(x => new TodoDto
            {
                Id = x.Id,
                Title = x.Title,
                Description = x.Description,
                DoneAt = x.DoneAt,
                CreatedAt = x.CreatedAt
            })
            .ToList();
    }
}

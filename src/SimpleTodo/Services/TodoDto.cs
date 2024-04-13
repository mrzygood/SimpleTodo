namespace SimpleTodo.Services;

public sealed class CreateTodoDto
{
    public string Title { get; set; }
    public string Description { get; set; }
}

public sealed class UpdateTodoDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
}

public sealed class TodoDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime? DoneAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

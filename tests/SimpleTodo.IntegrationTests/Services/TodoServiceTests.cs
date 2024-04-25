using Shouldly;
using SimpleTodo.Services;

namespace SimpleTodo.IntegrationTests.Services;

public sealed class TodoServiceTests : BaseIntegrationTest, IClassFixture<SimpleTodoAppFactory>, IAsyncLifetime
{
    private readonly SimpleTodoAppFactory _simpleTodoAppFactory;

    public TodoServiceTests(SimpleTodoAppFactory simpleTodoAppFactory) : base(simpleTodoAppFactory)
    {
        _simpleTodoAppFactory = simpleTodoAppFactory;
    }
    
    [Fact]
    public async Task ShouldSaveNewTodo()
    {
        // Given
        var sut = GetDependency<ITodoService>();

        // When
        var addedTodo = await CreateTodo(sut);
            
        // Then
        var savedTodo = await sut.Get(addedTodo.Id);
        savedTodo!.Title.ShouldBe(addedTodo.Title);
        savedTodo.Description.ShouldBe(addedTodo.Description);
    }
    
    [Fact]
    public async Task ShouldUpdateTodo()
    {
        // Given
        var sut = GetDependency<ITodoService>();
        var addedTodo = await CreateTodo(sut);
        var todoToUpdate = new UpdateTodoDto
        {
            Id = addedTodo.Id,
            Title = "Test title 2",
            Description = "Test description 2"
        };
    
        // When
        await sut.Update(todoToUpdate);
        
        // Then
        var updatedTodo = await sut.Get(addedTodo.Id);
        updatedTodo!.Title.ShouldBe(todoToUpdate.Title);
        updatedTodo.Description.ShouldBe(todoToUpdate.Description);
    }

    private async Task<TodoDto> CreateTodo(ITodoService service)
    {
        var todoToCreate = new CreateTodoDto
        {
            Title = "Test title 1",
            Description = "Test description 1"
        };
        var id = await service.Add(todoToCreate);
        var todo = await service.Get(id);

        return todo!;
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync() => await _simpleTodoAppFactory.ResetDatabase();
}

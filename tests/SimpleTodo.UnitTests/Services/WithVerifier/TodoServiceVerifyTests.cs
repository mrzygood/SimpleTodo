using Microsoft.Extensions.Logging;
using NSubstitute;
using SimpleTodo.Services;
using SimpleTodo.UnitTests.TestDoubles;

namespace SimpleTodo.UnitTests.Services.WithVerifier;

public class TodoServiceVerifyTests
{
    private readonly ITodoService _sut = new TodoService(
        new TodoFakeRepository(),
        Substitute.For<ILogger<TodoService>>());

    [Fact]
    public async Task ShouldSaveNewTodo()
    {
        // Given & When
        var todo = new CreateTodoDto
        {
            Title = "Test title 1",
            Description = "Test description 1"
        };

        // When
        var id = await _sut.Add(todo);
            
        // Then
        var savedTodo = await _sut.Get(id);
        await Verify(savedTodo);
    }
    
    [Fact]
    public async Task ShouldUpdateTodo()
    {
        // Given
        var createdTodo = await CreateTodo();
        var todoToUpdate = new UpdateTodoDto
        {
            Id = createdTodo.Id,
            Title = "Test title 2",
            Description = "Test description 2"
        };

        // When
        await _sut.Update(todoToUpdate);
        
        // Then
        var updatedTodo = await _sut.Get(createdTodo.Id);
        await Verify(updatedTodo);
    }

    private async Task<TodoDto> CreateTodo()
    {
        var todoToCreate = new CreateTodoDto
        {
            Title = "Test title 3",
            Description = "Test description 1"
        };
        var id = await _sut.Add(todoToCreate);
        var todo = await _sut.Get(id);

        return todo!;
    }
}

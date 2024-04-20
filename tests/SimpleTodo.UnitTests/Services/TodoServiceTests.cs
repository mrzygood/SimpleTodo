using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;
using SimpleTodo.Services;
using SimpleTodo.UnitTests.TestDoubles;

namespace SimpleTodo.UnitTests.Services;

public class TodoServiceTests
{
    private readonly ITodoService _sut = new TodoService(
        new TodoFakeRepository(),
        Substitute.For<ILogger<TodoService>>());

    [Fact]
    public async Task ShouldSaveNewTodo()
    {
        // Given
        var todo = new CreateTodoDto
        {
            Title = "Test title 1",
            Description = "Test description 1"
        };

        // When
        var id = await _sut.Add(todo);
            
        // Then
        var savedTodo = await _sut.Get(id);
        savedTodo!.Title.ShouldBe(todo.Title);
        savedTodo.Description.ShouldBe(todo.Description);
    }
    
    [Fact]
    public async Task ShouldUpdateTodo()
    {
        // Given
        var todoToCreate = new CreateTodoDto
        {
            Title = "Test title 1",
            Description = "Test description 1"
        };
        var id = await _sut.Add(todoToCreate);
        var todoToUpdate = new UpdateTodoDto
        {
            Id = id,
            Title = "Test title 2",
            Description = "Test description 2"
        };

        // When
        await _sut.Update(todoToUpdate);
        
        // Then
        var updatedTodo = await _sut.Get(id);
        updatedTodo!.Title.ShouldBe(todoToUpdate.Title);
        updatedTodo.Description.ShouldBe(todoToUpdate.Description);
    }
    
    [Fact]
    public async Task ShouldFinishTodo()
    {
        // Given
        var todo = await CreateTodo();

        // When
        await _sut.Finish(todo.Id);
        
        // Then
        var finishedTodo = await _sut.Get(todo.Id);
        finishedTodo!.DoneAt.ShouldNotBeNull();
    }
    
    [Fact]
    public async Task ShouldRemoveTodo()
    {
        // Given
        var todo = await CreateTodo();

        // When
        await _sut.Remove(todo.Id);
        
        // Then
        var removedTodo = await _sut.Get(todo.Id);
        removedTodo.ShouldBeNull();
    }

    private async Task<TodoDto> CreateTodo()
    {
        var todoToCreate = new CreateTodoDto
        {
            Title = "Test title 1",
            Description = "Test description 1"
        };
        var id = await _sut.Add(todoToCreate);
        var todo = await _sut.Get(id);

        return todo!;
    }
}

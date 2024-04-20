using Shouldly;
using SimpleTodo.Services;

namespace SimpleTodo.IntegrationTests.Services;

public sealed class TodoServiceTests : BaseIntegrationTest
{
    public TodoServiceTests(SimpleTodoAppFactory assetsWalletAppFactory)
        : base(assetsWalletAppFactory)
    {
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
}

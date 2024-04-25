using Microsoft.Extensions.DependencyInjection;

namespace SimpleTodo.IntegrationTests;

public abstract class BaseIntegrationTest(SimpleTodoAppFactory simpleTodoAppFactory)
{
    private readonly IServiceScope _serviceScope = simpleTodoAppFactory.Services.CreateScope();

    protected T GetDependency<T>() where T : class
    {
        return _serviceScope.ServiceProvider.GetRequiredService<T>();
    }
}

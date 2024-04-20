using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SimpleTodo.Persistence;
using Testcontainers.PostgreSql;

namespace SimpleTodo.IntegrationTests;

public sealed class SimpleTodoAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres")
        .Build();
        
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            var dbContextDescriptor = services
                .Single(x => x.ServiceType == typeof(DbContextOptions<TodoDbContext>));

            services.Remove(dbContextDescriptor);
            services.AddDbContext<TodoDbContext>(c =>
            {
                c.UseNpgsql(_dbContainer.GetConnectionString());
            });
        });
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
    }
}

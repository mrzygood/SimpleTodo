using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using SimpleTodo.Persistence;
using SimpleTodo.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<TodoDbContext>(options => {
    options.UseNpgsql(builder.Configuration.GetConnectionString("TodoConnection"));
});

builder.Services.AddScoped<ITodoRepository, TodoRepository>();
builder.Services.AddScoped<ITodoService, TodoService>();

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

const string CorsPolicyName = "TodoDefault"; 
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: CorsPolicyName,
        policyBuilder =>
        {
            policyBuilder
                .WithOrigins("http://localhost:8080")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

builder.Services.AddKeycloakWebApiAuthentication(builder.Configuration);
builder.Services
    .AddAuthorization();

builder.Services
    .AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/todo/{id}", ([FromServices] ITodoService todoService, Guid id) => todoService.Get(id))
    .WithName("GetTodo")
    .WithOpenApi();

app.MapGet("/todo", ([FromServices] ITodoService todoService, int limit) => todoService.Get(limit))
    .WithName("GetTodoList")
    .WithOpenApi()
    .RequireAuthorization();

app.MapPost("/todo", async ([FromServices] ITodoService todoService, CreateTodoDto todo) =>
    {
        var id = await todoService.Add(todo);
        return id;
    })
    .WithName("AddTodo")
    .WithOpenApi();

app.MapPatch("/todo", async ([FromServices] ITodoService todoService, UpdateTodoDto todo)
        => await todoService.Update(todo))
    .WithName("UpdateTodo")
    .WithOpenApi();

app.MapPost("/todo/{id}/finish", async ([FromServices] ITodoService todoService, Guid id)
        => await todoService.Finish(id))
    .WithName("FinishTodo")
    .WithOpenApi();

app.MapDelete("/todo/{id}", async ([FromServices] ITodoService todoService, Guid id)
        => await todoService.Remove(id))
    .WithName("RemoveTodo")
    .WithOpenApi();

using (var scope = app.Services.CreateScope())
{
    var administrationDbContext = scope.ServiceProvider
        .GetRequiredService<TodoDbContext>();
    await administrationDbContext.Database.MigrateAsync();
}

app.UseCors(CorsPolicyName);

app.Run();

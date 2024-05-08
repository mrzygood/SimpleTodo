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
    // .AddKeycloakAuthorization(builder.Configuration);

// builder.Services
//     .AddAuthentication()
//     .AddJwtBearer(x =>
//     {
//         x.Authority = "http://localhost:8085/realms/SimpleTodo";
//         x.RequireHttpsMetadata = false;
//         x.MetadataAddress = "http://localhost:8085/realms/SimpleTodo/.well-known/openid-configuration";
//         // x.TokenValidationParameters = new TokenValidationParameters
//         // {
//         //     ValidAudience = "simple-todo-ui"
//         // };
//         x.SaveToken = true;
//         x.TokenValidationParameters = new TokenValidationParameters
//         {
//             ValidateIssuer = false,
//             // NOTE: Usually you don't need to set the issuer since the middleware will extract it 
//             // from the .well-known endpoint provided above. but since I am using the container name in
//             // the above URL which is not what is published issueer by the well-known, I'm setting it here.
//             // ValidIssuer = "http://localhost:8080/auth/realms/AuthDemoRealm", 
//             // ValidAudience = "auth-demo-web-api",
//             // ValidateAudience = true,
//             // ValidateLifetime = true,
//             // ValidateIssuerSigningKey = true,
//             // ClockSkew = TimeSpan.FromMinutes(1)
//         };
//     });
//
// builder.Services
//     .AddAuthorization(o =>
//     {
//         o.DefaultPolicy = new AuthorizationPolicyBuilder()
//             .RequireAuthenticatedUser()
//             // .RequireClaim("email_verified", "true")
//             .Build();
//     });

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

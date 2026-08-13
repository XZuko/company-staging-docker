using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var connection = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("ConnectionStrings:Default is required.");

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connection));
builder.Services.AddHealthChecks().AddDbContextCheck<AppDbContext>();
builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
    policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();
app.UseCors();
app.MapHealthChecks("/health");

await ApplyDatabaseAsync(app.Services);

app.MapGet("/todos", async (AppDbContext db) => await db.Todos.AsNoTracking().ToListAsync());
app.MapPost("/todos", async (Todo todo, AppDbContext db) =>
{
    db.Todos.Add(todo);
    await db.SaveChangesAsync();
    return Results.Created($"/todos/{todo.Id}", todo);
});

app.Run();

static async Task ApplyDatabaseAsync(IServiceProvider services)
{
    using var scope = services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.EnsureCreatedAsync();
    if (!await db.Todos.AnyAsync())
    {
        db.Todos.Add(new Todo { Title = "Staging environment is running" });
        await db.SaveChangesAsync();
    }
}

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Todo> Todos => Set<Todo>();
}

public sealed class Todo
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public bool Complete { get; set; }
}


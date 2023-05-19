using cl_05_swarm_app.Data;
using cl_05_swarm_app.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDbContext<AppDbContext>(options => 
        options.UseInMemoryDatabase("Development"));
}

if (builder.Environment.IsProduction())
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer")));
}

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty;
});

using var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
var appDbContext = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();

app.MapGet("/", () => Console.WriteLine(app.Configuration.GetConnectionString("SqlServer")));

app.MapGet("/list", async () => await appDbContext.Persons?.ToListAsync());

app.MapGet("/{id}", async (int id) => 
    await appDbContext.Persons.FirstOrDefaultAsync(p =>
    p.Id == id));

app.MapPost("/post/{name}", async (string name) =>
{
    var createdPerson = await appDbContext.Persons.AddAsync(new Person { Name = name });
    await appDbContext.SaveChangesAsync();

    return Results.Created($"/{createdPerson.Entity.Id}", createdPerson.Entity);
});

app.MapPut("/put/{id}/{newName}", async (int id, string newName) =>
{
    var personToUpdate = await appDbContext.Persons.FirstOrDefaultAsync(p => 
        p.Id == id);

    if (personToUpdate is null) return Results.NotFound();

    personToUpdate.Name = newName;

    await appDbContext.SaveChangesAsync();
    return Results.Ok(personToUpdate);
});

app.MapDelete("/delete/{id}", async (int id) =>
{
    var person = await appDbContext.Persons.FirstOrDefaultAsync(p => p.Id == id);

    if (person is null) return Results.NotFound();

    appDbContext.Persons.Remove(person);
    await appDbContext.SaveChangesAsync();

    return Results.NoContent();
});

app.Run();
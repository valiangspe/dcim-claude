using backend.Data;
using backend.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=app.db"
    )
);

var app = builder.Build();

// Auto-migrate on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// CRUD endpoints
var items = app.MapGroup("/api/items");

items.MapGet("/", async (AppDbContext db) => await db.Items.ToListAsync());

items.MapGet(
    "/{id}",
    async (int id, AppDbContext db) =>
        await db.Items.FindAsync(id) is Item item ? Results.Ok(item) : Results.NotFound()
);

items.MapPost(
    "/",
    async (Item item, AppDbContext db) =>
    {
        db.Items.Add(item);
        await db.SaveChangesAsync();
        return Results.Created($"/api/items/{item.Id}", item);
    }
);

items.MapPut(
    "/{id}",
    async (int id, Item input, AppDbContext db) =>
    {
        var item = await db.Items.FindAsync(id);
        if (item is null)
            return Results.NotFound();

        item.Name = input.Name;
        item.Description = input.Description;
        await db.SaveChangesAsync();
        return Results.Ok(item);
    }
);

items.MapDelete(
    "/{id}",
    async (int id, AppDbContext db) =>
    {
        var item = await db.Items.FindAsync(id);
        if (item is null)
            return Results.NotFound();

        db.Items.Remove(item);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }
);

app.Run();

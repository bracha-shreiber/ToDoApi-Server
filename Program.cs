using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using TodoApi;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<ToDoDbContext>(options =>
   options.UseMySql("name=todoapi", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.41-mysql"))
);
    
    
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyOrigin() // Allow any origin
                   .AllowAnyMethod() // Allow any HTTP method
                   .AllowAnyHeader(); // Allow any header
        });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "ToDo API",
    });
});
var app = builder.Build();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Todo API V1");
    c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
});


app.UseSwagger();
app.UseSwaggerUI();
app.MapGet("/items", (ToDoDbContext dbContext) => GetTodos(dbContext));
app.MapPost("/items", (ToDoDbContext dbContext, Item item) => PostTodo(dbContext, item));
app.MapPut("/items/{id}", (ToDoDbContext dbContext, int id, Item item) => PutTodo(dbContext, id, item));
app.MapDelete("/items/{id}", (ToDoDbContext dbContext, int id) => DeleteTodo(dbContext, id));
app.UseCors("AllowAllOrigins");


List<Item> GetTodos(ToDoDbContext dbContext)
{
    return dbContext.items.ToList();
}
Item PostTodo(ToDoDbContext dbContext, Item item)
{
    dbContext.items.Add(item);
    dbContext.SaveChanges();
    return item;
}

Item PutTodo(ToDoDbContext dbContext, int id, Item item)
{
    var existingItem = dbContext.items.Find(id);
    if (existingItem != null)
    {
        existingItem.IsComplete = item.IsComplete;
        dbContext.items.Update(existingItem);
        dbContext.SaveChanges();
    }
    return existingItem;
}

void DeleteTodo(ToDoDbContext dbContext, int id)
{
    var item = dbContext.items.Find(id);
    if (item != null)
    {
        dbContext.items.Remove(item);
        dbContext.SaveChanges();
    }
}
app.MapGet("/",()=>"AuthServer API is running");
app.Run();
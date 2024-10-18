using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using WebApplicationminimalproject.API;
using WebApplicationminimalproject.Filter;
using WebApplicationminimalproject.Model;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<TodoDBContext>(options =>
{
    options.UseInMemoryDatabase("TODOLIST");
});
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    ///Middle wares
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


//Grouping 
var api = app.MapGroup("/api")
    .AddEndpointFilter(async (context, next) =>
    {
        //1
        app.Logger.LogInformation("Before first filter");
        //before ---invocation my method ---API
        //2
        var result = await next(context);
        //6
        //After server the api
        app.Logger.LogInformation("After first filter");
        return result;

    }).AddEndpointFilter<LogEndPointFilter>();

var todoitem = api.MapGroup("/todoItems").WithTags("todoItems").WithOpenApi();



todoitem.MapGet("/", TodoAPI.GetAllTodoItems).Produces(200);



todoitem.MapPost("/", async (Todo model, [FromServices] TodoDBContext dBContext, HttpRequest request) =>
{
    //var id = request.RouteValues["id"];
    //var page = request.Query["page"];
    var customHeader = request.Headers["Content-Type"];

    //4
    await dBContext.Todos.AddAsync(model);
    await dBContext.SaveChangesAsync();

    return model;
}).AddEndpointFilter(async (context, next) =>
{
    //3
    //Retervie the argument
    var todo = context.GetArgument<Todo>(0);

    if (string.IsNullOrEmpty(todo?.Name) || string.IsNullOrWhiteSpace(todo?.Name) || todo is null)
        return Results.Problem("Name is Required");
    if (todo.Name.Length > 5)
        return Results.Problem("Name should be less than 5 characters");
    var result = await next(context);
    //5
    return result;
});

todoitem.MapPut("/{id}", async (int id, Todo inputTodo, TodoDBContext db) =>
{
    var todo = await db.Todos.FindAsync(id);
    if (todo is null) return Results.NotFound();
    todo.Name = inputTodo.Name;
    todo.IsComplete = inputTodo.IsComplete;

    await db.SaveChangesAsync();

    return Results.NoContent();
});


todoitem.MapDelete("/{id}", async (int id, TodoDBContext db) =>
{
    if (await db.Todos.FindAsync(id) is Todo todo)
    {
        db.Todos.Remove(todo);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }

    return Results.NotFound();
});



todoitem.MapGet("/completed", GetCompletedItems).WithTags("todoItems");

async Task<List<Todo>> GetCompletedItems(HttpContext context, TodoDBContext db)
{
    return await db.Todos.Where(x => x.IsComplete).ToListAsync();
}


todoitem.MapGet("/{id}", async (int id, TodoDBContext db) =>
await db.Todos.FindAsync(id) is Todo todo ? Results.Ok(todo) :
Results.NotFound());


app.MapPost("/Test", ([AsParameters] ParamTest model) =>
{



}).WithTags("general");

app.MapGet("/SayHello", ([AsParameters] ParamTest model) =>
{

    return $"Hello {model.Name}";
}).WithTags("general");

//// GET  /tags?q=1&q=2&q=3 
//app.MapGet("/tags", (int[] q) =>
//{
//    return $"tag1: {q[0]} , tag2: {q[1]}, tag3: {q[2]}";
//    }).WithTags("general");

app.MapGet("/tags", (Tag[] q) =>
{
    return $"tag1: {q[0].Name} , tag2: {q[1].Name}";
}).WithTags("general");

app.MapPost("/tags", (Tag[] q) =>
{
    return $"tag1: {q[0].Name} , tag2: {q[1].Name}";
}).WithTags("general");


var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};


api.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast").WithTags("WeatherForecast")
.WithOpenApi();


app.UseEndpoints(edpoint =>
{
    edpoint.MapDefaultControllerRoute();
});




app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}



class ParamTest
{
    public int Id { get; set; }
    public string Name { get; set; }
}


public class Tag
{
    public string? Name { get; set; }
    public static bool TryParse(string? name, out Tag tag)
    {
        if (name is null)
        {
            tag = default!;
            return false;
        }
        tag = JsonConvert.DeserializeObject<Tag>(name);

        //tag = new Tag { Name = name };
        return true;
    }
}

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System.Text;
using Todo.EFCore;
using Todo.Models;
using Todo.Models.Dtos;
using Todo.Repository;
using WebApplicationminimalproject;
using WebApplicationminimalproject.API;
using WebApplicationminimalproject.Filter;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {


    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "JWT Authentication",
        Description = "Enter your JWT token in this field",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });

});
var configuration = builder.Configuration;
builder.Services.AddDbContext<ToDoDBContext>(options =>
{
    ///options.UseInMemoryDatabase("TODOLIST");
    options.UseSqlServer(configuration.GetConnectionString("TodoDB"));
});
builder.Services.AddDatabaseDeveloperPageExceptionFilter();


builder.Services.AddTransient<ITodoRepo, TodoRepo>();

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x => {

    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        IssuerSigningKey= new SymmetricSecurityKey(Encoding.ASCII.GetBytes(CustomUtility.PRIVATEKEY)),
        ValidateIssuer=false,
        ValidateIssuerSigningKey = true,
        ValidateAudience = false,
        ValidateLifetime = true,
    };
});
//builder.Services.AddAuthorization();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy => policy.RequireRole("admin"));
    options.AddPolicy("UserPolicy", policy => policy.RequireRole("user"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
///Middle wares
app.UseSwagger();
app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

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

//[Authorize(S)]


api.MapPost("/Authenticate", Authenticate.GenerateToken);

var todoitem = api.MapGroup("/todoItems").WithTags("todoItems").WithOpenApi();

//[Authorize(Role="Admin")]

todoitem.MapGet("/", TodoAPI.GetAllTodoItems).Produces(200).RequireAuthorization("AdminPolicy,UserPolicy");



todoitem.MapPost("/", async (TodoRequestDto model, [FromServices] ITodoRepo todoRepo, HttpRequest request) =>
{
    //var id = request.RouteValues["id"];
    //var page = request.Query["page"];
    var customHeader = request.Headers["Content-Type"];

    //4
    //await dBContext.Todos.AddAsync(model.ConvertTODO());
    //await dBContext.SaveChangesAsync();
    await todoRepo.Add(model.ConvertTODO());
    return model;
}).AddEndpointFilter(async (context, next) =>
{
    //3
    //Retervie the argument
    var todo = context.GetArgument<TodoRequestDto>(0);

    if (string.IsNullOrEmpty(todo?.Name) || string.IsNullOrWhiteSpace(todo?.Name) || todo is null)
        return Results.Problem("Name is Required");
    if (todo.Name.Length > 5)
        return Results.Problem("Name should be less than 5 characters");
    var result = await next(context);
    //5
    return result;
}).RequireAuthorization("AdminPolicy");

todoitem.MapPut("/{id}", async (int id, TodoRequestDto inputTodo,[FromServices] ITodoRepo todoRepo) =>
{

    var todo = await todoRepo.FindAsNoTracking(id);
    if (todo is null) return Results.NotFound();

    await todoRepo.Update(inputTodo.ConvertTODO());

    return Results.NoContent();
}).RequireAuthorization("AdminPolicy"); ;


todoitem.MapDelete("/{id}", async (int id, [FromServices] ITodoRepo todoRepo) =>
{

    var todo = await todoRepo.FindAsNoTracking(id);
    if (todo is null) return Results.NotFound();

    await todoRepo.Delete(id);

    return Results.NoContent();
    
}).RequireAuthorization("AdminPolicy"); ;



todoitem.MapGet("/completed", GetCompletedItems).WithTags("todoItems").RequireAuthorization("AdminPolicy,UserPolicy");

async Task<IEnumerable<TodoEntity>> GetCompletedItems(HttpContext context, [FromServices] ITodoRepo todoRepo)
{
   return await todoRepo.GetAllBy(x => x.IsComplete);
    //return await db.Todos.Where(x => x.IsComplete).ToListAsync();
}


todoitem.MapGet("/{id}", async (int id, ToDoDBContext db) =>
await db.Todos.FindAsync(id) is TodoEntity todo ? Results.Ok(todo) :
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

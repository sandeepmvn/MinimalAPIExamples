using Microsoft.EntityFrameworkCore;
using WebApplicationminimalproject.Model;

namespace WebApplicationminimalproject.API
{
    public static class TodoAPI
    {
       public static async Task<IResult> GetAllTodoItems(HttpContext context, TodoDBContext dBContext)
        {
            return TypedResults.Ok(await dBContext.Todos.ToListAsync());
        }
    }
}

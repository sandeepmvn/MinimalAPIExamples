using Microsoft.EntityFrameworkCore;
using Todo.EFCore;
using Todo.Models.Dtos.Todo;
using Todo.Repository;

namespace WebApplicationminimalproject.API
{
    public static class TodoAPI
    {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <param name="todoRepo"></param>
    /// <returns></returns>
       public static async Task<IResult> GetAllTodoItems(HttpContext context, ITodoRepo todoRepo)
        {
            var res=await todoRepo.GetAll();

            var claims= context.User.Claims;

            return TypedResults.Ok(res.Select(x=>new TodoResponseDto(x.Id,x.Name,x.IsComplete,x.IsActive)).ToList());
        }
    }
}

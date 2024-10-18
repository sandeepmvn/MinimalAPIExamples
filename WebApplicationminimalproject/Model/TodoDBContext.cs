using Microsoft.EntityFrameworkCore;

namespace WebApplicationminimalproject.Model
{
    public class TodoDBContext : DbContext
    {
        public TodoDBContext(DbContextOptions<TodoDBContext> options)
    : base(options) { }

        public DbSet<Todo> Todos => Set<Todo>();

    }
}

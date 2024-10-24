using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Todo.EFCore;
using Todo.Models;
using Todo.Repository.Shared;

namespace Todo.Repository
{
    public class TodoRepo : GenericRepository<TodoEntity>, ITodoRepo
    {
        public TodoRepo(ToDoDBContext dbcontext, ILogger<TodoRepo> logger) : base(dbcontext, logger)
        {
        }


        public override Task Update(TodoEntity entity)
        {
            return base.Update(entity);
        }
    }
}

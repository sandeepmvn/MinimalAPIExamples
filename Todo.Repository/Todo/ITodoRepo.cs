using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Todo.Models;
using Todo.Repository.Shared;

namespace Todo.Repository
{
    public interface ITodoRepo:IGenericRepository<TodoEntity>
    {
    }
}

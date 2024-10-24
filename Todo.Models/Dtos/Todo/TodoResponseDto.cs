using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Todo.Models.Dtos.Todo
{
    public record TodoResponseDto(int id,string name,bool isComplete,bool isActive);
}

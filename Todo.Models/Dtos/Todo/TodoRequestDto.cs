using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Todo.Models.Dtos
{
    public class TodoRequestDto
    {
        [Required]
        [StringLength(TodoEntity.NameLength)]
        public string Name { get; set; }
        public bool IsComplete { get; set; }

        // use mapper instead of this 
        public TodoEntity ConvertTODO()
        {
            return new TodoEntity { Name = Name, IsComplete = IsComplete,IsActive=true,IsDelete=false };
        }
    }
    
}

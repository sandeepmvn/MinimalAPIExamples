using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Todo.Models
{
    public class TodoEntity: Entity
    {
        #region Lengths
        public const int NameLength = 4;
        #endregion


        [Required]
        [StringLength(NameLength)]
        public string? Name { get; set; }
        public bool IsComplete { get; set; }
    }
}

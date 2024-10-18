using System.ComponentModel.DataAnnotations;

namespace WebApplicationminimalproject.Model
{
    public class Todo
    {
        public int Id { get; set; }
        [Required]
        [StringLength(4)]
        public string? Name { get; set; }
        public bool IsComplete { get; set; }

    }
}

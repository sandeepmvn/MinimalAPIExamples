using WebApplicationminimalproject.Model;

namespace WebApplicationminimalproject.Dtos
{
    public class ToDoDto
    {
        public string Name { get; set; }
        public bool IsComplete { get; set; }

        public Todo ConvertTODO()
        {
            return new Todo { Name = Name, IsComplete = IsComplete };
        }
    }
}

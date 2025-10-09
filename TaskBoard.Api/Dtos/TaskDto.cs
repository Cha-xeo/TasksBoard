using TaskBoard.Api.Models;

namespace TaskBoard.Api.Dtos
{
    public class TaskDto
    {
        public int ID { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public List<int>? UserIDS { get; set; }
    }
}

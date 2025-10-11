using TaskBoard.Api.Models.Asbstracts;

namespace TaskBoard.Api.Models
{
    public class Tasks : BaseEntity
    {
        public int ID { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public List<Users>? User { get; set; }
    }
}

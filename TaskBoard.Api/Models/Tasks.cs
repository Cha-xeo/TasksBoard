namespace TaskBoard.Api.Models
{
    public class Tasks
    {
        public int ID { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public List<int>? UserIDS { get; set; }
        public List<Users>? User { get; set; }
    }
}

namespace TaskBoard.Api.Models
{
    public class Users
    {
        public int ID { get; set; }
        public int? Age { set; get; }
        public string? UserName { get; set; }
        public DateTime? BirthDate { get; set; }
        public List<int>? TasksIDS { get; set; }
        public List<Tasks>? Tasks { get; set; }
    }
}

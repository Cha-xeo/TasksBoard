using TaskBoard.Api.Models.Asbstracts;

namespace TaskBoard.Api.Models
{
    public class ApiClients : BaseEntity
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string ApiKey { get; set; }
        public bool isActive { get; set; }
    }
}

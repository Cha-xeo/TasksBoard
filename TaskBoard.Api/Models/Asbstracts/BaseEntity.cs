using System.ComponentModel.DataAnnotations.Schema;

namespace TaskBoard.Api.Models.Asbstracts
{
    public abstract class BaseEntity
    {
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        [NotMapped]
        public bool IsDeleted => DeletedAt != null;
    }
}

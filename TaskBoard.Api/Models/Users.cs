using System.ComponentModel.DataAnnotations;
using TaskBoard.Api.Models.Asbstracts;

namespace TaskBoard.Api.Models
{
    public class Users : BaseEntity
    {
        public int ID { get; set; }
        public int? Age { set; get; }
        [Required, StringLength(100)]
        public string UserName { get; set; }
        [Required, EmailAddress, StringLength(256)]
        public string Email { get; set; }
        [StringLength(256)]
        public string? DisplayName { get; set; }
        [Required, StringLength(256), DataType(DataType.Password)]
        public string PasswordHash { get; set; }
        public string? PasswordResetToken { get; set; }
        [StringLength(50)]
        public string? FirstName { get; set; }
        [StringLength(50)]
        public string? LastName { get; set; }
        public DateTime? PasswordResetTokenExpiry { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? Role { get; set; }
        public DateTime? LockoutEnd { get; set; }
        public bool? IsActive { get; set; }

        public List<Tasks>? Tasks { get; set; }
    }
}

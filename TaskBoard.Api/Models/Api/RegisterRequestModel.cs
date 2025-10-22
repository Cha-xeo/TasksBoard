namespace TaskBoard.Api.Models.Api
{
    public class RegisterRequestModel
    {
        public string Email { get; set; }
        public string? DisplayName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public DateTime? Birthdate { get; set; }
    }
}

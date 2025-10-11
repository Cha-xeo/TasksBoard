namespace TaskBoard.Api.Configuration
{
    public class JwtOptions
    {
        public string Key { get; set; }
        public List<string> Issuer { get; set; }
        public List<string> Audience { get; set; }
        public int TokenValidityMins { get; set; }
    }
}

namespace TaskBoard.Api.Services.Interface
{
    public interface IApiKeyService
    {
        Task<bool> IsValid(string apiKey);
        Task<string> Create(string clientName);
    }
}

using Microsoft.EntityFrameworkCore;
using TaskBoard.Api.Data;
using TaskBoard.Api.Models;
using TaskBoard.Api.Services.Helper;
using TaskBoard.Api.Services.Interface;

namespace TaskBoard.Api.Services
{
    public class ApiKeyService : IApiKeyService
    {
        private readonly TasksContext _tasksContext;

        public ApiKeyService(TasksContext tasksContext)
        {
            _tasksContext = tasksContext;
        }

        public async Task<string> Create(string clientName)
        {
            string newKey = ApiKeyHelper.GenerateApiKey();
            ApiClients client = new ApiClients
            {
                Name = clientName,
                ApiKey = newKey,
                isActive = true,
                CreatedAt = DateTime.UtcNow,
            };
            _tasksContext.ApiClients.Add(client);
            await _tasksContext.SaveChangesAsync();
            return newKey;
        }

        public async Task<bool> IsValid(string apiKey)
        {
            return await _tasksContext.ApiClients.AnyAsync(c => c.ApiKey == apiKey && c.isActive);
        }
    }
}

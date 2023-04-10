using Shared.Models;

namespace Shared.Grains;

public interface ITodoGrain : IGrainWithGuidKey
{
    Task SetAsync(TodoItem item);
    Task<TodoItem?> GetAsync();
    Task RemoveAsync(Guid itemKey);
    Task ClearAsync(Guid userKey);
    Task<List<TodoItem>> GetAllAsync();
}
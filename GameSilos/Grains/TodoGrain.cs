using Microsoft.Extensions.Logging;
using Orleans.Runtime;
using Shared.Grains;
using Shared.Models;

namespace Silo.Grains;

public class TodoGrain : Grain, ITodoGrain
{
    private readonly ILogger<TodoGrain> _logger;
    private readonly IPersistentState<State> _state;

    private static string GrainType => nameof(TodoGrain);
    private Guid GrainKey => this.GetPrimaryKey();

    public TodoGrain(ILogger<TodoGrain> logger, [PersistentState("State")] IPersistentState<State> state)
    {
        _logger = logger;
        _state = state;
    }

    public Task<TodoItem?> GetAsync() => Task.FromResult(_state.State.Item);

    public async Task SetAsync(TodoItem item)
    {
        _state.State.Item = item;
        await _state.WriteStateAsync();

        _logger.LogInformation(
            "{@GrainType} {@GrainKey} now contains {@Todo}",
            GrainType, GrainKey, item);
    }

    public Task RemoveAsync(Guid itemKey)
    {
        if (_state.State.Item?.Key == itemKey)
        {
            _state.State.Item = null;
            return _state.WriteStateAsync();
        }
        return Task.CompletedTask;
    }

    public async Task ClearAsync(Guid userKey)
    {
        if (_state.State.Item?.OwnerKey == userKey)
        {
            _state.State.Item = null;
            await _state.WriteStateAsync();
        }
    }

    // fetch and return list of all items of the grain
    public Task<List<TodoItem>> GetAllAsync()
    {
        var items = new List<TodoItem>();
        if (_state.State.Item is not null)
        {
            items.Add(_state.State.Item);
        }
        return Task.FromResult(items);
    }

    [GenerateSerializer]
    public class State
    {
        [Id(0)] public TodoItem? Item { get; set; }
    }
}
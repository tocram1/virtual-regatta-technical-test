using Microsoft.AspNetCore.Mvc;
using Shared.Grains;
using Shared.Models;

namespace GameServicesAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class TodoController : Controller
{
    private readonly ILogger<TodoController> _logger;
    private readonly IClusterClient _client;

    public TodoController(ILogger<TodoController> logger, IClusterClient client)
    {
        _logger = logger;
        _client = client;
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(TodoItem item)
    {
        if (!string.IsNullOrWhiteSpace(item.Title))
        {
            // we could check if this item.OwnerKey is authorized to create items
            _logger.LogInformation("Adding {@item}.", item);
            await _client.GetGrain<ITodoGrain>(item.Key).SetAsync(item);

            return Ok(new { id = item.Key });
        }

        return BadRequest();
    }

    [HttpPatch("edit/{key}")]
    public async Task<IActionResult> Edit(Guid key, TodoItem item)
    {
        _logger.LogInformation("Editing {@item}.", item);

        var grain = _client.GetGrain<ITodoGrain>(key);
        var existingItem = await grain.GetAsync();

        if (existingItem is null)
        {
            return NotFound();
        }

        existingItem = existingItem with
        {
            Title = item.Title,
            IsDone = item.IsDone
        };

        await grain.SetAsync(existingItem);

        return Ok(new { id = existingItem.Key });
    }

    [HttpDelete("remove/{key}")]
    public async Task<IActionResult> Remove(Guid key)
    {
        var grain = _client.GetGrain<ITodoGrain>(key);
        var existingItem = await grain.GetAsync();

        if (existingItem is null)
        {
            return NotFound();
        }

        await grain.RemoveAsync(existingItem.Key);

        _logger.LogInformation("Removed {@key}.", key);

        return Ok(new { id = existingItem.Key });
    }

    [HttpPost("clear/{ownerKey}")]
    public async Task<IActionResult> Clear(Guid ownerKey)
    {
        var grain = _client.GetGrain<ITodoGrain>(ownerKey);
        var existingItem = await grain.GetAsync();

        if (existingItem is null)
        {
            return NotFound();
        }

        await grain.ClearAsync(ownerKey);
        return Ok();
    }

    [HttpPost("list/{ownerKey}")]
    public async Task<IActionResult> List(Guid ownerKey)
    {
        var grain = _client.GetGrain<ITodoGrain>(ownerKey);
        var items = await grain.GetAllAsync();

        if (items is null)
        {
            return NotFound();
        }

        return Ok(items);
    }
}
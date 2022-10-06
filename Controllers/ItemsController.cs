using Catalog.Dtos;
using Catalog.Entities;
using Catalog.Repositories;
using Microsoft.AspNetCore.Mvc;
namespace Catalog.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ItemsController : ControllerBase 
{
    private readonly IItemsRepository repository;

    public ItemsController(IItemsRepository repository) {
        this.repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ItemDto>>> GetItemsAsync() 
    {
        var items = (await repository.GetItemsAsync()).AsDto();
        return Ok(items);
    } 
    
    [HttpGet("{id}")]
    public async Task<ActionResult<ItemDto>> GetItemAsync(Guid id) 
    {
        var item = await repository.GetItemAsync(id);
        if (item is null) 
        {
            return NotFound();
        }
        return Ok(item.AsDto());
    }

    [HttpPost] //Return the name of the method as well as the object
    public async Task<ActionResult<ItemDto>> CreateItemAsync(CreatedItemDto itemDto)
    {
        Item item = new () 
        {
            Id = Guid.NewGuid(),
            Name = itemDto.Name,
            Price = itemDto.Price,
            CreatedDate = DateTimeOffset.UtcNow
        };
        await repository.CreatedItemAsync(item);       

        return CreatedAtAction(nameof(GetItemAsync), new{id = item.Id},item.AsDto());
    }

    [HttpPut("{id}")] //No content 
    public async Task<ActionResult> UpdateItemAsync(Guid id,UpdateItemDto itemDto) 
    {
        var existingItem = await repository.GetItemAsync(id);
        
        if (existingItem is null) 
        {
            return NotFound();
        }

        Item updatedItem = existingItem with {
            Name = itemDto.Name,
            Price = itemDto.Price
        };  
    
        await repository.UpdateItemAsync(updatedItem);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteItem(Guid id) {
        var existingItem = await repository.GetItemAsync(id);

        if (existingItem is null) 
        {
            return NotFound();
        }

        await repository.DeleteItemAsync(id);

        return NoContent();
    }

}
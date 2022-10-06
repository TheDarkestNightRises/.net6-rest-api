using Catalog.Entities;

public interface IItemsRepository
{
    Task<Item?> GetItemAsync(Guid id);
    Task<IEnumerable<Item>> GetItemsAsync();
    Task CreatedItemAsync(Item item);
    Task UpdateItemAsync(Item item);

    Task DeleteItemAsync(Guid id);
}

using Catalog.Dtos;
using Catalog.Entities;

namespace Catalog;

public static class Extensions
{
    public static ItemDto AsDto(this Item item) 
    {
        return new ItemDto
        {
            Id = item.Id,
            Name = item.Name,
            Price = item.Price,
            CreatedDate = item.CreatedDate
        };
    }

        public static IEnumerable<ItemDto> AsDto(this IEnumerable<Item> items)
        {
            var products = (from item in items 
                            select new ItemDto 
                            {
                                Id = item.Id,
                                Name = item.Name,
                                Price = item.Price,
                                CreatedDate = item.CreatedDate  
                            });
            return products;                
        }
}
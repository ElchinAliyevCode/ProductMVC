using ProductMVC.Models;

namespace ProductMVC.Abstraction;

public interface IBasketService
{
    Task<List<BasketItem>> GetBasketItemAsync();
}

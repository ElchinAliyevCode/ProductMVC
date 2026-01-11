using Microsoft.EntityFrameworkCore;
using ProductMVC.Abstraction;
using ProductMVC.Contexts;
using ProductMVC.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ProductMVC.Services;

public class BasketService : IBasketService
{
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly ProniaDbContext _context;
    public BasketService(IHttpContextAccessor contextAccessor, ProniaDbContext context)
    {
        _contextAccessor = contextAccessor;
        _context = context;
    }

    public async Task<List<BasketItem>> GetBasketItemAsync()
    {
        var userId = _contextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

        var isExistUser = await _context.Users.AnyAsync(x => x.Id == userId);
        if (!isExistUser)
        {
            return [];
        }

        var basketItems = await _context.BasketItems.Include(x => x.Product).Where(x => x.AppUserId == userId).ToListAsync();

        return basketItems;
    }
}

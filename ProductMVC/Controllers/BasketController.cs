using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductMVC.Abstraction;
using ProductMVC.Contexts;
using ProductMVC.Models;
using ProductMVC.Services;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ProductMVC.Controllers;
[Authorize]
public class BasketController : Controller
{
    private readonly ProniaDbContext _context;
    private readonly IBasketService _basketService;
    public BasketController(ProniaDbContext context, IBasketService basketService)
    {
        _context = context;
        _basketService = basketService;
    }

    public async Task<IActionResult> Index()
    {
        var basketItems = await _basketService.GetBasketItemAsync();
        return View(basketItems);
    }

    public async Task<IActionResult> AddToBasket(int productId)
    {
        var isExistProduct = await _context.Products.AnyAsync(x => x.Id == productId);
        if (!isExistProduct)
        {
            return NotFound();
        }

        var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
        var existUser = await _context.Users.AnyAsync(u => u.Id == userId);
        if (!existUser)
        {
            return NotFound();
        }

        var existBasketItem = await _context.BasketItems.FirstOrDefaultAsync(x => x.AppUserId == userId && x.ProductId == productId);

        if (existBasketItem is { })
        {
            existBasketItem.Count++;
            _context.Update(existBasketItem);
        }
        else
        {
            BasketItem item = new BasketItem()
            {
                ProductId = productId,
                Count = 1,
                AppUserId = userId
            };

            await _context.BasketItems.AddAsync(item);
        }



        await _context.SaveChangesAsync();


        return RedirectToAction("Index", "Shop");
    }

    public async Task<IActionResult> DeleteFromBasket(int productId)
    {
        var isExistProduct = await _context.Products.AnyAsync(x => x.Id == productId);
        if (!isExistProduct)
        {
            return NotFound();
        }

        var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
        var existUser = await _context.Users.AnyAsync(u => u.Id == userId);
        if (!existUser)
        {
            return NotFound();
        }

        var existBasketItem = await _context.BasketItems.FirstOrDefaultAsync(x => x.AppUserId == userId && x.ProductId == productId);
        if (existBasketItem == null)
        {
            return NotFound();
        }

        _context.BasketItems.Remove(existBasketItem);
        await _context.SaveChangesAsync();

        var returnUrl = Request.Headers["Referer"];

        if (!string.IsNullOrEmpty(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction("Index", "Shop");
    }

    public async Task<IActionResult> DecreaseBasketItemCount(int productId)
    {
        var isExistProduct = await _context.Products.AnyAsync(x => x.Id == productId);
        if (!isExistProduct)
        {
            return NotFound();
        }

        var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
        var existUser = await _context.Users.AnyAsync(u => u.Id == userId);
        if (!existUser)
        {
            return NotFound();
        }

        var existBasketItem = await _context.BasketItems.FirstOrDefaultAsync(x => x.AppUserId == userId && x.ProductId == productId);
        if (existBasketItem == null)
        {
            return NotFound();
        }

        if (existBasketItem.Count > 1)
        {
            existBasketItem.Count--;
        }
        _context.BasketItems.Update(existBasketItem);
        await _context.SaveChangesAsync();

        var basketItems = await _basketService.GetBasketItemAsync();
        return PartialView("_BasketPartialView", basketItems);
    }

    public async Task<IActionResult> IncreaseBasketItemCount(int productId)
    {
        var isExistProduct = await _context.Products.AnyAsync(x => x.Id == productId);
        if (!isExistProduct)
        {
            return NotFound();
        }

        var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
        var existUser = await _context.Users.AnyAsync(u => u.Id == userId);
        if (!existUser)
        {
            return NotFound();
        }

        var existBasketItem = await _context.BasketItems.FirstOrDefaultAsync(x => x.AppUserId == userId && x.ProductId == productId);
        if (existBasketItem == null)
        {
            return NotFound();
        }

        existBasketItem.Count++;
        _context.BasketItems.Update(existBasketItem);
        await _context.SaveChangesAsync();

        var basketItems = await _basketService.GetBasketItemAsync();
        return PartialView("_BasketPartialView", basketItems);
    }
}

using CartFlow.Data.Data;
using CartFlow.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CartFlow.Web.Controllers;

[Authorize]
public class OrdersController : Controller
{
    private readonly AppDbContext _context;

    public OrdersController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> MyOrders()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString))
            return RedirectToAction("SignIn", "Account");

        int userId = int.Parse(userIdString);

        var orders = await _context.Orders
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.OrderDate)
            .Select(o => new OrderListItem
            {
                Id = o.Id,
                OrderDate = o.OrderDate,
                ItemCount = o.TotalQuantity,
                Total = o.TotalPrice,
                Status = o.OrderStatus.ToString()
            })
            .ToListAsync();

        var viewModel = new MyOrdersViewModel { Orders = orders };
        return View(viewModel);
    }
}

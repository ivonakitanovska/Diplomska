using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrespaEvents.Web.Data;
using PrespaEvents.Web.Models.Domain;
using PrespaEvents.Web.Models.DTO;
using PrespaEvents.Web.Models.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PrespaEvents.Web.Controllers
{
    public class CartController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly UserManager<EventApplicationUser> _userMenager;

        public CartController(ApplicationDbContext context, UserManager<EventApplicationUser> userMenager)
        {
            _context = context;
            _userMenager = userMenager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var loggedInUser = await _context.Users
                .Where(z => z.Id == userId)
                .Include(z => z.UserCart)
                .Include(z => z.UserCart.EventInCarts)
                .Include("UserCart.EventInCarts.Event")
                .FirstOrDefaultAsync();

            var userShoppingCart = loggedInUser.UserCart;

            var eventPrice = userShoppingCart.EventInCarts.Select(z => new
            {
                EventPrice = z.Event.EventPrice,
                Quantity = z.Quantity
            }).ToList();

            double totalPrice = 0;

            foreach (var item in eventPrice)
            {
                totalPrice += item.EventPrice * item.Quantity;
            }

            CartDto cartDtoItem = new CartDto
            {
                EventInCarts = userShoppingCart.EventInCarts.ToList(),
                TotalPrice = totalPrice
            };

           // var allEvents = userShoppingCart.EventInCarts.Select(z => z.Event).ToList();

            
            return View(cartDtoItem);
        }

        public async Task<IActionResult> DeleteEventFromCart(Guid eventId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var loggedInUser = await _context.Users
                .Where(z => z.Id == userId)
                .Include(z => z.UserCart)
                .Include(z => z.UserCart.EventInCarts)
                .Include("UserCart.EventInCarts.Event")
                .FirstOrDefaultAsync();

            var userShoppingCart = loggedInUser.UserCart;

            var eventToDelete = userShoppingCart.EventInCarts
                .Where(z => z.EventId == eventId).FirstOrDefault();

            userShoppingCart.EventInCarts.Remove(eventToDelete);

            _context.Update(userShoppingCart);
            await _context.SaveChangesAsync();
            
            return RedirectToAction("Index", "Cart");
        }

        public async Task<IActionResult> OrderNow()
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var loggedInUser = await _context.Users
               .Where(z => z.Id == userId)
               .Include(z => z.UserCart)
               .Include(z => z.UserCart.EventInCarts)
               .Include("UserCart.EventInCarts.Event")
               .FirstOrDefaultAsync();

            var userCart = loggedInUser.UserCart;

            Order orderItem = new Order
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                User = loggedInUser
            };

            _context.Add(orderItem);

            List<EventInOrder> eventInOrders = new List<EventInOrder>();

            eventInOrders = userCart.EventInCarts
                .Select(z => new EventInOrder
                {
                    OrderId = orderItem.Id,
                    EventId = z.Event.Id,
                    SelectedEvent = z.Event,
                    UserOrder = orderItem
                }).ToList();

            foreach (var item in eventInOrders)
            {
                _context.Add(item);
            }

            loggedInUser.UserCart.EventInCarts.Clear();

            _context.Update(loggedInUser);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Cart");

        }
    }
}

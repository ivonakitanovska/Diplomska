using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrespaEvents.Web.Data;
using PrespaEvents.Web.Models.Domain;
using PrespaEvents.Web.Models.DTO;
using PrespaEvents.Web.Models.Identity;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PrespaEvents.Web.Controllers
{
    [Authorize]
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

        public async Task<Boolean> OrderNow()
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!string.IsNullOrEmpty(userId))
            {
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

                return true;
            }
            else
            {
                return false;
            }

        }

        public IActionResult PayOrder(string stripeEmail, string stripeToken)
        {
            var customerService = new CustomerService();
            var chargeService = new ChargeService();
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var loggedInUser = _context.Users.Include(z => z.UserCart)
               .Include("UserCart.EventInCarts")
               .Include("UserCart.EventInCarts.Event")
               .SingleOrDefault(s => s.Id == userId);

            var userShoppingCart = loggedInUser.UserCart;

            var AllProducts = userShoppingCart.EventInCarts.ToList();

            var allProductPrice = AllProducts.Select(z => new
            {
                ProductPrice = z.Event.EventPrice,
                Quanitity = z.Quantity
            }).ToList();

            var totalPrice = 0;


            foreach (var item in allProductPrice)
            {
                totalPrice += item.Quanitity * item.ProductPrice;
            }

            var order = new ShoppingCartDto
            {
                Events = AllProducts,
                TotalPrice = totalPrice
            };

            var customer = customerService.Create(new CustomerCreateOptions
            {
                Email = stripeEmail,
                Source = stripeToken
            });

            var charge = chargeService.Create(new ChargeCreateOptions
            {
                Amount = (Convert.ToInt32(order.TotalPrice) * 100),
                Description = "EShop Application Payment",
                Currency = "usd",
                Customer = customer.Id
            });

            if (charge.Status == "succeeded")
            {
                var result = this.Order();

                if (result)
                {
                    return RedirectToAction("Index", "Cart");
                }
                else
                {
                    return RedirectToAction("Index", "Cart");
                }
            }

            return RedirectToAction("Index", "Cart");
        }

        private Boolean Order()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = this.OrderNow().Result;

            return result;
        }
    }
}

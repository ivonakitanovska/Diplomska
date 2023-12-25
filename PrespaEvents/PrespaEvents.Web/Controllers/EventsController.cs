using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PrespaEvents.Web.Data;
using PrespaEvents.Web.Models.Domain;
using PrespaEvents.Web.Models.DTO;

namespace PrespaEvents.Web.Controllers
{
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public Event Event { get; private set; }

        public EventsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Events
        //public async Task<IActionResult> Index()
        //{
        //    var all_events = await _context.Events.ToListAsync();
        //    foreach (var item in all_events)
        //    {
        //        item.EventDescription = item.EventDescription.Substring(0, 30) + "...";
        //    }
        //    return View(all_events);
        //}

        public async Task<IActionResult> Index(Guid? selectedCategoryId)
        {
            IQueryable<Event> eventsQuery = _context.Events;

            if (selectedCategoryId.HasValue)
            {
                eventsQuery = eventsQuery.Where(e => e.CategoryId == selectedCategoryId.Value);
            }

            var events = await eventsQuery.ToListAsync();

            foreach (var item in events)
            {
                item.EventDescription = item.EventDescription.Substring(0, 30) + "...";
            }

            var categories = await _context.Category.ToListAsync();

            var viewModel = new CategoryEventsViewModel
            {
                SelectedCategoryId = selectedCategoryId,
                Events = events,
                Categories = new SelectList(categories, "Id", "Name")
            };

            return View(viewModel);
        }

        [Authorize(Roles = "User")]
        public async Task<IActionResult> AddEventToCard(Guid? id)
        {
            var events = await _context.Events.Where(z => z.Id.Equals(id)).FirstOrDefaultAsync();
            AddToShoppingCardDto model = new AddToShoppingCardDto
            {
                SelectedEvent = events,
                EventId = events.Id,
                Quantity = 1
            };
            return View(model);
        }
        

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> AddEventToCard([Bind("EventId", "Quantity")] AddToShoppingCardDto item)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var userShoppingCard = await _context.Carts.Where(z => z.OwnerId.Equals(userId)).FirstOrDefaultAsync();

            if(item.EventId != null && userShoppingCard != null)
            {
                var events = await _context.Events.Where(z => z.Id.Equals(item.EventId)).FirstOrDefaultAsync();

                if(events != null)
                {
                    EventInCart itemToAdd = new EventInCart
                    {
                        Event = events,
                        EventId = events.Id,
                        Cart = userShoppingCard,
                        CartId = userShoppingCard.Id,
                        Quantity = item.Quantity
                    };

                    _context.Add(itemToAdd);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction("Index", "Events");
            }

            return  View(item);
        }

        // GET: Events/Details/5
        [Authorize]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        [Authorize(Roles = "Organizer, Admin")]
        // GET: Events/Create
        public IActionResult Create()
        {
            var categories = _context.Category.ToList();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View();
        }

        // POST: Events/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Organizer, Admin")]
        public async Task<IActionResult> Create([Bind("Id,EventName,EventImage,EventDescription,EventPrice,EventDate, CategoryId")] Event @event)
        {
            if (ModelState.IsValid)
            {
                var organizerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var organizer = _context.Users.Where(z => z.Id.Equals(organizerId)).FirstOrDefault();
                var categoty = _context.Category.Where(z => z.Id.Equals(@event.CategoryId)).FirstOrDefault();
                @event.Id = Guid.NewGuid();
                @event.Organizer = organizer;
                @event.OrganizerId = organizerId;
                @event.EventCategory = categoty;
                _context.Add(@event);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Categories = new SelectList(_context.Category, "CategoryId", "CategoryName", @event.CategoryId);
            return View(@event);
        }

        // GET: Events/Edit/5
        [Authorize(Roles = "Organizer, Admin")]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events.FindAsync(id);
            if (@event == null)
            {
                return NotFound();
            }
            var categories = _context.Category.ToList();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View(@event);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Organizer, Admin")]
        public async Task<IActionResult> Edit(Guid Id, [Bind("Id,EventName,EventImage,EventDescription,EventPrice, EventDate, CategoryId")] Event @event)
        {
            if (Id != @event.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var categoty = _context.Category.Where(z => z.Id.Equals(@event.CategoryId)).FirstOrDefault();
                    //TODO: Add date and other columns to update
                    var event_to_update = _context.Events.Where(z => z.Id == Id).FirstOrDefault();
                    event_to_update.EventName = @event.EventName;
                    event_to_update.EventImage = @event.EventImage;
                    event_to_update.EventDescription = @event.EventDescription;
                    event_to_update.EventPrice = @event.EventPrice;
                    event_to_update.EventDate = @event.EventDate;
                    event_to_update.CategoryId = categoty.Id;
                    event_to_update.EventCategory = categoty;
                    _context.Update(event_to_update);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(@event.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(@event);
        }

        // GET: Events/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var @event = await _context.Events.FindAsync(id);
            _context.Events.Remove(@event);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EventExists(Guid id)
        {
            return _context.Events.Any(e => e.Id == id);
        }
    }
}

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
        public async Task<IActionResult> Index()
        {
            return View(await _context.Events.ToListAsync());
        }

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

        [Authorize(Roles = "Organizer")]
        // GET: Events/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Events/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Organizer")]
        public async Task<IActionResult> Create([Bind("Id,EventName,EventImage,EventDescription,EventPrice,EventDate")] Event @event)
        {
            if (ModelState.IsValid)
            {
                var organizerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var organizer = _context.Users.Where(z => z.Id.Equals(organizerId)).FirstOrDefault();
                @event.Id = Guid.NewGuid();
                @event.Organizer = organizer;
                @event.OrganizerId = organizerId;
                _context.Add(@event);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(@event);
        }

        // GET: Events/Edit/5
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
            return View(@event);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid Id, [Bind("Id,EventName,EventImage,EventDescription,EventPrice, EventDate")] Event @event)
        {
            if (Id != @event.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //TODO: Add date and other columns to update
                    var event_to_update = _context.Events.Where(z => z.Id == Id).FirstOrDefault();
                    event_to_update.EventName = @event.EventName;
                    event_to_update.EventImage = @event.EventImage;
                    event_to_update.EventDescription = @event.EventDescription;
                    event_to_update.EventPrice = @event.EventPrice;
                    event_to_update.EventDate = @event.EventDate;
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

using PrespaEvents.Web.Models.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PrespaEvents.Web.Models.Domain
{
    public class Event
    {
        public Guid Id { get; set; }
        [Required]
        [DisplayName("Event Name")]
        public string EventName { get; set; }
        [Required]
        [DisplayName("Image")]
        public string EventImage { get; set; }
        [Required]
        [DisplayName("Description for the event")]
        public string EventDescription { get; set; }
        [Required]
        [DisplayName("Event Date")]
        public DateTime EventDate { get; set; }
        [Required]
        [DisplayName("Event Price")]
        public int EventPrice { get; set; }
        [DisplayName("Category")]
        public Guid CategoryId { get; set; }
        public Category EventCategory { get; set; }
        public string OrganizerId { get; set; }
        public EventApplicationUser Organizer { get; set; }
        public virtual ICollection<EventInCart> EventInCarts { get; set; }
        public virtual ICollection<EventInOrder> Orders { get; set; }

        public Event() { }
    }
}

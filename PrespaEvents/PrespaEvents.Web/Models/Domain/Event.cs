using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PrespaEvents.Web.Models.Domain
{
    public class Event
    {
        public Guid Id { get; set; }
        [Required]
        public string EventName { get; set; }
        [Required]
        public string EventImage { get; set; }
        [Required]
        public string EventDescription { get; set; }
        [Required]
        public int EventPrice { get; set; }
        public virtual ICollection<EventInCart> EventInCarts { get; set; }
        public virtual ICollection<EventInOrder> Orders { get; set; }

        public Event() { }
    }
}

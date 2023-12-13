using PrespaEvents.Web.Models.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrespaEvents.Web.Models.Domain
{
    public class Cart
    {
        internal object Events;

        public Guid Id { get; set; }

        public string OwnerId { get; set; }

        public EventApplicationUser Owner { get; set; }

        public virtual ICollection<EventInCart> EventInCarts{ get; set; }
    }
}

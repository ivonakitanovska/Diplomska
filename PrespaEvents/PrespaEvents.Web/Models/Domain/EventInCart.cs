using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrespaEvents.Web.Models.Domain
{
    public class EventInCart
    {
        public Guid EventId { get; set; }
        public Event Event { get; set; }
        public Guid CartId { get; set; }
        public Cart Cart { get; set; }

        public int Quantity { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrespaEvents.Web.Models.Domain
{
    public class EventInOrder
    {
        public Guid EventId { get; set; }
        public Event SelectedEvent { get; set; }
        public Guid OrderId { get; set; }
        public Order UserOrder { get; set; }
    }
}

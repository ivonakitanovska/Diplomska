using PrespaEvents.Web.Models.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrespaEvents.Web.Models.Domain
{
    public class Order
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public EventApplicationUser User { get; set; }

        public virtual ICollection<EventInOrder> Events { get; set; }
    }
}

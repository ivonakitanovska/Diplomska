using PrespaEvents.Web.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrespaEvents.Web.Models.DTO
{
    public class AddToShoppingCardDto
    {
        public Event SelectedEvent { get; set; }

        public Guid EventId { get; set; }

        public int Quantity { get; set; }
    }
}

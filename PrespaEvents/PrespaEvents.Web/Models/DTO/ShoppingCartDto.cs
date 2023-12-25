using PrespaEvents.Web.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrespaEvents.Web.Models.DTO
{
    public class ShoppingCartDto
    {
        public List<EventInCart> Events { get; set; }
        public double TotalPrice { get; set; }
    }
}

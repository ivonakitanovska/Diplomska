using PrespaEvents.Web.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrespaEvents.Web.Models.DTO
{
    public class CartDto
    {
        public List<EventInCart> EventInCarts { get; set; }
        public double TotalPrice { get; set; }
    }
}

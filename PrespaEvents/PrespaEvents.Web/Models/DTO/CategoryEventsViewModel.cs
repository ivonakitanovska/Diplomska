using Microsoft.AspNetCore.Mvc.Rendering;
using PrespaEvents.Web.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrespaEvents.Web.Models.DTO
{
    public class CategoryEventsViewModel
    {
        public Guid? SelectedCategoryId { get; set; }
        public List<Event> Events { get; set; }
        public SelectList Categories { get; set; }
    }
}

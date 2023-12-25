using Microsoft.AspNetCore.Identity;
using PrespaEvents.Web.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrespaEvents.Web.Models.Identity
{
    public class EventApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Address { get; set; }

        public virtual Cart UserCart { get; set; }
        public virtual ICollection<Event> MyEvents { get; set; }

    }
}

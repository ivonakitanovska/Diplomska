using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrespaEvents.Web.Models.DTO
{
    public class StripeSettings
    {
        public string SecretKey { get; set; }
        public string PublicKey { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class StripeSettings
    {
        public string PublishableKey => Environment.GetEnvironmentVariable("STRIPE__PUBLISHABLEKEY");
        public string SecretKey => Environment.GetEnvironmentVariable("STRIPE__SECRETKEY");
    }
}

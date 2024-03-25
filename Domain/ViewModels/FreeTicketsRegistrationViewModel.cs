using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModels
{
    public class FreeTicketsRegistrationViewModel
    {
        public int TicketId { get; set; }
        public int Quantity { get; set; }
        public int EventId { get; set; }
    }
}

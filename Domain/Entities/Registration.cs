using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Registration
    {
        public int Id { get; set; }
        public DateTime RegistrationDate { get; set; }

        public int UserAccountId { get; set; }
        public UserAccount UserAccount { get; set; }
        public int EventId { get; set; }
        public Event Event { get; set; }
        //public int TicketTypeId { get; set; }
        //public TicketType TicketType { get; set; }
    }
}

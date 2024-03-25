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
        public string? TransactionId { get; set; }
        public int Quantity { get; set; }
        public double? TicketPrice { get; set; }
        public double? TotalPrice { get; set; }
        public bool IsAssigned { get; set; }
        public int UserAccountId { get; set; }
        public UserAccount UserAccount { get; set; }
        public int EventId { get; set; }
        public Event Event { get; set; }
        public int TicketTypeId { get; set; }
        public TicketType TicketType { get; set; }
        public ICollection<AssignedTicket> AssignedTickets { get; set; }
    }
}

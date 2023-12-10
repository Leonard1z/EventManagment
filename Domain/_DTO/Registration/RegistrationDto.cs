using Domain._DTO.AssignedTickets;
using Domain._DTO.Event;
using Domain._DTO.Ticket;
using Domain._DTO.UserAccount;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain._DTO.Registration
{
    public class RegistrationDto
    {
        public int Id { get; set; }
        [NotMapped]
        public string EncryptedId { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string TransactionId { get; set; }
        public int Quantity { get; set; }
        public double TicketPrice { get; set; }
        public double TotalPrice { get; set; }
        public bool IsAssigned { get; set; }
        public int UserAccountId { get; set; }
        public UserAccountDto UserAccount { get; set; }
        public int EventId { get; set; }
        public EventDto Event { get; set; }
        public int TicketTypeId { get; set; }
        public TicketTypeDto TicketType { get; set; }
        public ICollection<AssignedTicketsDto> AssignedTickets { get; set; }
    }
}

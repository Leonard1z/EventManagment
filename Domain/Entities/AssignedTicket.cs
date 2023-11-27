using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class AssignedTicket
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string EventName { get; set; }
        public DateTime EventStartDate { get; set; }
        public DateTime ExpireDate { get; set; }
        public bool IsInsideEvent { get; set; }
        public string Venue { get; set; }
        public string TicketType { get; set; }
        public double TicketPrice { get; set; }
        public string QrCodeData { get; set; }
        public int TicketNumber { get; set; }
        public int RegistrationId { get; set; }
    }
}

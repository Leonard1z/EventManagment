using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain._DTO.Registration
{
    public class RegistrationDetailsDto
    {
        public int Id { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string EventName { get; set; }
        public DateTime EventStartDate { get; set; }
        public DateTime EventEndDate { get; set; }
        public string Venue { get; set; }
        public string TicketTypeName { get; set; }
        public int TicketTypeId { get; set; }
        public double TicketPrice { get; set; }
        public int Quantity { get; set; }
    }
}

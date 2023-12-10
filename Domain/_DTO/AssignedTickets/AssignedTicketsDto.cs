using Domain._DTO.Registration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain._DTO.AssignedTickets
{
    public class AssignedTicketsDto
    {
        public int Id { get; set; }
        [NotMapped]
        public string EncryptedId  { get; set; }
        [NotMapped]
        public string EncryptedRegistrationId { get; set; }
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
        public RegistrationDto Registration { get; set; }
    }
}

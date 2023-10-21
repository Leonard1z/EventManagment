using Domain._DTO.Event;
using Domain._DTO.Registration;
using Domain._DTO.Reservation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain._DTO.Ticket
{
    public class TicketTypeDto
    {
        public int Id { get; set; }
        [NotMapped]
        public string EncryptedId { get; set; }
        [NotMapped]
        public string EncryptedEventId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public bool IsAvailable { get; set; }
        public string ImagePath { get; set; }
        public DateTime SaleStartDate { get; set; }
        public DateTime SaleEndDate { get; set; }
        public EventDto Event { get; set; }
        public int EventId { get; set; }
        public ICollection<ReservationDto> Reservations { get; set; }
        public ICollection<RegistrationDto> Registrations { get; set; }
    }
}

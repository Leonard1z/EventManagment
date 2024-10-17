using Domain._DTO.Category;
using Domain._DTO.Registration;
using Domain._DTO.Reservation;
using Domain._DTO.Ticket;
using Domain._DTO.UserAccount;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain._DTO.Event
{
    public class EventEditDto
    {
        public int Id { get; set; }
        [NotMapped]
        public string EncryptedId { get; set; }
        [NotMapped]
        public string EncryptedCategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Image { get; set; }
        public string State { get; set; }
        public string? City { get; set; }
        public string? StreetName { get; set; }
        public string Place { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public bool IsActive { get; set; }
        public string Status { get; set; }
        public int UserAccountId { get; set; }
        public UserAccountDto UserAccount { get; set; }
        public int CategoryId { get; set; }
        public IList<CategoryDto> Category { get; set; }
        public ICollection<RegistrationDto> Registrations { get; set; }
        public ICollection<TicketTypeDto> TicketTypes { get; set; }
        public ICollection<ReservationDto> Reservations { get; set; }
    }
}

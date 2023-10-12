using Domain._DTO.Ticket;
using Domain._DTO.UserAccount;
using Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain._DTO.Reservation
{
    public class ReservationDto
    {
        public int Id { get; set; }
        [NotMapped]
        public string EncryptedId { get; set; }
        [NotMapped]
        public string EncryptedUserAccountId { get; set; }
        [NotMapped]
        public string TicketTypesEncryptedId { get; set; }
        public int Quantity { get; set; }
        public int ReservationNumber { get; set; }
        public DateTime ReservationTime { get; set; }
        public DateTime ExpirationTime { get; set; }
        public bool IsExpired { get; set; }
        public ReservationStatus Status { get; set; }
        public double TicketTotalPrice { get; set; }
        public int TicketTypeId { get; set; }
        public TicketTypeDto TicketTypes { get; set; }
        public int UserAccountId { get; set; }
        public UserAccountDto UserAccount { get; set; }
    }
}

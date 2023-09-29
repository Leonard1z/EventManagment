using Domain._DTO.UserAccount;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain._DTO.Reservation
{
    public class ReservationDto
    {
        public int Id { get; set; }
        [NotMapped]
        public string EncryptedId { get; set; }
        public int TicketTypeId { get; set; }
        public int Quantity { get; set; }
        public DateTime ReservationTime { get; set; }
        public bool IsExpired { get; set; }
        public int UserAccountId { get; set; }
        public UserAccountDto UserAccount { get; set; }
    }
}

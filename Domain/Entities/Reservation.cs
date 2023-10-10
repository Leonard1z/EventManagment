namespace Domain.Entities
{
    public class Reservation
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public DateTime ReservationTime { get; set; }
        public DateTime ExpirationTime { get; set; }
        public bool IsExpired { get; set; }
        public double TicketTotalPrice { get; set; }
        public int TicketTypeId { get; set; }
        public TicketType TicketTypes { get; set; }
        public int UserAccountId { get; set; }
        public UserAccount UserAccount { get; set; }
    }
}

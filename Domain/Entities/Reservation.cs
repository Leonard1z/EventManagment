﻿namespace Domain.Entities
{
    public class Reservation
    {
        public int Id { get; set; }
        public int ReservationNumber { get; set; }
        public int Quantity { get; set; }
        public DateTime ReservationTime { get; set; }
        public DateTime ExpirationTime { get; set; }
        public ReservationStatus Status { get; set; }
        public double TicketTotalPrice { get; set; }
        public int TicketTypeId { get; set; }
        public TicketType TicketTypes { get; set; }
        public int UserAccountId { get; set; }
        public UserAccount UserAccount { get; set; }
        public int EventId { get; set; }
        public Event Event { get; set; }
        public ICollection<Notification> Notifications { get; set; }
    }
}

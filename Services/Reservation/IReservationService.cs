using Domain._DTO.Reservation;
using Domain.Entities;
using Services.Common;

namespace Services.Reservation
{
    public interface IReservationService : IService
    {
        Task<Domain.Entities.Reservation> Create(int ticketId,int eventId, int userId, int quantity, double ticketTotalPrice);
        Task SendPaymentReminderEmail(int userId, TicketType ticket, Domain.Entities.Reservation reservation, double ticketTotalPrice, string paymentToken);
        Task<IList<ReservationDto>> GetExpiredReservationsAsync(DateTime currentDate);
        Task<ReservationDto> UpdateAsync(ReservationDto reservationDto);
        Task<ReservationDto> GetByIdWithTicket(int id);
        Task UpdateReservationStatus(int reservationId, ReservationStatus newStatus);
        Task<bool> HasActiveReservationForTickets(int userId, List<int> ticketIds);
        Task<bool> HasCompletedPayment(int userId, int eventId, int ticketId);
    }
}

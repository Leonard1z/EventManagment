using Domain._DTO.Reservation;
using Domain.Entities;
using Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Reservation
{
    public interface IReservationService:IService
    {
        Task<Domain.Entities.Reservation> Create(int ticketId,int userId,int quantity,double ticketTotalPrice);
        Task SendPaymentReminderEmail(int userId, TicketType ticket, Domain.Entities.Reservation reservation, double ticketTotalPrice);
        Task<IList<ReservationDto>> GetExpiredReservationsAsync(DateTime currentDate);
        Task<ReservationDto> UpdateAsync(ReservationDto reservationDto);
    }
}

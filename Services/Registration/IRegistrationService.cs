﻿using Domain._DTO.Registration;
using Services.Common;

namespace Services.Registration
{
    public interface IRegistrationService : IService
    {
        Task<bool> IsUserRegisteredAsync(int userId, int eventId, int ticketTypeId);
        Task RegisterUserForEventAsync(Domain.Entities.Registration registration);
        Task<RegistrationDetailsDto> GetRegistrationById(int id);
        IEnumerable<Domain.Entities.Registration> GetRegistrationByEventId(int eventId);
        Task<List<Domain.Entities.Registration>> GetUserPurchasedTicketsAsync(int userId);
    }
}

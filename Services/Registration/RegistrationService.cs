using AutoMapper;
using Domain._DTO.Registration;
using Infrastructure.Repositories.Registrations;

namespace Services.Registration
{
    public class RegistrationService : IRegistrationService
    {
        private readonly IRegistrationRepository _registrationRepository;
        public readonly IMapper _mapper;

        public RegistrationService(IRegistrationRepository registrationRepository, IMapper mapper)
        {
            _registrationRepository = registrationRepository;
            _mapper = mapper;
        }
        public async Task<bool> IsUserRegisteredAsync(int userId, int eventId, int ticketTypeId)
        {
            return await _registrationRepository.IsUserRegisteredAsync(userId, eventId, ticketTypeId);
        }

        public async Task RegisterUserForEventAsync(Domain.Entities.Registration registration)
        {

            await _registrationRepository.CreateAsync(registration);
        }
        public IEnumerable<Domain.Entities.Registration> GetRegistrationByEventId(int eventId)
        {
            return _registrationRepository.GetRegistrationByEventId(eventId);
        }

        public async Task<List<Domain.Entities.Registration>> GetUserPurchasedTicketsAsync(int userId)
        {
            return await _registrationRepository.GetUserPurchasedTicketsAsync(userId);
        }

        public async Task<RegistrationDetailsDto> GetRegistrationById(int id)
        {
            var registration = await _registrationRepository.GetRegistrationById(id);

            var registrationDetailsDto = new RegistrationDetailsDto
            {
                Id = registration.Id,
                RegistrationDate = registration.RegistrationDate,
                EventName = registration.Event.Name,
                EventStartDate = registration.Event.StartDate,
                EventEndDate = registration.Event.EndDate,
                Venue = registration.Event.StreetName,
                TicketTypeName = registration.TicketType.Name,
                TicketTypeId = registration.TicketTypeId,
                Quantity = registration.Quantity,
                TicketPrice = registration.TicketPrice,
                IsAssigned = registration.IsAssigned,
            };

            return registrationDetailsDto;
        }

        public async Task<int> GetTotalTicketsSoldForUser(int userId)
        {
            return await _registrationRepository.GetTotalTicketsSoldForUser(userId);
        }
    }
}

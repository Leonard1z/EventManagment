using AutoMapper;
using Infrastructure.Repositories.Registrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            return await _registrationRepository.IsUserRegisteredAsync(userId, eventId,ticketTypeId);
        }

        public async Task RegisterUserForEventAsync(Domain.Entities.Registration registration)
        {

            await _registrationRepository.CreateAsync(registration);
        }
        public IEnumerable<Domain.Entities.Registration> GetRegistrationByEventId(int eventId)
        {
            return _registrationRepository.GetRegistrationByEventId(eventId);
        }
    }
}

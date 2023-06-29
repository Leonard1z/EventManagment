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
        public bool Delete(int id)
        {
            _registrationRepository.Delete(id);

            return true;
        }
        public async Task<bool> CheckIfUserExist(int userId, int eventId)
        {
            var registration = await _registrationRepository.GetUserAndEvent(userId, eventId);
            //return true if user exist if not returns false
            return registration != null;
        }

        public async Task RegisterUserForEvent(int userId, int eventId)
        {

            var registration = new Domain.Entities.Registration
            {
                UserAccountId = userId,
                EventId = eventId,
                RegistrationDate = DateTime.UtcNow
            };

            _registrationRepository.Create(registration);
        }
        public IEnumerable<Domain.Entities.Registration> GetRegistrationByEventId(int eventId)
        {
            return _registrationRepository.GetRegistrationByEventId(eventId);
        }
    }
}

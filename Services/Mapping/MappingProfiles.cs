using AutoMapper;
using Domain._DTO.AssignedTickets;
using Domain._DTO.Category;
using Domain._DTO.Event;
using Domain._DTO.Permission;
using Domain._DTO.Registration;
using Domain._DTO.Reservation;
using Domain._DTO.Role;
using Domain._DTO.Ticket;
using Domain._DTO.UserAccount;
using Domain.Entities;

namespace Services.Mapping
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            #region
            CreateMap<Category, CategoryDto>();
            CreateMap<CategoryDto, Category>();

            CreateMap<Category, CategoryCreateDto>();
            CreateMap<CategoryCreateDto, Category>();
            #endregion

            #region
            CreateMap<Event, EventDto>();
            CreateMap<EventDto, Event>();

            CreateMap<Event, EventCreateDto>();
            CreateMap<EventCreateDto, Event>();

            CreateMap<Event, EventEditDto>();
            CreateMap<EventEditDto, Event>();

            CreateMap<Event, EventWithMetricsDto>();
            CreateMap<EventWithMetricsDto, Event>();
            #endregion

            #region
            CreateMap<Domain.Entities.Registration, RegistrationDto>();
            CreateMap<RegistrationDto, Domain.Entities.Registration>();
            #endregion

            #region
            CreateMap<Domain.Entities.UserAccount, UserAccountDto>();
            CreateMap<UserAccountDto, Domain.Entities.UserAccount>();

            CreateMap<Domain.Entities.UserAccount, UserAccountCreateDto>();
            CreateMap<UserAccountCreateDto, Domain.Entities.UserAccount>();

            CreateMap<Domain.Entities.UserAccount, UserAccountEditDto>();
            CreateMap<UserAccountEditDto, Domain.Entities.UserAccount>();

            CreateMap<Domain.Entities.UserAccount, ProfileUpdateDto>();
            CreateMap<ProfileUpdateDto, Domain.Entities.UserAccount>();
            #endregion


            #region
            CreateMap<Roles, RoleDto>();
            CreateMap<RoleDto, Roles>();
            #endregion

            #region
            CreateMap<Permission, PermissionDto>();
            CreateMap<PermissionDto, Permission>();
            #endregion

            #region
            CreateMap<TicketType, TicketTypeDto>();
            CreateMap<TicketTypeDto, TicketType>();

            CreateMap<TicketType, TicketTypeEditDto>();
            CreateMap<TicketTypeEditDto, TicketType>();
            #endregion

            #region
            CreateMap<Domain.Entities.Reservation, ReservationDto>();
            CreateMap<ReservationDto, Domain.Entities.Reservation>();
            #endregion

            #region
            CreateMap<AssignedTicket, AssignedTicketsDto>();
            CreateMap<AssignedTicketsDto, AssignedTicket>();
            #endregion

        }
    }
}

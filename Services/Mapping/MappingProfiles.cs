﻿using AutoMapper;
using Domain._DTO.Category;
using Domain._DTO.Event;
using Domain._DTO.Registration;
using Domain._DTO.Role;
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
            #endregion

            #region
            CreateMap<Registration, RegistrationDto>();
            CreateMap<RegistrationDto, Registration>();

            CreateMap<Registration, RegistrationCreateDto>();
            CreateMap<RegistrationCreateDto, Registration>();
            #endregion

            #region
            CreateMap<Domain.Entities.UserAccount, UserAccountDto>();
            CreateMap<UserAccountDto, Domain.Entities.UserAccount>();

            CreateMap<Domain.Entities.UserAccount, UserAccountCreateDto>();
            CreateMap<UserAccountCreateDto, Domain.Entities.UserAccount>();
            #endregion


            #region
            CreateMap<Role, RoleDto>();
            CreateMap<RoleDto, Role>();
            #endregion
        }
    }
}

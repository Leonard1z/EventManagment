﻿using AutoMapper;
using Domain._DTO.Role;
using Domain.Entities;
using Infrastructure.Repositories.Roles;

namespace Services.Role
{
    public class RoleService : IRoleService
    {

        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;

        public RoleService(IRoleRepository roleRepository, IMapper mapper)
        {
            _roleRepository = roleRepository;
            _mapper = mapper;
        }

        public RoleDto Create(RoleDto roleDto)
        {
            var result = _roleRepository.Create(_mapper.Map<Roles>(roleDto));

            return _mapper.Map<RoleDto>(result);
        }

        public bool Delete(int id)
        {
            _roleRepository.Delete(id);

            return true;
        }

        public async Task<IEnumerable<RoleDto>> GetAll()
        {
            var result = await _roleRepository.GetAll();

            return _mapper.Map<List<RoleDto>>(result.ToList());
        }

        public async Task<RoleDto> GetById(int id)
        {
            return _mapper.Map<RoleDto>(await _roleRepository.GetById(id));
        }

        public RoleDto Update(RoleDto roleDto)
        {
            var result = _roleRepository.Update(_mapper.Map<Roles>(roleDto));

            return _mapper.Map<RoleDto>(result);
        }
        public RoleDto GetDefaultRole()
        {
            var result = _roleRepository.GetDefaultRole();

            return _mapper.Map<RoleDto>(result);
        }
    }
}

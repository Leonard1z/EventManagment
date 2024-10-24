using AutoMapper;
using Domain._DTO.Permission;
using Domain._DTO.Role;
using Domain.Entities;
using Infrastructure.Repositories.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Permissions
{
    public class PermissionService : IPermissionService
    {
        private readonly IPermissionRepository _permissionRepository;
        private readonly IMapper _mapper;

        public PermissionService(IPermissionRepository permissionRepository, IMapper mapper)
        {
            _permissionRepository = permissionRepository;
            _mapper = mapper;
        }

        public async Task<List<PermissionDto>> GetAllPermissionsAsync()
        {
            var result = await _permissionRepository.GetAll();

            return _mapper.Map<List<PermissionDto>>(result.ToList());
        }

        public async Task<PermissionDto> GetByIdAsync(int id)
        {
            return _mapper.Map<PermissionDto>(await _permissionRepository.GetById(id));
        }
    }
}

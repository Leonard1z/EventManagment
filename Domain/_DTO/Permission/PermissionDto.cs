using Domain._DTO.Role;
using Domain._DTO.UserAccount;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain._DTO.Permission
{
    public class PermissionDto
    {
        public int Id { get; set; }
        [NotMapped]
        public string? EncryptedId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<UserAccountDto> UserAccounts { get; set; }
        public ICollection<RoleDto> Roles { get; set; }
    }
}

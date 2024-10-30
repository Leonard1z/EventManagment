using Domain._DTO.Permission;
using Domain._DTO.UserAccount;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain._DTO.Role
{
    public class RoleDto
    {
        public int Id { get; set; }
        [NotMapped]
        public string? EncryptedId { get; set; }
        [Required(ErrorMessage = "Role name is required")]
        public string Name { get; set; }
        public ICollection<UserAccountDto>? UserAccounts { get; set; }
        public ICollection<PermissionDto>? Permissions { get; set; }
    }
}

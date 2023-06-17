using Domain._DTO.UserAccount;
using System;
using System.Collections.Generic;
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
        public string EncryptedUserAccountId { get; set; }
        public string Name { get; set; }
        public ICollection<UserAccountDto> UserAccounts { get; set; }
    }
}

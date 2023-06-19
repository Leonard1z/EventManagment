using Domain._DTO.Role;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain._DTO.UserAccount
{
    public class UserAccountCreateDto
    {
        [NotMapped]
        [Required(ErrorMessage = "Role is required...")]
        public string EncryptedRoleId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [DataType("Password")]
        public string Password { get; set; }
        [DataType("Password")]
        [Compare("Password", ErrorMessage = "Confirm password must match with the Password")]
        public string ConfirmPassword { get; set; }
        public string? Salt { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public char Gender { get; set; }
        public int RoleId { get; set; }
        public ICollection<RoleDto>? Role { get; set; }
    }
}

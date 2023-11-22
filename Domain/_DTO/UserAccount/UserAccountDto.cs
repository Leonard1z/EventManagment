using Domain._DTO.Role;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain._DTO.UserAccount
{
    public class UserAccountDto
    {
        public int Id { get; set; }
        [NotMapped]
        public string EncryptedId { get; set; }
        public string FirstName { get; set; }
        public string Username { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        [DataType("Password")]
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public char Gender { get; set; }
        public bool IsEmailVerified { get; set; }
        public string EmailVerificationToken { get; set; }
        public int RoleId { get; set; }
        public RoleDto Role { get; set; }
    }
}

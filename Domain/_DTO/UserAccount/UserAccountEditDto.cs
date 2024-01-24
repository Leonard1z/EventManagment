using Domain._DTO.Role;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain._DTO.UserAccount
{
	public class UserAccountEditDto
	{
		public int Id { get; set; }
		[NotMapped]
		public string EncryptedId { get; set; }
		public string EncryptedRoleId { get; set; }
		public string FirstName { get; set; }
		public string Username { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }
		public string Password { get; set; }
		public string Salt { get; set; }
		public string PhoneNumber { get; set; }
		public string Address { get; set; }
		public char Gender { get; set; }
		public bool IsEmailVerified { get; set; }
		public string EmailVerificationToken { get; set; }
		public string? PasswordResetToken { get; set; }
		public DateTime? PasswordResetTokenExpiry { get; set; }
		public int RoleId { get; set; }
		public List<RoleDto> Role { get; set; }
	}
}

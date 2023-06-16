using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain._DTO.UserAccount
{
    public class UserAccountCreateDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [DataType("Password")]
        public string Password { get; set; }
        [DataType("Password")]
        public string ConfirmPassword { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public char Gender { get; set; }
        public bool IsAdmin { get; set; } = false;
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class UserAccount
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        [DataType("Password")]
        public string Password { get; set; }
        public string? Salt { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public char Gender { get; set; }
        public bool IsEmailVerified { get; set; }
        public string? EmailVerificationToken { get; set; }
        public int RoleId { get; set; }
        public Roles Role { get; set; }
        public ICollection<Event> Events { get; set; }
        public ICollection<Registration> Registrations { get; set; }
    }
}

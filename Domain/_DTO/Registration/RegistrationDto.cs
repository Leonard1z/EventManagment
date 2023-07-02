using Domain._DTO.Event;
using Domain._DTO.UserAccount;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain._DTO.Registration
{
    public class RegistrationDto
    {
        public int Id { get; set; }
        [NotMapped]
        public string EncryptedId { get; set; }
        public DateTime RegistrationDate { get; set; }
        public int UserAccountId { get; set; }
        public UserAccountDto UserAccount { get; set; }
    }
}

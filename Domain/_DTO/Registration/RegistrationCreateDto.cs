using Domain._DTO.Event;
using Domain._DTO.UserAccount;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

using System.Threading.Tasks;

namespace Domain._DTO.Registration
{
    public class RegistrationCreateDto
    {
        [NotMapped]
        public string EncryptedUserAccountId { get; set; }
        [NotMapped]
        public string EncryptedEventId { get; set; }
        public DateTime RegistrationDate { get; set; } = DateTime.Now;
        public int EventId { get; set; }
        public EventDto Event { get; set; }
        public int UserAccountId { get; set; }
        public UserAccountDto UserAccount { get; set; }

    }
}

﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain._DTO.Registration
{
    public class RegistrationCreateDto
    {
        public int UserAccountId { get; set; }
        [NotMapped]
        public string EncryptedUserAccountId { get; set; }
        public int EventId { get; set; }
        [NotMapped]
        public string EncryptedEventId { get; set; }
        public DateTime RegistrationDate { get; set; }=DateTime.Now;
    }
}
﻿using Domain._DTO.Event;
using Domain._DTO.Registration;
using Domain._DTO.Role;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain._DTO.UserAccount
{
    public class UserAccountDto
    {
        public int Id { get; set; }
        [NotMapped]
        public string EncryptedId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        [DataType("Password")]
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public char Gender { get; set; }
        public int RoleId { get; set; }
        public RoleDto Role { get; set; }
        public ICollection<EventDto> Events { get; set; }
        public ICollection<RegistrationDto> Registrations { get; set; }
    }
}
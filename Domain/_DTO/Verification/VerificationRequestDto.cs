﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain._DTO.Verification
{
    public class VerificationRequestDto
    {
        public string PhoneNumber { get; set; }
        public string? VerificationCode { get; set; }
    }
}

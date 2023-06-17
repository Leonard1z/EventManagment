﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Event
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Image { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public ICollection<Registration> Registrations { get; set; }

    }
}
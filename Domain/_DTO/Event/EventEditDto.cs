using Domain._DTO.Category;
using Domain._DTO.Registration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain._DTO.Event
{
    public class EventEditDto
    {
        public int Id { get; set; }
        [NotMapped]
        public string EncryptedId { get; set; }
        [NotMapped]
        public string EncryptedCategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Image { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public int CategoryId { get; set; }
        public IList<CategoryDto> Category { get; set; }
        public ICollection<RegistrationDto> Registrations { get; set; }
    }
}

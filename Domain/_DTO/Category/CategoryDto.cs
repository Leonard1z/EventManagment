using Domain._DTO.Event;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain._DTO.Category
{
    public class CategoryDto
    {
        public int Id { get; set; }
        [NotMapped]
        public string EncryptedId { get; set; }
        public string Name { get; set; }
    }
}

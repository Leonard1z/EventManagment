using Domain._DTO.Category;
using Domain._DTO.UserAccount;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain._DTO.Event
{
    public class EventCreateDto
    {
        [NotMapped]
        [Required(ErrorMessage = "Category is required...")]
        public string EncryptedCategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; } = DateTime.Now.Date;
        public DateTime EndDate { get; set; } = DateTime.Now.Date;
        public string Image { get; set; }
        public string State { get; set; }
        public string? City { get; set; }
        public string? StreetName { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public bool IsActive { get; set; }
        public int UserAccountId { get; set; }
        public UserAccountDto? UserAccount { get; set; }
        public int CategoryId { get; set; }
        public IList<CategoryDto> Category { get; set; }
    }
}

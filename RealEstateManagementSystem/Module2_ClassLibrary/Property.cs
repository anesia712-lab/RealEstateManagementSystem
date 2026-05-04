using System.ComponentModel.DataAnnotations;

namespace RealEstateManagementSystem.Models
{
    public class Property
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Address is required.")]
        public string Address { get; set; }
        
        [Required(ErrorMessage = "Price is required.")]
        [Range(1000, 100000000, ErrorMessage = "Price must be between 1,000 and 100,000,000")]
        public decimal Price { get; set; }
        
        [Range(1, 50, ErrorMessage = "Bedrooms must be between 1 and 50")]
        public int Bedrooms { get; set; }
        
        [Range(1, 50, ErrorMessage = "Bathrooms must be between 1 and 50")]
        public int Bathrooms { get; set; }
        
        [Range(100, 50000, ErrorMessage = "Square Feet must be between 100 and 50,000")]
        public int SquareFeet { get; set; }
    }
}

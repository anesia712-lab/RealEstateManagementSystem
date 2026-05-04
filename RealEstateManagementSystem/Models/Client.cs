using System.ComponentModel.DataAnnotations;

namespace RealEstateManagementSystem.Models
{
    public class Client
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Full Name is required.")]
        public string FullName { get; set; }
        
        [Required(ErrorMessage = "Phone Number is required.")]
        [RegularExpression(@"^\d{3}-\d{3}-\d{4}$", ErrorMessage = "Phone format must be 123-456-7890")]
        public string PhoneNumber { get; set; }
        
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        public string Email { get; set; }
        
        [Required(ErrorMessage = "Address is required.")]
        public string Address { get; set; }
    }
}

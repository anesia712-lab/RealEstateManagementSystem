namespace RealEstateManagementSystem.Models
{
    public class Property
    {
        public int Id { get; set; }
        public string Address { get; set; }
        public decimal Price { get; set; }
        public int Bedrooms { get; set; }

        public int Bathrooms { get; set; }
        public int SquareFeet { get; set; }
    }
}

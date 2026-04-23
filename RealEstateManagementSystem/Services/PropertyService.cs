using RealEstateManagementSystem.Models;

namespace RealEstateManagementSystem.Services
{
    public static class PropertyService
    {
        private static List<Property> _properties = new List<Property>();
        private static int _nextId = 1;

        public static List<Property> GetAll()
        {
            return _properties;
        }

        public static void Add(Property property)
        {
            property.Id = _nextId;
            _nextId++;
            _properties.Add(property);
        }

        public static Property GetById(int id)
        {
            return _properties.FirstOrDefault(p => p.Id == id);
        }
    }
}

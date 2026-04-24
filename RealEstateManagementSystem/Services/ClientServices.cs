using RealEstateManagementSystem.Models;

namespace RealEstateManagementSystem.Services
{
    public static class ClientService
    {
        private static List<Client> _client = new List<Client>();
        private static int _nextId = 1;

        public static List<Client> GetAll()
        {
            return _client;
        }

        public static void Add(Client client)
        {
            client.Id = _nextId;
            _nextId++;
            _client.Add(client);
        }

        public static Client GetById(int id)
        {
            return _client.FirstOrDefault(c => c.Id == id);
        }
    }
}


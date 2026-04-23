using RealEstateManagementSystem.Models;

namespace RealEstateManagementSystem.Services
{
    public static class ClientService
    {
        private static List<Client> _clients = new List<Client>();
        private static int _nextId = 1;

        public static List<Client> GetAll()
        {
            return _clients;
        }

        public static void Add(Client client)
        {
            client.Id = _nextId;
            _nextId++;
            _clients.Add(client);
        }

        public static Client GetById(int id)
        {
            return _clients.FirstOrDefault(c => c.Id == id);
        }
    }
}


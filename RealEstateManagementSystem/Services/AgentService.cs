using RealEstateManagementSystem.Models;

namespace RealEstateManagementSystem.Services
{
    public static class AgentService
    {
        private static List<Agent> _agents = new List<Agent>();
        private static int _nextId = 1;

        public static List<Agent> GetAll()
        {
            return _agents;
        }

        public static void Add(Agent agent)
        {
            agent.Id = _nextId;
            _nextId++;
            _agents.Add(agent);
        }

        public static Agent GetById(int id)
        {
            return _agents.FirstOrDefault(a => a.Id == id);
        }
    }
}

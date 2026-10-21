using ParcialWebAPI.Models;

namespace ParcialWebAPI.Repositories
{
    public interface IRepository
    {
        Agente? GetById(int id);

        IEnumerable<Agente> GetActiveAgentsWithPendingTasks(string? modelName);

        bool AssignTaskToAgent(int agentId, string desc, int tokens);
    }
}

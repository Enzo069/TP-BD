using Microsoft.EntityFrameworkCore;
using ParcialWebAPI.Models;

namespace ParcialWebAPI.Repositories
{

    
    public class AgentesRepository : IRepository
    {
        private readonly AgentesDbContext db;
        public AgentesRepository(AgentesDbContext db)
        {
         this.db = db;   
        }


        public bool AssignTaskToAgent(int agentId, string desc, int tokens)
        {
            if(tokens <= 0 || tokens > 100000)
            {
                return false;
            }

            Agente? agenteExiste = GetById(agentId);
            if(agenteExiste == null) { 
                return false; 
            }
            if (agenteExiste.EstaActivo == false)
            {
                return false;
            }

            Agente Agentedb = db.Agentes.Include(a => a.TareasAgentes).FirstOrDefault(a => a.Id == agentId)!;
            int cantidad = Agentedb.TareasAgentes.Count(t => t.EstaCompleta == false);
            if (cantidad >= 5)
            {
                return false;
            }


            TareasAgente tareaNueva = new TareasAgente();
            tareaNueva.IdAgente = agentId;
            tareaNueva.Descripcion = desc;
            tareaNueva.TokensUsados = tokens;
            tareaNueva.EstaCompleta = false;
            tareaNueva.Fecha = DateOnly.FromDateTime(DateTime.UtcNow);

            db.TareasAgentes.Add(tareaNueva);
            db.SaveChanges();

            return true;
        }

        public IEnumerable<Agente> GetActiveAgentsWithPendingTasks(string? modelName)
        {
            if (string.IsNullOrEmpty(modelName))
            {
                //validar activos y pendientes
                return db.Agentes.Where(a => a.EstaActivo == true && a.TareasAgentes.Any(t => t.EstaCompleta == false)).ToList();

            }
            else
            {
                //validar activos, modelo y pendientes
                return db.Agentes.Where(a => a.EstaActivo == true && a.Modelo == modelName && a.TareasAgentes.Any(t => t.EstaCompleta == false)).ToList();
            }
        }

        public Agente? GetById(int id)
        {
            return db.Agentes.Find(id);
        }
    }
}

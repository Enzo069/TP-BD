using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ParcialWebAPI.Dtos;
using ParcialWebAPI.Models;
using ParcialWebAPI.Repositories;

namespace ParcialWebAPI.Controllers
{
    [ApiController]
    [Route("api/agentes")]
    public class AgentesController : Controller
    {
        private readonly IRepository repo;
        public AgentesController(IRepository repo)
        {
            this.repo = repo;
        }

        [HttpGet]
        [Route("{id}")]
        public ActionResult GetById([FromRoute] int id)
        {
            try {
                Agente? agentedb = repo.GetById(id);

                if (agentedb == null)
                {
                    return NotFound();
                }
                else
                {
                    return Ok(agentedb);
                }
            
            }
            catch (Exception ex)
            { 
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("/activos")]
        public IActionResult GetByFilter([FromQuery] string modelo)
        {
            try
            {
                return Ok(repo.GetActiveAgentsWithPendingTasks(modelo));
            }
            catch (Exception) { 
                return BadRequest();
            }
            
        }

        [HttpPost]
        [Route("{id}/tarea")]
        public IActionResult AssignTask([FromRoute] int id, [FromBody] CreateTaskDto dto)
        {
            try
            {
                bool aux = repo.AssignTaskToAgent(id, dto.Descripcion, dto.Tokens);
                if (aux)
                {
                    return Created();
                }
                else
                {
                    return BadRequest();
                }

            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}

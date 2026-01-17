using Microsoft.AspNetCore.Mvc;
using TrilhaApiDesafio.Context;
using TrilhaApiDesafio.Models;
using TrilhaApiDesafio.Services;

namespace TrilhaApiDesafio.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TarefaController : ControllerBase
    {
        private readonly OrganizadorContext _context;

        private readonly ITarefaService _service;

        public TarefaController(ITarefaService service)
        {
            _service = service;
        }


        [HttpGet("{id}")]
        public IActionResult ObterPorId(int id)
        {
            var tarefa = _service.ObterPorId(id);
            if (tarefa == null) return NotFound("Nenhuma tarefa encontrada com o id informado");
            return Ok(tarefa);
        }

        [HttpGet("ObterTodos")]
        public IActionResult ObterTodos()
        => Ok(_service.ObterTodos());

        [HttpGet("ObterPorTitulo")]
        public IActionResult ObterPorTitulo(string titulo)
        => Ok(_service.ObterPorTitulo(titulo));

        [HttpGet("ObterPorData")]
        public IActionResult ObterPorData(DateTime data)
        => Ok(_service.ObterPorData(data));

        [HttpGet("ObterPorStatus")]
        public IActionResult ObterPorStatus(EnumStatusTarefa status)
        => Ok(_service.ObterPorStatus(status));

        [HttpPost]
        public IActionResult Criar(Tarefa tarefa)
        {
            if (tarefa.Data == DateTime.MinValue)
                return BadRequest("Data inválida");

            var resultado = _service.Criar(tarefa);

            if (!resultado.Sucesso)
                return BadRequest(new { Erro = resultado.Mensagem });

            return CreatedAtAction(nameof(ObterPorId), new { id = tarefa.Id }, tarefa);
        }

        [HttpPut("{id}")]
        public IActionResult Atualizar(int id, Tarefa tarefa)
        {
            if (tarefa.Data == DateTime.MinValue)
                return BadRequest("Data inválida");

            var resultado = _service.Atualizar(id, tarefa);

            if (!resultado.Sucesso)
            {
                if (resultado.Mensagem == "Tarefa não encontrada.")
                    return NotFound(new { Erro = resultado.Mensagem });

                return BadRequest(new { Erro = resultado.Mensagem });
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Deletar(int id)
        {
            var deletado = _service.Deletar(id);

            if (deletado)
                return NoContent();
            else
                return Ok("ID não encontrado, nenhuma tarefa foi deletada.");
        }
    }
}

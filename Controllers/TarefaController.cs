using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TrilhaApiDesafio.Models;
using TrilhaApiDesafio.Services;

namespace TrilhaApiDesafio.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TarefaController : ControllerBase
    {
        private readonly ITarefaService _service;
        private readonly ILogger<TarefaController> _logger;

        public TarefaController(ITarefaService service, ILogger<TarefaController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(Tarefa), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult ObterPorId(int id)
        {
            if (id <= 0)
                return BadRequest("Id inválido.");

            var tarefa = _service.ObterPorId(id);
            if (tarefa == null)
                return NotFound("Nenhuma tarefa encontrada com o id informado");

            return Ok(tarefa);
        }

        [HttpGet("ObterTodos")]
        [ProducesResponseType(typeof(IEnumerable<Tarefa>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ResponseCache(Duration = 30, VaryByQueryKeys = new[] { "page", "pageSize" })]
        public IActionResult ObterTodos([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page <= 0 || pageSize <= 0 || pageSize > 100)
                return BadRequest("Parâmetros inválidos: 'page' e 'pageSize' devem ser positivos. 'pageSize' máximo = 100.");

            var todas = _service.ObterTodos()?.ToList() ?? new List<Tarefa>();

            if (!todas.Any())
                return NoContent();

            var total = todas.Count;
            var itens = todas
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            Response.Headers["X-Total-Count"] = total.ToString();
            Response.Headers["X-Page"] = page.ToString();
            Response.Headers["X-Page-Size"] = pageSize.ToString();

            return Ok(new
            {
                Total = total,
                Page = page,
                PageSize = pageSize,
                Items = itens
            });
        }

        [HttpGet("ObterPorTitulo")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult ObterPorTitulo([FromQuery] string titulo)
        {
            if (string.IsNullOrWhiteSpace(titulo))
                return BadRequest("O parâmetro 'titulo' é obrigatório.");

            var tituloNormalizado = titulo.Trim();
            var tarefas = _service.ObterPorTitulo(tituloNormalizado);

            if (tarefas == null || !tarefas.Any())
                return NotFound($"Nenhuma tarefa encontrada com o título informado: '{tituloNormalizado}'.");

            return Ok(tarefas);
        }

                [HttpGet("ObterPorData")]
        [ProducesResponseType(typeof(IEnumerable<Tarefa>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ResponseCache(Duration = 30, VaryByQueryKeys = new[] { "data" })]
        public IActionResult ObterPorData([FromQuery] DateTime? data)
        {
            if (!data.HasValue || data.Value == DateTime.MinValue)
                return BadRequest("O parâmetro 'data' é obrigatório e deve ser uma data válida (ex: 2026-01-18).");

            var tarefas = _service.ObterPorData(data.Value)?.ToList() ?? new List<Tarefa>();

            if (!tarefas.Any())
            {
                _logger?.LogInformation("Nenhuma tarefa encontrada para a data {Data}", data.Value.Date);
                return NotFound($"Nenhuma tarefa encontrada para a data informada: {data.Value:yyyy-MM-dd}.");
            }

            return Ok(tarefas);
        }

        [HttpGet("ObterPorStatus")]
        [ProducesResponseType(typeof(IEnumerable<Tarefa>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ResponseCache(Duration = 30, VaryByQueryKeys = new[] { "status" })]
        public IActionResult ObterPorStatus([FromQuery] EnumStatusTarefa? status)
        {
            if (!ModelState.IsValid)
                return BadRequest("Valor de 'status' inválido.");

            if (!status.HasValue)
                return BadRequest("O parâmetro 'status' é obrigatório.");

            var tarefas = _service.ObterPorStatus(status.Value)?.ToList() ?? new List<Tarefa>();

            if (!tarefas.Any())
                return NoContent();

            return Ok(tarefas);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Tarefa), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Criar([FromBody] Tarefa tarefa)
        {
            if (tarefa == null)
                return BadRequest("Tarefa não pode ser nula.");

            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            if (tarefa.Data == DateTime.MinValue)
                return BadRequest("Data inválida");

            try
            {
                var resultado = _service.Criar(tarefa);

                if (!resultado.Sucesso)
                {
                    if (!string.IsNullOrWhiteSpace(resultado.Mensagem) &&
                        resultado.Mensagem.Contains("Já existe", StringComparison.OrdinalIgnoreCase))
                    {
                        return Conflict(new { Erro = resultado.Mensagem });
                    }

                    return BadRequest(new { Erro = resultado.Mensagem });
                }

                var tarefaPersistida = _service.ObterPorId(tarefa.Id) ?? tarefa;

                return CreatedAtAction(nameof(ObterPorId), new { id = tarefaPersistida.Id }, tarefaPersistida);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar tarefa");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno ao processar a requisição.");
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Atualizar(int id, Tarefa tarefa)
        {
            _logger.LogDebug("Atualizar chamada para id={Id}", id);

            if (id <= 0)
            {
                _logger.LogWarning("Id inválido informado: {Id}", id);
                return BadRequest("Id inválido.");
            }

            if (tarefa == null)
            {
                _logger.LogWarning("Corpo da requisição nulo para id={Id}", id);
                return BadRequest("Tarefa não pode ser nula.");
            }

            if (tarefa.Id != 0 && tarefa.Id != id)
            {
                _logger.LogWarning("Id da rota ({RouteId}) diferente do Id do body ({BodyId})", id, tarefa.Id);
                return BadRequest("O id do recurso não corresponde ao id da rota.");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState inválido ao atualizar id={Id}", id);
                return ValidationProblem(ModelState);
            }

            if (tarefa.Data == DateTime.MinValue)
            {
                _logger.LogWarning("Data inválida ao atualizar id={Id}", id);
                return BadRequest("Data inválida");
            }

            try
            {
                tarefa.Id = id;

                var resultado = _service.Atualizar(id, tarefa);

                if (!resultado.Sucesso)
                {
                    if (string.Equals(resultado.Mensagem, "Tarefa não encontrada.", StringComparison.Ordinal))
                    {
                        _logger.LogInformation("Tarefa não encontrada ao atualizar id={Id}", id);
                        return NotFound(new { Erro = resultado.Mensagem });
                    }

                    if (!string.IsNullOrWhiteSpace(resultado.Mensagem) &&
                        resultado.Mensagem.Contains("Já existe", StringComparison.OrdinalIgnoreCase))
                    {
                        _logger.LogInformation("Conflito ao atualizar id={Id}: {Mensagem}", id, resultado.Mensagem);
                        return Conflict(new { Erro = resultado.Mensagem });
                    }

                    _logger.LogInformation("Falha ao atualizar id={Id}: {Mensagem}", id, resultado.Mensagem);
                    return BadRequest(new { Erro = resultado.Mensagem });
                }

                _logger.LogInformation("Tarefa atualizada com sucesso id={Id}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro interno ao atualizar tarefa id={Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno ao processar a requisição.");
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Deletar(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Tentativa de deletar com id inválido: {Id}", id);
                return BadRequest("Id inválido.");
            }

            try
            {
                _logger.LogDebug("Deletar chamada para id={Id}", id);

                var deletado = _service.Deletar(id);

                if (deletado)
                {
                    _logger.LogInformation("Tarefa deletada com sucesso id={Id}", id);
                    return NoContent();
                }
                else
                {
                    _logger.LogInformation("Tarefa não encontrada para deleção id={Id}", id);
                    return NotFound(new { Erro = "ID não encontrado, nenhuma tarefa foi deletada." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro interno ao deletar tarefa id={Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno ao processar a requisição.");
            }
        }
    }
}

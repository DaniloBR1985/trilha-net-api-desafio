using TrilhaApiDesafio.Models;
using TrilhaApiDesafio.Repositories;

namespace TrilhaApiDesafio.Services
{
    public class TarefaService : ITarefaService
    {
        private readonly ITarefaRepository _repository;

        public TarefaService(ITarefaRepository repository)
        {
            _repository = repository;
        }

        public Tarefa ObterPorId(int id)
            => _repository.ObterPorId(id);

        public IEnumerable<Tarefa> ObterTodos()
            => _repository.ObterTodos();

        public IEnumerable<Tarefa> ObterPorTitulo(string titulo)
            => _repository.ObterPorTitulo(titulo);

        public IEnumerable<Tarefa> ObterPorData(DateTime data)
            => _repository.ObterPorData(data);

        public IEnumerable<Tarefa> ObterPorStatus(EnumStatusTarefa status)
            => _repository.ObterPorStatus(status);

        public ResultadoMetodo Criar(Tarefa tarefa)
        {
            if (string.IsNullOrWhiteSpace(tarefa.Titulo))
                return ResultadoMetodo.Falha("O título da tarefa é obrigatório.");

            if (_repository.ExistePorTituloEData(tarefa.Titulo, tarefa.Data))
                return ResultadoMetodo.Falha("Já existe uma tarefa com o mesmo título e data.");

            _repository.Criar(tarefa);
            return ResultadoMetodo.Ok();
        }

        public ResultadoMetodo Atualizar(int id, Tarefa tarefa)
        {
            var tarefaBanco = _repository.ObterPorId(id);
            if (tarefaBanco == null)
                return ResultadoMetodo.Falha("Tarefa não encontrada.");

            if (string.IsNullOrWhiteSpace(tarefa.Titulo))
                return ResultadoMetodo.Falha("O título da tarefa é obrigatório.");

            if (_repository.ExistePorTituloEData(tarefa.Titulo, tarefa.Data, id))
                return ResultadoMetodo.Falha("Já existe outra tarefa com o mesmo título e data.");

            tarefaBanco.Titulo = tarefa.Titulo;
            tarefaBanco.Descricao = tarefa.Descricao;
            tarefaBanco.Data = tarefa.Data;
            tarefaBanco.Status = tarefa.Status;

            _repository.Atualizar(tarefaBanco);
            return ResultadoMetodo.Ok();
        }

        public bool Deletar(int id)
        {
            var tarefa = _repository.ObterPorId(id);
            if (tarefa != null)
            {
                _repository.Deletar(tarefa);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

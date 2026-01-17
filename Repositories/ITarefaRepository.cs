using TrilhaApiDesafio.Models;

namespace TrilhaApiDesafio.Repositories
{
    public interface ITarefaRepository
    {
        Tarefa ObterPorId(int id);
        IEnumerable<Tarefa> ObterTodos();
        IEnumerable<Tarefa> ObterPorTitulo(string titulo);
        IEnumerable<Tarefa> ObterPorData(DateTime data);
        IEnumerable<Tarefa> ObterPorStatus(EnumStatusTarefa status);
        void Criar(Tarefa tarefa);
        void Atualizar(Tarefa tarefa);
        void Deletar(Tarefa tarefa);
        bool ExistePorTituloEData(string titulo, DateTime data, int? idIgnorar = null);
    }
}

using TrilhaApiDesafio.Models;

namespace TrilhaApiDesafio.Services
{
    public interface ITarefaService
    {
        Tarefa ObterPorId(int id);
        IEnumerable<Tarefa> ObterTodos();
        IEnumerable<Tarefa> ObterPorTitulo(string titulo);
        IEnumerable<Tarefa> ObterPorData(DateTime data);
        IEnumerable<Tarefa> ObterPorStatus(EnumStatusTarefa status);
        ResultadoMetodo Criar(Tarefa tarefa);
        ResultadoMetodo Atualizar(int id, Tarefa tarefa);
        bool Deletar(int id);
    }
}

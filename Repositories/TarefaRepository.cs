using TrilhaApiDesafio.Context;
using TrilhaApiDesafio.Models;

namespace TrilhaApiDesafio.Repositories
{
    public class TarefaRepository : ITarefaRepository
    {
        private readonly OrganizadorContext _context;

        public TarefaRepository(OrganizadorContext context)
        {
            _context = context;
        }

        public Tarefa ObterPorId(int id)
            => _context.Tarefas.Find(id);

        public IEnumerable<Tarefa> ObterTodos()
            => _context.Tarefas.ToList();

        public IEnumerable<Tarefa> ObterPorTitulo(string titulo)
            => _context.Tarefas.Where(x => x.Titulo.Contains(titulo));

        public IEnumerable<Tarefa> ObterPorData(DateTime data)
            => _context.Tarefas.Where(x => x.Data.Date == data.Date);

        public IEnumerable<Tarefa> ObterPorStatus(EnumStatusTarefa status)
            => _context.Tarefas.Where(x => x.Status == status);

        public void Criar(Tarefa tarefa)
        {
            _context.Tarefas.Add(tarefa);
            _context.SaveChanges();
        }

        public void Atualizar(Tarefa tarefa)
        {
            _context.Tarefas.Update(tarefa);
            _context.SaveChanges();
        }

        public void Deletar(Tarefa tarefa)
        {
            _context.Tarefas.Remove(tarefa);
            _context.SaveChanges();
        }

        public bool ExistePorTituloEData(string titulo, DateTime data, int? idIgnorar = null)
        {
            return _context.Tarefas.Any(x =>
                x.Titulo == titulo &&
                x.Data.Date == data.Date &&
                (!idIgnorar.HasValue || x.Id != idIgnorar.Value));
        }
    }
}

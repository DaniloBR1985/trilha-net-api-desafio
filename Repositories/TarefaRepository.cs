using Microsoft.EntityFrameworkCore;
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
        {
            if (id <= 0)
                return null;

            return _context.Tarefas
                           .AsNoTracking()
                           .FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<Tarefa> ObterTodos()
            => _context.Tarefas
                       .AsNoTracking()
                       .OrderBy(x => x.Data)
                       .ToList();

        public IEnumerable<Tarefa> ObterPorTitulo(string titulo)
            => _context.Tarefas
                       .AsNoTracking()
                       .Where(x => EF.Functions.Like(x.Titulo, $"%{titulo}%"))
                       .OrderBy(x => x.Data)
                       .ToList();

        public IEnumerable<Tarefa> ObterPorData(DateTime data)
        {
            var inicioDoDia = data.Date;
            var inicioProximoDia = inicioDoDia.AddDays(1);

            return _context.Tarefas
                           .AsNoTracking()
                           .Where(x => x.Data >= inicioDoDia && x.Data < inicioProximoDia)
                           .OrderBy(x => x.Data)
                           .ToList();
        }

        public IEnumerable<Tarefa> ObterPorStatus(EnumStatusTarefa status)
            => _context.Tarefas
                       .AsNoTracking()
                       .Where(x => x.Status == status)
                       .OrderBy(x => x.Data)
                       .ToList();

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

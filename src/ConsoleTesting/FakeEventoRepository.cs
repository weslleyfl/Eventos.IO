using Eventos.IO.Domain.EventosRoot;
using Eventos.IO.Domain.EventosRoot.Repository;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ConsoleTesting
{
    partial class Program
    {
        public class FakeEventoRepository : IEventoRepository
        {
            public void Add(Evento obj)
            {
                //
            }

            public void Adicionar(Evento obj)
            {
                throw new NotImplementedException();
            }

            public void AdicionarEndereco(Endereco endereco)
            {
                throw new NotImplementedException();
            }

            public void Atualizar(Evento obj)
            {
                throw new NotImplementedException();
            }

            public void AtualizarEndereco(Endereco endereco)
            {
                throw new NotImplementedException();
            }

            public IEnumerable<Evento> Buscar(Expression<Func<Evento, bool>> predicate)
            {
                throw new NotImplementedException();
            }

            public void Dispose()
            {
                throw new NotImplementedException();
            }

            public IEnumerable<Evento> Find(Expression<Func<Evento, bool>> predicate)
            {
                throw new NotImplementedException();
            }

            public IEnumerable<Evento> GetAll()
            {
                throw new NotImplementedException();
            }

            public Evento GetById(Guid id)
            {
                return new Evento("FakeEvento", DateTime.Now, DateTime.Now, true, 1, true, "Empresa fake");
            }

            public IEnumerable<Categoria> ObterCategorias()
            {
                throw new NotImplementedException();
            }

            public Endereco ObterEnderecoPorId(Guid id)
            {
                throw new NotImplementedException();
            }

            public IEnumerable<Evento> ObterEventoPorOrganizador(Guid organizadorId)
            {
                throw new NotImplementedException();
            }

            public Evento ObterMeuEventoPorId(Guid id, Guid organizadorId)
            {
                throw new NotImplementedException();
            }

            public Evento ObterPorId(Guid id)
            {
                throw new NotImplementedException();
            }

            public IEnumerable<Evento> ObterTodos()
            {
                throw new NotImplementedException();
            }

            public void Remove(Guid id)
            {
                //
            }

            public void Remover(Guid id)
            {
                throw new NotImplementedException();
            }

            public int SaveChanges()
            {
                throw new NotImplementedException();
            }

            public void Update(Evento obj)
            {
                //
            }
        }

    }
}

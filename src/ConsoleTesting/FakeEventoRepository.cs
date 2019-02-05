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

            public void Remove(Guid id)
            {
                //
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

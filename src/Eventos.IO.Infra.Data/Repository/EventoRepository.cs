using Eventos.IO.Domain.EventosRoot;
using Eventos.IO.Domain.EventosRoot.Repository;
using Eventos.IO.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eventos.IO.Infra.Data.Repository
{
    public class EventoRepository : Repository<Evento>, IEventoRepository
    {
        public EventoRepository(EventosContext context) : base(context)
        {

        }

        public void AdicionarEndereco(Endereco endereco)
        {
            Db.Enderecos.Add(endereco);
        }

        public void AtualizarEndereco(Endereco endereco)
        {
            Db.Enderecos.Update(endereco);
        }

        public IEnumerable<Categoria> ObterCategorias()
        {
            throw new NotImplementedException();
        }

        public Endereco ObterEnderecoPorId(Guid id)
        {
            return Db.Enderecos.Find(id);
        }

        public override Evento ObterPorId(Guid id)
        {
            return Db.Eventos.Include(e => e.Endereco).FirstOrDefault(e => e.Id == id);
        }


        public IEnumerable<Evento> ObterEventoPorOrganizador(Guid organizadorId)
        {
            return Db.Eventos.Where(e => e.OrganizadorId == organizadorId);
        }

        public Evento ObterMeuEventoPorId(Guid id, Guid organizadorId)
        {
            throw new NotImplementedException();
        }


    }
}

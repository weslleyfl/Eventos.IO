﻿using Dapper;
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
            var sql = "SELECT * FROM Categorias";

            IEnumerable<Categoria> categorias = Db.Database.GetDbConnection().Query<Categoria>(sql);

            return categorias;
        }

        public Endereco ObterEnderecoPorId(Guid id)
        {
            var sql = @"SELECT * FROM Enderecos E " +
                      "WHERE E.Id = @uid";

            IEnumerable<Endereco> enderecos = Db.Database.GetDbConnection().Query<Endereco>(sql, new { uid = id });

            return enderecos.FirstOrDefault();

            //return Db.Enderecos.Find(id);
        }

        public override Evento ObterPorId(Guid id)
        {
            var sql = @"SELECT * FROM Eventos E " +
                     "LEFT JOIN Enderecos EN " +
                     "ON E.Id = EN.EventoId " +
                     "WHERE E.Id = @uid";

            var evento = Db.Database.GetDbConnection().Query<Evento, Endereco, Evento>(sql,
                (e, end) =>
                {
                    if (end != null)
                        e.AtribuirEndereco(end);

                    return e;
                }, new { uid = id });

            return evento.FirstOrDefault();

            //return Db.Eventos.Include(e => e.Endereco).FirstOrDefault(e => e.Id == id);
        }

        public IEnumerable<Evento> ObterEventoPorOrganizador(Guid organizadorId)
        {
            var sql = @"SELECT * FROM EVENTOS E " +
                       "WHERE E.EXCLUIDO = 0 " +
                       "AND E.ORGANIZADORID = @oid " +
                       "ORDER BY E.DATAFIM DESC";

            return Db.Database.GetDbConnection().Query<Evento>(sql, new { oid = organizadorId });

            //return Db.Eventos.Where(e => e.OrganizadorId == organizadorId);
        }

        public Evento ObterMeuEventoPorId(Guid id, Guid organizadorId)
        {
            var sql = @"SELECT * FROM EVENTOS E " +
                      "LEFT JOIN Enderecos EN " +
                      "ON E.Id = EN.EventoId " +
                      "WHERE E.EXCLUIDO = 0 " +
                      "AND E.ORGANIZADORID = @oid " +
                      "AND E.ID = @eid";

            var evento = Db.Database.GetDbConnection().Query<Evento, Endereco, Evento>(sql,
                    (e, en) =>
                    {
                        if (en != null)
                            e.AtribuirEndereco(en);
                        return e;
                    }, 
                    new { oid = organizadorId, eid = id });

            return evento.FirstOrDefault();
        }

        public override IEnumerable<Evento> ObterTodos()
        {
            var sql = "SELECT * FROM EVENTOS E " +
                    "WHERE E.EXCLUIDO = 0 " +
                    "ORDER BY E.DATAFIM DESC ";

            return Db.Database.GetDbConnection().Query<Evento>(sql);
        }

        public override void Remover(Guid id)
        {
            var evento = ObterPorId(id);
            evento.ExcluirEvento();
            Atualizar(evento);
        }


    }
}

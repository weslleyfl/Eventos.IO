﻿using Eventos.IO.Domain.OrganizadoresRoot;
using Eventos.IO.Domain.OrganizadoresRoot.Repository;
using Eventos.IO.Infra.Data.Context;
using System;
using System.Collections.Generic;
using System.Text;

namespace Eventos.IO.Infra.Data.Repository
{
    public class OrganizadorRepository : Repository<Organizador>, IOrganizadorRepository
    {
        public OrganizadorRepository(EventosContext context) : base(context)
        {
        }
    }
}

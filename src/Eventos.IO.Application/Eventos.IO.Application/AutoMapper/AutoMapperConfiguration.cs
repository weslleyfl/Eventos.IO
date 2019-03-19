using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;

namespace Eventos.IO.Application.AutoMapper
{
    public class AutoMapperConfiguration
    {
        //public static MapperConfiguration RegisterMappings()
        //{
        //    return new MapperConfiguration( ps =>
        //    {
        //        ps.AddProfile(new DomainToViewModelMappingProfile());
        //        ps.AddProfile(new ViewModelToDomainMappingProfile());

        //    });
        //}

        public static void RegisterMappings()
        {          
            Mapper.Initialize(x =>
            {
                x.AddProfile<DomainToViewModelMappingProfile>();
                x.AddProfile<ViewModelToDomainMappingProfile>();
            });
        }
    }
}

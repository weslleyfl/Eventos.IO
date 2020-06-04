using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace Eventos.IO.Services.Api.AutoMapper
{
    /// <summary>
    /// https://code-maze.com/automapper-net-core/
    /// </summary>
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            // CreateMap<User, UserViewModel>();
            //CreateMap<User, UserViewModel>()
            //   .ForMember(dest =>
            //       dest.FName,
            //       opt => opt.MapFrom(src => src.FirstName))
            //   .ForMember(dest =>
            //       dest.LName,
            //       opt => opt.MapFrom(src => src.LastName))
        }

        //public void AddAutoMapperConfig(IServiceCollection services)
        //{
        //    var mapperConfiguration = new MapperConfiguration(config =>
        //    {

        //        //config.CreateMap<,>();

        //    });

        //    IMapper mapper = mapperConfiguration.CreateMapper();
        //    services.AddSingleton(mapper);
        //}

    }
}

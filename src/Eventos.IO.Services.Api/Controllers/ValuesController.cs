using Eventos.IO.Domain.Core.Bus;
using Eventos.IO.Domain.Core.Notifications;
using Eventos.IO.Domain.Interfaces;
using Eventos.IO.Infra.CrossCutting.Identity.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Eventos.IO.Services.Api.Controllers
{
    /// <summary>
    /// https://medium.com/@nelson.souza/net-core-api-versioning-d4f869fb9052
    /// https://www.blogofpi.com/versioning-web-api/
    /// </summary>
    //[Route("api/v{version:apiVersion}/[controller]")]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]    
    [Route("api/v{version:apiVersion}")]
    [ApiController]
    [ApiVersion("1", Deprecated = true)] // futura API depreciada
    [ApiVersion("2")]
    public class ValuesController : BaseController
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<ValuesController> _logger;
        private readonly IUser _user;
        private readonly IHttpContextAccessor _accessor;

        public ValuesController(IDomainNotificationHandler<DomainNotification> notifications,
                                IUser user,
                                IBus bus,
                                IHttpContextAccessor accessor,
                                SignInManager<ApplicationUser> signInManager,
                                ILogger<ValuesController> logger) : base(notifications, user, bus)
        {
            _signInManager = signInManager;
            _logger = logger;
            _user = user;
            _accessor = accessor;

            var nome = _user.IsAuthenticated();
            var valor = accessor.HttpContext.User.Identities;
        }


        // GET api/values
        [HttpGet]
        [Route("values")]       
        [Authorize(Policy = "PodeLerEventos")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "PodeLerEventos")]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2", _user.Name };

        }

        // overall route: /api/v{version}/values/{id}
        [HttpGet()]       
        [Route("valor/{id}")]        
        [Authorize(Policy = "PodeGravar")]
        [MapToApiVersion("2")] // v2.0 specific action for GET api/values endpoint
        [ApiExplorerSettings(GroupName = "v2")]
        public ActionResult<string> GetById(int id)
        {
            // ApiVersion.Default.MajorVersion
            //do stuff with version and id
            return $"item: {id}, version: { User.Identity.Name }";
        }

        [HttpGet()]
        [Route("sair/{returnUrl?}")]
        [AllowAnonymous]
        public async Task<string> Logout(string returnUrl = null)
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");

            if (returnUrl != null)
            {
                return returnUrl;
            }

            return "User logged out";
        }

        // GET api/values/5
        //[HttpGet("{id}")]
        //public ActionResult<string> Get(int id)
        //{
        //    return "value";
        //}

        // POST api/values
        //[HttpPost]
        //public void Post([FromBody] string value)
        //{
        //}

        //// PUT api/values/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE api/values/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}

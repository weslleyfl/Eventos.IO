using Eventos.IO.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Eventos.IO.Site.Controllers
{
    public class ErrosController : Controller
    {
        private readonly IUser _user;

        public ErrosController(IUser user)
        {
            _user = user;
        }

        [Route("/erro-de-aplicacao")]
        [Route("/erro-de-aplicacao/{id}")]
        public IActionResult Erros(string id)
        {

            switch (id)
            {
                case "404": return View("NotFound");
                case "403": // ForbidResult();
                case "401": // nao logado
                    if (!_user.IsAuthenticated())
                        return LocalRedirect("/Identity/Account/Login");
                    return LocalRedirect("/Identity/Account/AccessDenied");
                    // return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            return View("Error");
        }
    }
}
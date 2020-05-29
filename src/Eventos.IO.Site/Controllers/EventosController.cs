using AutoMapper;
using Eventos.IO.Application.Interfaces;
using Eventos.IO.Application.ViewModels;
using Eventos.IO.Domain.Core.Notifications;
using Eventos.IO.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Eventos.IO.Site.Controllers
{
    [Route("")]  
    public class EventosController : BaseController
    {
        // private readonly ApplicationDbContext _context;
        private readonly IEventoAppService _eventoAppService;

        public EventosController(IEventoAppService eventoAppService,
                                IDomainNotificationHandler<DomainNotification> notifications,
                                IUser user
                                ) : base(notifications, user)
        {
            //_context = context;
            _eventoAppService = eventoAppService;
        }


        // GET: Eventos
        [AllowAnonymous]
        [HttpGet()]
        [Route("")]
        [Route("proximos-eventos")]       
        public IActionResult Index()
        {
            return View(_eventoAppService.ObterTodos());
        }


        [HttpGet()]
        [Route("meus-eventos")]
        [Authorize(Policy = "PodeLerEventos")]
        public IActionResult MeusEventos()
        {                
            return View(_eventoAppService.ObterEventoPorOrganizador(OrganizadorId));
        }

        // GET: Eventos/Details/5
        [AllowAnonymous]
        [HttpGet()]
        [Route("dados-do-evento/{id:guid}")]
        public IActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventoViewModel = _eventoAppService.ObterPorId(id.Value);

            if (eventoViewModel == null)
            {
                return NotFound();
            }

            return View(eventoViewModel);
        }

        // GET: Eventos/Create
        [HttpGet()]
        [Route("novo-evento")]
        [Authorize(Policy = "PodeGravar")]
        public IActionResult Create()
        {
            // ViewData["CategoriaId"] = new SelectList(_context.Set<CategoriaViewModel>(), "Id", "Id");
            return View();
        }

        // POST: Eventos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("novo-evento")]
        [Authorize(Policy = "PodeGravar")]
        public IActionResult Create(EventoViewModel eventoViewModel)
        {
            if (ModelState.IsValid == false) return View(eventoViewModel);

            eventoViewModel.OrganizadorId = OrganizadorId;
            _eventoAppService.Registrar(eventoViewModel);

            ViewBag.RetornoPost = OperacaoValida() ? "success,Evento registrado com sucesso!" : "error,Evento não registrado! Verifique as mensagens";
        
            return View(eventoViewModel);
        }

        // GET: Eventos/Edit/5
        [HttpGet()]
        [Route("editar-evento/{id:guid}")]
        [Authorize(Policy = "PodeGravar")]
        public IActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventoViewModel = _eventoAppService.ObterPorId(id.Value);

            if (eventoViewModel == null)
            {
                return NotFound();
            }

            if (ValidarAutoridadeEvento(eventoViewModel))
            {
                return RedirectToAction("MeusEventos", _eventoAppService.ObterEventoPorOrganizador(OrganizadorId));
            }

            return View(eventoViewModel);
        }

        // POST: Eventos/Edit/5     
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("editar-evento/{id:guid}")]
        [Authorize(Policy = "PodeGravar")]
        public IActionResult Edit(EventoViewModel eventoViewModel)
        {

            if (!ModelState.IsValid) return View(eventoViewModel);

            eventoViewModel.OrganizadorId = OrganizadorId;

            _eventoAppService.Atualizar(eventoViewModel);

            ViewBag.RetornoPost = OperacaoValida() ? "success,Evento atualizado com sucesso!" : "error,Evento não pode ser atualizado! Verifique as mensagens";

            var eventoView = _eventoAppService.ObterPorId(eventoViewModel.Id);

            if (eventoView.Online)
            {
                eventoViewModel.Endereco = null;
            }
            else
            {
                eventoViewModel = eventoView;
            }

            return View(eventoViewModel);
        }

        // GET: Eventos/Delete/5
        [HttpGet()]
        [Route("excluir-evento/{id:guid}")]
        [Authorize(Policy = "PodeGravar")]
        public IActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventoViewModel = _eventoAppService.ObterPorId(id.Value);

            if (eventoViewModel == null)
            {
                return NotFound();
            }

            if (ValidarAutoridadeEvento(eventoViewModel))
            {
                return RedirectToAction("MeusEventos", _eventoAppService.ObterEventoPorOrganizador(OrganizadorId));
            }

            return View(eventoViewModel);
        }

        // POST: Eventos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Route("excluir-evento/{id:guid}")]
        [Authorize(Policy = "PodeGravar")]
        public IActionResult DeleteConfirmed(Guid id)
        {
            if (ValidarAutoridadeEvento(_eventoAppService.ObterPorId(id)))
            {
                return RedirectToAction("MeusEventos", _eventoAppService.ObterEventoPorOrganizador(OrganizadorId));
            }

            _eventoAppService.Excluir(id);

            return RedirectToAction("Index");
        }

        [HttpGet()]
        [Route("incluir-endereco/{id:guid}")]
        [Authorize(Policy = "PodeGravar")]
        public IActionResult IncluirEndereco(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventoViewModel = _eventoAppService.ObterPorId(id.Value);
            return PartialView("_IncluirEndereco", eventoViewModel);
        }

        [HttpPost()]
        [ValidateAntiForgeryToken]
        [Route("incluir-endereco/{id:guid}")]
        [Authorize(Policy = "PodeGravar")]
        public IActionResult IncluirEndereco(EventoViewModel eventoViewModel)
        {
            ModelState.Clear();

            eventoViewModel.Endereco.EventoId = eventoViewModel.Id;
            _eventoAppService.AdicionarEndereco(eventoViewModel.Endereco);

            if (OperacaoValida())
            {
                var url = Url.Action("ObterEndereco", "Eventos", new { id = eventoViewModel.Id });
                return Json(new { success = true, url = url });
            }

            return PartialView("_IncluirEndereco", eventoViewModel);
        }

        [HttpGet()]
        [Route("atualizar-endereco/{id:guid}")]
        [Authorize(Policy = "PodeGravar")]
        public IActionResult AtualizarEndereco(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventoViewModel = _eventoAppService.ObterPorId(id.Value);
            return PartialView("_AtualizarEndereco", eventoViewModel);
        }

        //[Authorize(Policy = "PodeGravar")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("atualizar-endereco/{id:guid}")]
        [Authorize(Policy = "PodeGravar")]
        public IActionResult AtualizarEndereco(EventoViewModel eventoViewModel)
        {
            ModelState.Clear();
            _eventoAppService.AtualizarEndereco(eventoViewModel.Endereco);

            if (OperacaoValida())
            {
                var url = Url.Action("ObterEndereco", "Eventos", new { id = eventoViewModel.Id });
                return Json(new { success = true, url = url });
            }

            return PartialView("_AtualizarEndereco", eventoViewModel);
        }

        [HttpGet()]
        [Route("listar-endereco/{id:guid}")]
        public IActionResult ObterEndereco(Guid id)
        {
            return PartialView("_DetalhesEndereco", _eventoAppService.ObterPorId(id));
        }

        private bool ValidarAutoridadeEvento(EventoViewModel eventoViewModel)
        {
            return eventoViewModel.OrganizadorId != OrganizadorId;
        }

    }
}

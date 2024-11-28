using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Bibliotec.Contexts;
using Bibliotec.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Bibliotec_mvc.Controllers
{
    [Route("[controller]")]
    public class UsuarioController : Controller
    {
        private readonly ILogger<UsuarioController> _logger;

        public UsuarioController(ILogger<UsuarioController> logger)
        {
            _logger = logger;
        }

        //criando um obj da classe contex:
        Context context = new Context();

        //o metodo index esta retornando a view usuario/index.cshtml

        public IActionResult Index()
        {
            //pegar as informacoes da sessionque sao necessarias para que apareca os detalhes do meu usuario

            int id = int.Parse(HttpContext.Session.GetString("UsuarioID"));
            ViewBag.Admin = HttpContext.Session.GetString("Admin");

            //busquei o usuario que esta logado(Beatriz)
            Usuario usuarioEncontrado = context.Usuario.FirstOrDefault(usuario => usuario.UsuarioID == id)!;
            // se naofor encontrado ninguem
            if (usuarioEncontrado == null)
            {
                return NotFound();
            }

            // procurar o curso que meu usuario encontrado esta cadastrado
            Curso cursoEncontrado = context.Curso.FirstOrDefault(curso => curso.CursoID == usuarioEncontrado.CursoID)!;


            //verificar se o usuario nao possui curso
            if (cursoEncontrado == null)
            {

                // preciso que vc mande essas mensagem para a View:
                ViewBag.Curso = "O usuário nao possui curso cadastrado";
            }
            else
            {
                //O usuario possui o curso xxx
                ViewBag.Curso = cursoEncontrado.Nome;
            }

            ViewBag.Nome = usuarioEncontrado.Nome;
            ViewBag.Email = usuarioEncontrado.Email;
            ViewBag.Telefone = usuarioEncontrado.Contato;
            ViewBag.DtNascimento = usuarioEncontrado.DtNascimento.ToString("dd/MM/yyyy");


            return View();
        }

        // [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        // public IActionResult Error()
        // {
        //     return View("Error!");
        // }
    }
}
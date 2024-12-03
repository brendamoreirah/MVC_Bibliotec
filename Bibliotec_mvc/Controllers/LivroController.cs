using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Bibliotec.Contexts;
using Bibliotec.Models;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Bibliotec_mvc.Controllers
{
    [Route("[controller]")]
    public class LivroController : Controller
    {
        private readonly ILogger<LivroController> _logger;

        public LivroController(ILogger<LivroController> logger)
        {
            _logger = logger;
        }

        Context context = new Context();

        public IActionResult Index()
        {
            ViewBag.Admin = HttpContext.Session.GetString("Admin");

            //criar uma lista de livros
            List<Livro> listaLivros = context.Livro.ToList();

            //Verificar se o livro tem reserva ou nao
            var livrosReservados = context.LivroReserva.ToDictionary(livro => livro.LivroID, livror => livror.DtReserva);

            ViewBag.Livros = listaLivros;
            ViewBag.LivrosComReserva = livrosReservados;

            return View();
        }


        [Route("Cadastro")]
        //metodo que retorna a tela de cadastro
        public IActionResult Cadastro()
        {
            ViewBag.Admin = HttpContext.Session.GetString("Admin");

            ViewBag.Categorias = context.Categoria.ToList();

            //retorna a view de cadastro:
            return View();
        }

        //metodo para cadastrar um livro:

        [Route("Cadastrar")]
        public IActionResult Cadastrar(IFormCollection form)
        {
            Livro novoLivro = new Livro();

            //o que meu usuario escrever no formulario sera atribuido ao novoLivro

            novoLivro.Nome = form["Nome"].ToString();
            novoLivro.Descricao = form["Descricao"].ToString();
            novoLivro.Nome = form["Editora"].ToString();
            novoLivro.Nome = form["Escritor"].ToString();
            novoLivro.Nome = form["Idioma"].ToString();
            //trabalhar com imagens:
            if (form.Files.Count > 0)
            {
                //Primeiro passo
                //Armazenar o arquivo/foto enviado pelo usuario
                var arquivo = form.Files[0];

                //segundo passo
                //criar variavel do caminho da minha pasta para colocar as fotos dos livros
                var pasta = Path.Combine(Directory.GetCurrentDirectory(), "wwwrooot/images/Livros");
                //Validaremos se a pasta que sera armazenada as imagen, existe.Caso nao exista, criaremos uma nova pasta


                if (!Directory.Exists(pasta))
                {
                    //criar pasta
                    Directory.CreateDirectory(pasta);
                }

                //terceiro passo:
                //criar a variavel para armazenar o caminho em que meu arquivo estara, alem do nome dele
                var caminho = Path.Combine(pasta, arquivo.FileName);

                using (var stream = new FileStream(caminho, FileMode.Create)){
                    arquivo.CopyTo(stream);
                }

                novoLivro.Imagem = arquivo.FileName;
            }else{
                novoLivro.Imagem = "padrao.png";
            }

            context.Livro.Add(novoLivro);
            context.SaveChanges();

            //SEGUNDA PARTE/ é adicionar dentro da LivroCategoria a categoria que pertence ao novoLivro
            //lista as categorias
            List<LivroCategoria> livroCategorias = new List<LivroCategoria>();

            //arrays  que possui as categorias selecionadas pelo usuario
            string[] categoriasSelecionadas = form["Categoria"].ToString().Split(',');
            //Acao,terror, suspense 
            //1,5,7


            // Categoria possuia informacao do id da categoria ATUAL selecionada
            foreach (string categoria in categoriasSelecionadas)
            {
                LivroCategoria livroCategoria = new LivroCategoria();

                livroCategoria.CategoriaID = int.Parse(categoria);
                livroCategoria.LivroID = novoLivro.LivroID;
                //adicionamos o obj livrocategoria dentro da listaLivro
                livroCategorias.Add(livroCategoria);
            }

            //peguei a colecao da livrocategoria e coloquei na tabela Livro Categoria
            context.LivroCategoria.AddRange(livroCategorias);
            context.SaveChanges();

            return LocalRedirect("/Livro/Cadastro");


        }


        // [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        // public IActionResult Error()
        // {
        //     return View("Error!");
        // }
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
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
        private Stream stream;

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

                using (var stream = new FileStream(caminho, FileMode.Create))
                {
                    arquivo.CopyTo(stream);
                }

                novoLivro.Imagem = arquivo.FileName;
            }
            else
            {
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

        [Route("Editar/{id}")]
        public IActionResult Editar(int id)
        {

            ViewBag.Admin = HttpContext.Session.GetString("Admin");

            ViewBag.CategoriasDoSistema = context.Categoria.ToList();

            //Buscar quem é o tal do id numero 3:
            Livro livroEncontrada = context.Livro.FirstOrDefault(livro => livro.LivroID == id)!; ;

            //Buscar as categorias que o livroEncontrado possui
            var categoriasDoLivroEncontrado = context.LivroCategoria
            .Where(identificadorLivro => identificadorLivro.LivroID == id)
            .Select(livro => livro.Categoria)
            .ToList();


            //quero pegar as informacoes e mandar para minha View
            ViewBag.Livro = livroEncontrada;
            ViewBag.Categoria = categoriasDoLivroEncontrado;

            return View();
        }

        //criar um metodo que atualiza as informacoes do livro
        [Route("Atualizar/{id}")]
        public IActionResult Atualizar(IFormCollection form, int id, IFormFile imagem)
        {
            //buscar um livro especifico pelo id
            Livro livroAtualizado = context.Livro.FirstOrDefault(livro => livro.LivroID == id)!;
            livroAtualizado.Nome = form["Nome"];
            livroAtualizado.Escritor = form["Escritor"];
            livroAtualizado.Editora = form["Editora"];
            livroAtualizado.Editora = form["Idioma"];
            livroAtualizado.Editora = form["Descricao"];

            //upload de imagem
            if (imagem != null && imagem.Length > 0)
            {
                //definir o caminho da minha imagem do livro ATUAL que eu quero alterar:
                var caminhoImagem = Path.Combine("wwwroot/images/Livros", imagem.FileName);

                //verificar se minha imagem ainda exite no meu caminho

                if (!string.IsNullOrEmpty(livroAtualizado.Imagem))
                {
                    //Caso exista, ela irá ser apagada
                    var caminhoImagemAntiga = Path.Combine("wwwroot/images/Livros", livroAtualizado.Imagem);
                    //ver se existe uma imagem no caminho antigo
                    if (System.IO.File.Exists(caminhoImagemAntiga))
                    {
                        System.IO.File.Delete(caminhoImagemAntiga);
                    }
                }

                //salvar a imagem nova
                using (var strem = new FileStream(caminhoImagem, FileMode.Create))
                {
                    imagem.CopyTo(stream);
                }

                //subir essa mudanca para o meu banco de dados
                livroAtualizado.Imagem = imagem.FileName;
            }

            //categorias:

            //primeiro:precisamos pegar as categorias selecionadas do usuario
            var categoriasSelecionadas = form["Categoria"].ToList();
            //segundo:pegaremosas categorias ATUAIS do livro
            var categoriasAtuais = context.LivroCategoria.Where(livro => livro.LivroID == id).ToList();
            //terceiro:Removeremos as cateorias antigas
            foreach (var categoria in categoriasAtuais)
            {
                if (!categoriasSelecionadas.Contains(categoria.CategoriaID.ToString()))
                {
                    //nos vamos remover a categorias do nosso contexts
                    context.LivroCategoria.Remove(categoria);
                }
            }
            //quarto:adicionaremos as novas categorias
            foreach (var categoria in categoriasSelecionadas)
            {
                //verificando se nao existe a categoria nesse livro
                if (!categoriasAtuais.Any(c => c.CategoriaID.ToString() == categoria))
                {
                    context.LivroCategoria.Add(new LivroCategoria
                    {
                        LivroID = id,
                        CategoriaID = int.Parse(categoria)



                    });
                }

            }
            context.SaveChanges();

            return LocalRedirect("/Livro");

        }

        //metodo de excluir o livro
        [Route("Excluir/{id}")]
        public IActionResult Excluir(int id){
         //buscar qual o livro do id que precisamos excluir
         Livro livroEncontrado = context.Livro.First(livro => livro.LivroID == id);

         //buscar as categorias desse livro
         var categoriasDoLivro = context.LivroCategoria.Where(livro => livro.LivroID == id).ToList();

         //precisa exluir primewiro o registro da tabela intermediaria
         foreach(var categoria in categoriasDoLivro){
            context.LivroCategoria.Remove(categoria);
         }

         context.Livro.Remove(livroEncontrado);

         context.SaveChanges();

            return LocalRedirect("/Livro");
        }

        // [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        // public IActionResult Error()
        // {
        //     return View("Error!");
        // }
    }
}
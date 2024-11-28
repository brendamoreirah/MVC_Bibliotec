using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Bibliotec.Models
{
    public class LivroReserva
    {
        //Na tabela Livro Reserva temos 6 atributos
            	//1 PK
                //2 FK
        [Key]
        public int LivroReservaID { get; set; }
        public DateOnly DtReserva { get; set; }
        public DateOnly DtDevolucao { get; set; }
        public int Status { get; set; }

        [ForeignKey("Usuario")]
        public int UsuarioID { get; set; }
        public Usuario Usuario {get;set;}

        [ForeignKey("Livro")]
        public int LivroID { get; set; }
        public Livro Livro {get;set;}
    }
}
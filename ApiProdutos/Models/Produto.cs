using MongoDB.Bson;
using System;
using System.ComponentModel.DataAnnotations;

namespace ApiProdutos.Models
{
    public class Produto
    {
        public ObjectId _id { get; set; }

        [Required(ErrorMessage = "O código é obrigatório.")]
        [Range(1, int.MaxValue, ErrorMessage = "O código é obrigatório.")]
        public int Codigo { get; set; }

        [Required(ErrorMessage = "O título é obrigatório.")]
        [MaxLength(120, ErrorMessage = "O título deve conter até 120 caracteres")]
        [MinLength(3, ErrorMessage = "O título deve conter no mínimo 3 caracteres")]
        public string Titulo { get; set; }

        public string Descricao { get; set; }

        [Required(ErrorMessage = "O valor é obrigatório.")]
        [Range(0.1, int.MaxValue, ErrorMessage = "O valor é obrigatório.")]
        public decimal Valor { get; set; }

        public int Quantidade { get; set; }
        
        public DateTime? DataEntrada { get; set; }

    }
}
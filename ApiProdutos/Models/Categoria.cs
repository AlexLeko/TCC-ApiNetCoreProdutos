using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections;
using System.Collections.Generic;

namespace ApiProdutos.Models
{
    public class Categoria
    {
        public ObjectId _id { get; set; }

        public string Titulo { get; set; }

        public IEnumerable<Produto> Produtos { get; set; }


    }
}

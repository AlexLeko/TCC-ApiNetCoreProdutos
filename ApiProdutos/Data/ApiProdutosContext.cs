using ApiProdutos.Data.Interface;
using ApiProdutos.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace ApiProdutos.Data
{
    public class ApiProdutosContext : IApiProdutoContext
    {
        private readonly IMongoDatabase _db;
        
        public ApiProdutosContext(IOptions<ConfigContext> options)
        {
            MongoClient client = new MongoClient(options.Value.ConnectionString);
            _db = client.GetDatabase(options.Value.Database);
        }

        public IMongoCollection<Produto> Produtos => _db.GetCollection<Produto>("ProdutosNetCore");

    }
}

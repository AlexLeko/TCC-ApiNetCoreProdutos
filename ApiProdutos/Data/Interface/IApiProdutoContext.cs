using ApiProdutos.Models;
using MongoDB.Driver;

namespace ApiProdutos.Data.Interface
{
    public interface IApiProdutoContext
    {
        IMongoCollection<Produto> Produtos { get; }

    }
}

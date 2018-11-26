using ApiProdutos.Data.Interface;
using ApiProdutos.Models;
using ApiProdutos.Repository.Interface;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiProdutos.Repository
{
    public class ProdutoRepository : IProdutoRepository
    {
        #region [IoC]

        private readonly IApiProdutoContext _context;


        public ProdutoRepository(IApiProdutoContext context)
        {
            _context = context;
        }

        #endregion [IoC]

        #region [Actions]

        public async Task<IEnumerable<Produto>> GetAll()
        {
            return await _context
                            .Produtos
                            .Find(_ => true)
                            .ToListAsync();
        }

        public Task<Produto> GetCodigo(int codigo)
        {
            FilterDefinition<Produto> filter = Builders<Produto>.Filter.Eq(m => m.Codigo, codigo);
            return _context
                    .Produtos
                    .Find(filter)
                    .FirstOrDefaultAsync();
        }

        public async Task Create(Produto produto)
        {
            await _context.Produtos.InsertOneAsync(produto);
        }

        public async Task<bool> Update(Produto produto)
        {
            ReplaceOneResult updateResult =
                await _context
                        .Produtos
                        .ReplaceOneAsync(
                            filter: g => g.Codigo == produto.Codigo,
                            replacement: produto);

            return updateResult.IsAcknowledged && updateResult.ModifiedCount > 0;
        }

        public async Task<bool> Delete(int codigo)
        {
            FilterDefinition<Produto> filter = Builders<Produto>.Filter.Eq(m => m.Codigo, codigo);
            DeleteResult deleteResult = await _context
                                                .Produtos
                                                .DeleteOneAsync(filter);

            return deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0;
        }

        public Task<Categoria> GetTitulo(string titulo)
        {
            FilterDefinition<Categoria> filter = Builders<Categoria>.Filter.Eq(m => m.Titulo, titulo);
            return _context
                    .Categorias
                    .Find(titulo)
                    .FirstOrDefaultAsync();
        }
    }

    #endregion [Actions]

}

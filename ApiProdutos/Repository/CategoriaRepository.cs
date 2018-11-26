using ApiProdutos.Data.Interface;
using ApiProdutos.Models;
using ApiProdutos.Repository.Interface;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiProdutos.Repository
{
    public class CategoriaRepository : ICategoriaRepository
    {
        #region [IoC]

        private readonly IApiProdutoContext _context;

        public CategoriaRepository(IApiProdutoContext context)
        {
            _context = context;
        }

        #endregion [IoC]

        #region [Actions]

        public Task<Categoria> GetTitulo(string titulo)
        {
            FilterDefinition<Categoria> filter = Builders<Categoria>.Filter.Eq(m => m.Titulo, titulo);
            return _context
                    .Categorias
                    .Find(titulo)
                    .FirstOrDefaultAsync();
        }

        public async Task Create(Categoria categoria)
        {
            await _context.Categorias.InsertOneAsync(categoria);
        }

    }

    #endregion [Actions]

}

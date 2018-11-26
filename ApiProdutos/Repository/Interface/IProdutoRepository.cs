using ApiProdutos.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiProdutos.Repository.Interface
{
    public interface IProdutoRepository
    {
        Task<IEnumerable<Produto>> GetAll();

        Task<Produto> GetCodigo(int codigo);

        Task Create(Produto produto);

        Task<bool> Update(Produto produto);

        Task<bool> Delete(int codigo);

        Task<Categoria> GetTitulo(string titulo);


    }
}

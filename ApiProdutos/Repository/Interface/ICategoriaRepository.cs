using ApiProdutos.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiProdutos.Repository.Interface
{
    public interface ICategoriaRepository
    {
        Task<Categoria> GetTitulo(string titulo);

        Task Create(Categoria categoria);

    }
}

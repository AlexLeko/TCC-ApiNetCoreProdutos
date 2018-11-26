using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiProdutos.Models.ViewModels
{
    public class ListaProdutosViewModel
    {
        #region [ PROPRIEDADES ]

        public int Codigo { get; set; }

        public string Titulo { get; set; }

        public decimal Valor { get; set; }

        public string Categoria { get; set; }

        #endregion [ PROPRIEDADES ]
    }
}

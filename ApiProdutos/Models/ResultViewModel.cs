using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiProdutos.Models
{
    public class ResultViewModel
    {
        public bool Sucess { get; set; }

        public string Message { get; set; }

        public object Data { get; set; }
    }
}

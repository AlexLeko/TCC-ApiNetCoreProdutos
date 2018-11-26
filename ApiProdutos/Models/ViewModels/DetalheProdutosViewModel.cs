using Flunt.Notifications;
using Flunt.Validations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiProdutos.Models.ViewModels
{
    public class DetalheProdutosViewModel : Notifiable, IValidatable
    {
        #region [ PROPRIEDADES ]

        public int Codigo { get; set; }

        public string Titulo { get; set; }

        public string Descricao { get; set; }

        public decimal Valor { get; set; }

        public int Quantidade { get; set; }

        public DateTime? DataEntrada { get; set; }

        public string Categoria { get; set; }

        #endregion [ PROPRIEDADES ]


        #region [ VALIDAÇÕES ]

        public void Validate()
        {
            AddNotifications(
                new Contract()
                    .IsGreaterThan(Codigo, 0, "Codigo", "Nenhum produto com este código foi encontrado.")
                );
        }

        #endregion [ VALIDAÇÕES ]

    }
}

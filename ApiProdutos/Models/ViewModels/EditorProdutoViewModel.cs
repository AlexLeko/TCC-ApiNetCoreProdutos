using Flunt.Notifications;
using Flunt.Validations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiProdutos.Models.ViewModels
{
    public class EditorProdutoViewModel : Notifiable, IValidatable
    {
        #region [ PROPRIEDADES ]

        public int Codigo { get; set; }

        public string Titulo { get; set; }

        public string Descricao { get; set; }

        public decimal Valor { get; set; }

        public int Quantidade { get; set; }

        public DateTime? DataEntrada { get; set; }

        public int CategoriaId { get; set; }

        public string Categoria { get; set; }

        #endregion [ PROPRIEDADES ]


        #region [ VALIDAÇÕES ]

        public void Validate()
        {
            AddNotifications(
                new Contract()
                    .HasMaxLen(Titulo, 120, "Titulo", "O título deve conter até 120 caracteres")
                    .HasMinLen(Titulo, 3, "Titulo", "O título deve conter pelo menos 3 caracteres")

                    .IsGreaterThan(Valor, 0, "Valor", "O valor do produto deve ser maior que zero")

                    .IsGreaterThan(Quantidade, 0, "Quantidade", "A quantidade deve ser maior que zero")
                );
        }
        #endregion [ VALIDAÇÕES ]
    }
}

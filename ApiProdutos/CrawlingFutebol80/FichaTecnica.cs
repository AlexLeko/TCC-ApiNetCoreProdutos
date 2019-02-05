using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiProdutos.CrawlingFutebol80
{
    public class FichaTecnica
    {
        public string NumeroJogo { get; set; }

        public string TimePalmeiras { get; set; }

        public string Resultado { get; set; }

        public string TimeAdversario { get; set; }

        public string CodigoFutebol80 { get; set; }

        public string DataPartida { get; set; }

        public string Campeonado { get; set; }

        public string FaseCampeonato { get; set; }

        public string Estadio { get; set; }

        public string Cidade { get; set; }

        public string NomeArbitro { get; set; }

        public string RendaPartida { get; set; }

        public string QuantidadePublico { get; set; }

        public List<string> CartoesAmarelo { get; set; }

        public List<string> CartoesVermelho { get; set; }

        public List<string> Gols { get; set; }

        public string JogadoresPalmeiras { get; set; }

        public string TecnicoPalmeiras { get; set; }

        public string JogadoresAdversario { get; set; }

        public string TecnicoAdversario { get; set; }

        public string Observacao { get; set; }

        public bool IsMandante { get; set; }








        //public void VericarMandoPartida()
        //{
        //    if (!string.IsNullOrEmpty(Time1) && Time1.Equals("Palmeiras"))
        //        IsMandante = true;

        //    IsMandante = false;
        //}

        public virtual string TituloCompleto
        {
            get
            {
                if(IsMandante)
                    return $"Jogo {NumeroJogo} - {TimePalmeiras} {Resultado} {TimeAdversario} - {CodigoFutebol80}";
                else
                    return $"Jogo {NumeroJogo} - {TimeAdversario} {Resultado} {TimePalmeiras} - {CodigoFutebol80}";
            }
            set { }
        }

        





    }
}

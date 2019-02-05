using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ApiProdutos.CrawlingFutebol80;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;

namespace ApiProdutos.Controllers.Crawling
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class CrawlingController : ControllerBase
    {
        #region [ CODIGO HEIGHT ]
        public const string TOPO = "112";
        public const string LINHA_VAZIA_EXTERNA = "33";
        public const string LINHA_VAZIA_INTERNA = "8";
        public const string TITULO = "26";
        public const string TEXTO = "21";
        public const string LINHA_FINALIZA_FICHA = "18";
        public const string LINHA_ESPACO_FINAL = "17";
        public const string LINHA_JOGADOR_TECNICO = "24";
        public const string LINHA_FINALIZA_TUDO = "0";

        #endregion [ CODIGO HEIGHT ]

        #region [ CONSTANTES TEXTOS ]
        public const string CARTAO_AMARELO = "Cartão Amarelo:";
        public const string EXPULSAO = "Expulsão:";
        public const string GOLS = "Gols:";
        public const string PALMEIRAS = "Palmeiras:";
        public const string TECNICO = "Técnico:";

        #endregion [ CONSTANTES TEXTOS ]


        [HttpPost]
        [Route("fichas")]
        public void CrawlingFichaTecnicaFutebol80()
        {
            var url = "http://futebol80.com.br/links/times/palmeiras/palmeirasfichas/palmeirasft2018.htm";

            //Uri myUri = new Uri(url, UriKind.Absolute);
            //Encoding encoder = Encoding.GetEncoding("ISO-8859-1");
            //var pagina = LoadWebPageAsync(myUri, encoder);

            CarregarFichaTecnica(url);
        }

        public async Task<string> LoadWebPageAsync(Uri pageUrl, Encoding encoder)
        {
            HttpClient http = new HttpClient();
            string source = string.Empty;

            if (pageUrl != null)
            {
                var response = await http.GetByteArrayAsync(pageUrl.AbsoluteUri.Trim());
                source = encoder.GetString(response, 0, response.Length - 1);
            }

            return source;
        }

        private static async Task CarregarFichaTecnica(string url)
        {
            var httpClient = new HttpClient();
            //var html = await httpClient.GetStringAsync(url);

            // converter texto com caracteres especiais
            string pagina = string.Empty;

            Uri myUri = new Uri(url, UriKind.Absolute);
            Encoding encoder = Encoding.GetEncoding("ISO-8859-1");

            if (myUri != null)
            {
                var response = await httpClient.GetByteArrayAsync(myUri.AbsoluteUri.Trim());
                pagina = encoder.GetString(response, 0, response.Length - 1);
            }


            // extrair dados
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(pagina);

            var linhas = htmlDoc.DocumentNode.Descendants("tr")
                            .Where(x => x.GetAttributeValue("style", "")
                            .Contains("mso-height-source")).ToList();

            int cont = 0;
            List<FichaTecnica> fichas = new List<FichaTecnica>();
            FichaTecnica ficha = null;

            int tr21 = 0;
            bool fichaFinalizada = false;
            bool isSEP = false;
            bool naoMandante = false;


            foreach (var linha in linhas)
            {
                var height = linha.GetAttributeValue("height", "");

                switch (height)
                {
                    // INICIAR CICLO
                    case TOPO:
                        break;

                    case LINHA_VAZIA_EXTERNA:
                        break;

                    case LINHA_VAZIA_INTERNA:
                        break;

                    case TITULO:
                        if (cont < 5) break;

                        // |=====> INICIAR PARTIDA <=====|

                        // TITULO PARTIDA
                        var td26 = linha?.Descendants("td");

                        ficha = new FichaTecnica();

                        // DESCRIÇÃO PARTIDA
                        ficha.NumeroJogo = td26?.FirstOrDefault()?.InnerText?.Trim().Split("Jogo ")?.LastOrDefault();

                        var descricaoPartida = td26?.First()?.NextSibling?.NextSibling;
                        var fonts = descricaoPartida?.Elements("font").ToList();

                        var placar1 = fonts?.FirstOrDefault()?.InnerText.Trim();
                        var placar2 = fonts?.FirstOrDefault()?.NextSibling?.NextSibling?.InnerText.Trim();
                        ficha.Resultado = $"{ placar1 }x{placar2}";

                        var time = descricaoPartida.FirstChild.InnerText;

                        if (time.Equals(PALMEIRAS.Replace(":", "")))
                            ficha.TimePalmeiras = time;
                        else
                            ficha.TimeAdversario = fonts?.LastOrDefault()?.LastChild?.InnerText;

                        ficha.CodigoFutebol80 = td26?.First()?.NextSibling?.NextSibling?.NextSibling?.NextSibling?.InnerText;
                        break;

                    case TEXTO:
                        var proximoHeight = linha?.NextSibling?.NextSibling?.GetAttributeValue("height", "");

                        /*  Para os <TR> com Height de valor 21, verificar qual o tipo de variavel a capturar.  */
                        if (proximoHeight.Equals(TEXTO))
                        {
                            tr21++;
                            bool capturado = false;

                            bool elencoCompleto = false;

                            var elenco = string.Empty;
                            var clube = string.Empty;
                            var tecnico = string.Empty;

                            switch (tr21)
                            {
                                // DATA
                                case 1:
                                    var dataTD = linha?.Descendants("td");
                                    var campo = dataTD?.FirstOrDefault()?.InnerText?.Trim();
                                    ficha.DataPartida = dataTD?.FirstOrDefault()?.InnerText?.Trim();
                                    break;

                                // CAMPEONATO
                                case 2:
                                    var campeonado = linha?.Descendants("td").FirstOrDefault()?.InnerText?.Trim().Split("-");

                                    ficha.Campeonado = campeonado.FirstOrDefault().Trim();
                                    ficha.FaseCampeonato = campeonado.LastOrDefault().Replace("\r\n ", "").Replace("&nbsp;", "").Trim();
                                    break;

                                // LOCAL - ARBITRO
                                case 3:
                                    var localTD = linha?.Descendants("td").FirstOrDefault();

                                    // Local
                                    var local = localTD?.Elements("font").ToList()?.LastOrDefault()?.InnerText;

                                    var descricao = local?.Split('(');
                                    ficha.Estadio = descricao?.FirstOrDefault().Trim();
                                    ficha.Cidade = descricao?.LastOrDefault().ToString().Replace(")", "").Trim();

                                    // Arbitro
                                    var arb = localTD?.NextSibling?.NextSibling.NextSibling.NextSibling;
                                    ficha.NomeArbitro = arb?.Elements("font").ToList()
                                        ?.LastOrDefault()?.InnerText?.Replace("\r\n ", "").Trim();

                                    break;

                                // RENDA - PUBLICO
                                case 4:
                                    var rendaTD = linha?.Descendants("td").FirstOrDefault();

                                    // Renda
                                    ficha.RendaPartida = rendaTD?.Elements("font").ToList()?.LastOrDefault()?.InnerText.Trim();

                                    // Publico
                                    var pub = rendaTD?.NextSibling?.NextSibling.NextSibling.NextSibling.NextSibling.NextSibling;
                                    ficha.QuantidadePublico = pub?.Elements("font").ToList()
                                        ?.LastOrDefault()?.InnerText?.Replace("\r\n ", "").Trim();

                                    break;

                                // CARTÃO AMARELO - EXPULSOS - GOLS
                                case 5:
                                    capturado = VerificarCampoParaCaptura_CartoesGols(ficha, linha);

                                    if (!capturado)
                                    {
                                        VerificarCampos_Clube_Elenco_Tecnico(ficha, ref isSEP, ref naoMandante, linha, ref elencoCompleto, ref elenco, ref clube, ref tecnico);
                                    }
                                    break;

                                case 6:
                                    capturado = VerificarCampoParaCaptura_CartoesGols(ficha, linha);

                                    if (!capturado)
                                    {
                                        VerificarCampos_Clube_Elenco_Tecnico(ficha, ref isSEP, ref naoMandante, linha, ref elencoCompleto, ref elenco, ref clube, ref tecnico);
                                    }
                                    break;

                                case 7:
                                    capturado = VerificarCampoParaCaptura_CartoesGols(ficha, linha);

                                    if (!capturado)
                                    {
                                        VerificarCampos_Clube_Elenco_Tecnico(ficha, ref isSEP, ref naoMandante, linha, ref elencoCompleto, ref elenco, ref clube, ref tecnico);
                                    }
                                    break;

                                case 8:
                                    VerificarCampos_Clube_Elenco_Tecnico(ficha, ref isSEP, ref naoMandante, linha, ref elencoCompleto, ref elenco, ref clube, ref tecnico);
                                    break;


                                case 9:
                                    VerificarCampos_Clube_Elenco_Tecnico(ficha, ref isSEP, ref naoMandante, linha, ref elencoCompleto, ref elenco, ref clube, ref tecnico);
                                    break;

                                case 10:
                                    VerificarCampos_Clube_Elenco_Tecnico(ficha, ref isSEP, ref naoMandante, linha, ref elencoCompleto, ref elenco, ref clube, ref tecnico);
                                    break;

                                case 11:
                                    VerificarCampos_Clube_Elenco_Tecnico(ficha, ref isSEP, ref naoMandante, linha, ref elencoCompleto, ref elenco, ref clube, ref tecnico);
                                    break;
                            }
                        }
                        else if (proximoHeight.Equals(LINHA_VAZIA_INTERNA))
                        {
                            // fim
                        }


                        break;
                }

                if (fichaFinalizada)
                {
                    fichas.Add(ficha);
                    tr21 = 0;
                }

                cont++;
            }

        }

        private static void VerificarCampos_Clube_Elenco_Tecnico(FichaTecnica ficha, ref bool isSEP, ref bool naoMandante, HtmlNode linha, ref bool elencoCompleto, ref string elenco, ref string clube, ref string tecnico)
        {
            var titulo = linha?.Descendants("td")?.FirstOrDefault()?.Elements("font")?.ToList().FirstOrDefault()?.InnerText.Trim();
            var td = linha?.Descendants("td").FirstOrDefault();

            if (!string.IsNullOrEmpty(titulo) && titulo.Contains(":"))
            {
                if (titulo.Contains(TECNICO))
                {
                    var tec = td.Elements("font").ToList()?.LastOrDefault().PreviousSibling.PreviousSibling.InnerText.Trim();
                    var continuaElenco = td?.InnerText.Replace(".", "").Replace("\r\n", "").Replace(";", ", ").Trim();

                    if (!string.IsNullOrEmpty(tec))
                    {
                        tecnico = tec;

                        if (isSEP)
                        {
                            ficha.TecnicoPalmeiras = tecnico;

                            if (!string.IsNullOrEmpty(continuaElenco))
                                string.Concat(ficha.JogadoresPalmeiras, continuaElenco);
                        }
                        else
                        {
                            ficha.TecnicoAdversario = tecnico;

                            if (!string.IsNullOrEmpty(continuaElenco))
                                string.Concat(ficha.JogadoresAdversario, continuaElenco);
                        }

                        elencoCompleto = true;
                    }
                }
                else
                {
                    clube = titulo.Replace(":", "");
                    elenco = td.Elements("font").ToList()?.LastOrDefault()?.InnerText.Replace(".", "").Replace("\r\n", "").Replace(";", ", ").Trim();

                    if (clube.Equals(PALMEIRAS.Replace(":", "")))
                    {
                        isSEP = true;
                        ficha.TimePalmeiras = clube;
                        ficha.JogadoresPalmeiras = elenco;
                        ficha.IsMandante = !naoMandante ? true : false;
                    }
                    else
                    {
                        ficha.TimeAdversario = clube;
                        ficha.JogadoresAdversario = elenco;
                    }
                }

                naoMandante = true;
            }
            else
            {
                if (!elencoCompleto)
                {
                    var continuaTecnico = td?.Elements("font").ToList()?.LastOrDefault()?.InnerText.Replace(".", "").Replace("\r\n", "").Replace(";", ", ").Trim();
                    var continuaElenco = td?.InnerText.Replace(".", "").Replace("\r\n", "").Replace(";", ", ").Trim();

                    if (!string.IsNullOrEmpty(continuaElenco))
                        string.Concat(elenco, " ", continuaElenco);

                    if (string.IsNullOrEmpty(continuaTecnico))
                        tecnico = continuaTecnico;

                }
            }
        }

        private static void VerificarCampo_Elencos_Tecnico(HtmlNode linha, FichaTecnica ficha, string clube, string elenco, string tecnico, bool elencoCompleto)
        {
            //var clube = "";
            //var tecnico = "";
            //var plantel = "";

            var titulo = linha?.Descendants("td")?.FirstOrDefault()?
                            .Elements("font")?.ToList().FirstOrDefault()?.InnerText.Trim();

            var td = linha?.Descendants("td").FirstOrDefault();


            if (!string.IsNullOrEmpty(titulo) && titulo.Contains(":"))
            {

                if (titulo.Contains(TECNICO))
                {
                    var tec = td.Elements("font").ToList()?.LastOrDefault().PreviousSibling.PreviousSibling.InnerText.Trim();

                    if (!string.IsNullOrEmpty(tec))
                    {
                        tecnico = tec;
                        elencoCompleto = true;
                    }
                }
                else
                {
                    clube = titulo.Replace(":", "");
                    elenco = td.Elements("font").ToList()?.LastOrDefault()?.InnerText.Replace(".", "").Replace("\r\n", "").Replace(";", ", ").Trim();
                }

                //if (titulo.Equals(PALMEIRAS))
                //{
                //    elenco = td.Elements("font").ToList()?.LastOrDefault()?.InnerText.Replace(".", "").Replace("\r\n", "").Replace(";", ", ").Trim();
                //    //.Replace(" e ", ";").Trim().Split(";").ToList();
                //}

                //if (titulo.Equals(TECNICO))
                //{
                //    var tec = td.Elements("font").ToList()?.LastOrDefault().PreviousSibling.PreviousSibling.InnerText.Trim();

                //    if (!string.IsNullOrEmpty(tec))
                //        elencoCompleto = true;
                //}

            }
            else
            {
                if (!elencoCompleto)
                {
                    var continuaPlantel = td?.InnerText.Replace(".", "").Replace("\r\n", "").Replace(";", ", ").Trim();

                    if (!string.IsNullOrEmpty(continuaPlantel))
                    {
                        if (string.IsNullOrEmpty(tecnico))
                            tecnico = continuaPlantel;
                        else
                            string.Concat(elenco, " ", continuaPlantel);
                    }
                }
            }


        }

        private static bool VerificarCampoParaCaptura_CartoesGols(FichaTecnica ficha, HtmlNode linha)
        {
            bool retorno = false;

            var titulo = linha?.Descendants("td")?.FirstOrDefault()?.Elements("font")
                ?.ToList().FirstOrDefault()?.InnerText.Trim();

            if (titulo.Equals(CARTAO_AMARELO) || titulo.Equals(EXPULSAO) || titulo.Equals(GOLS))
            {
                var td = linha?.Descendants("td").FirstOrDefault();

                switch (titulo)
                {
                    case CARTAO_AMARELO:
                        var nomesAmarelados = td?.Elements("font").ToList()
                            ?.LastOrDefault()?.InnerText.Replace(".", "").Split(", ");
                        ficha.CartoesAmarelo = nomesAmarelados.ToList();

                        retorno = true;
                        break;

                    case EXPULSAO:
                        var listaExpulsos = td?.Elements("font").ToList()?.LastOrDefault()?.InnerText;
                        var expulsos = listaExpulsos.Replace(".", "").Replace(",", ";").Replace("\r\n", "").Replace(" e ", ";").Trim().Split(';').ToList();

                        ficha.CartoesVermelho = expulsos;
                        retorno = true;
                        break;

                    case GOLS:
                        var textoGols = td?.Elements("font").ToList()?.LastOrDefault()?.InnerText;
                        var gols = textoGols.Replace(".", "").Replace(",", ";").Replace("\r\n", "").Replace(" e ", ";").Trim().Split(';').ToList();

                        // deixar preparado para montar o objeto GOLS.
                        if (gols != null && gols.Count > 0)
                        {
                            var listaGols = new List<string>();
                            foreach (var gol in gols)
                            {
                                listaGols.Add(gol.Trim());
                            }

                            ficha.Gols = listaGols;
                        }
                        retorno = true;
                        break;
                }
            }

            return retorno;
        }




    }
}
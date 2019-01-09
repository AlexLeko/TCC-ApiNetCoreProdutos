using ApiProdutos.CrawlingFutebol80;
using ApiProdutos.CrawlingSelenium;
using ApiProdutos.Models;
using ApiProdutos.Repository.Interface;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace ApiProdutos.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutoController : ControllerBase
    {
        #region [IoC]

        private readonly IProdutoRepository _repository;

        public ProdutoController(IProdutoRepository repository)
        {
            _repository = repository;
        }

        #endregion [IoC]

        // GET: api/Produto
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return new ObjectResult(await _repository.GetAll());
        }

        // GET: api/produto/3
        [HttpGet("{codigo}", Name = "Get")]
        public async Task<IActionResult> GetById(int codigo)
        {
            var produtoDB = await _repository.GetCodigo(codigo);

            if (produtoDB == null)
                return new NotFoundResult();

            return new ObjectResult(produtoDB);
        }

        [HttpPost]
        public IActionResult Post([FromBody]Produto produto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            produto.DataEntrada = DateTime.Now;
            _repository.Create(produto).GetAwaiter().GetResult();

            return new OkObjectResult(produto);
        }

        // PUT: api/produto/5
        [HttpPut("{codigo}")]
        public async Task<IActionResult> Put(int codigo, [FromBody]Produto produto)
        {
            var produtoDB = await _repository.GetCodigo(codigo);

            if (produtoDB == null)
                return new NotFoundResult();

            produto._id = produtoDB._id;

            await _repository.Update(produto);
            return new OkObjectResult(produto);
        }

        // DELETE: api/produto/5
        [HttpDelete("{codigo}")]
        public async Task<IActionResult> Delete(int codigo)
        {
            var produtoDB = await _repository.GetCodigo(codigo);

            if (produtoDB == null)
                return new NotFoundResult();

            await _repository.Delete(codigo);
            return new OkResult();
        }


        #region [ POC CRAWLING - HTTP_CLIENT ]

        [HttpPost]
        [Route("crawling")]
        public void Crawling()
        {
            StartCrowlerAsync();
        }

        private static async Task StartCrowlerAsync()
        {
            var url = "http://www.automobile.tn/neuf/bmw.3/";

            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(url);

            // extrair dados
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var divs = htmlDoc.DocumentNode.Descendants("div")
                .Where(x => x.GetAttributeValue("class", "")
                .Equals("article_new_car article_last_modele")).ToList();

            List<Car> cars = new List<Car>();

            foreach (var div in divs)
            {
                var car = new Car();

                car.Model = div?.Descendants("h2")?.FirstOrDefault()?.InnerText;

                car.Price = div?.Descendants("div")?.FirstOrDefault()?.InnerText;

                car.ImageURL = div?.Descendants("img")?.FirstOrDefault()?
                    .ChildAttributes("src")?.FirstOrDefault()?.Value;

                car.Link = div?.Descendants("a")?.FirstOrDefault()?
                    .ChildAttributes("href")?.FirstOrDefault()?.Value;

                cars.Add(car);

                Directory.CreateDirectory($"C:\\Users\\alex.santos\\Desktop\\Alex\\Teste\\{ car.Model }");
            }
        }

        [DebuggerDisplay("{Model} - {Price}")]
        private class Car
        {
            public string Model { get; set; }
            public string Price { get; set; }
            public string Link { get; set; }
            public string ImageURL { get; set; }
        }

        #endregion [ POC CRAWLING - HTTP_CLIENT ]

        #region [ POC CRAWLING - SELENIUM ]

        public const string TOPO = "112";
        public const string LINHA_VAZIA_EXTERNA = "33";
        public const string LINHA_VAZIA_INTERNA = "8";
        public const string TITULO = "26";
        public const string TEXTO = "21";

        public const string CARTAO_AMARELO = "Cartão Amarelo:";
        public const string EXPULSAO = "Expulsão:";
        public const string GOLS = "Gols:";
        public const string PALMEIRAS = "Palmeiras:";
        public const string TECNICO = "Técnico:";



        



        [HttpPost]
        [Route("crawlingFichas")]
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

            foreach (var item in linhas)
            {
                var height = item.GetAttributeValue("height", "");

                switch (height)
                {
                    case TOPO: break;

                    case LINHA_VAZIA_EXTERNA: break;

                    case LINHA_VAZIA_INTERNA: break;

                    case TITULO:
                        if (cont < 5) break;

                        // recuperar Titulo Partida
                        var td26 = item?.Descendants("td");

                        ficha = new FichaTecnica();

                        // descrição Partida
                        ficha.NumeroJogo = td26?.FirstOrDefault()?.InnerText?.Trim().Split("Jogo ")?.LastOrDefault();

                        var descricaoPartida = td26?.First()?.NextSibling?.NextSibling;
                        var fonts = descricaoPartida?.Elements("font").ToList();

                        var placar1 = fonts?.FirstOrDefault()?.InnerText.Trim();
                        var placar2 = fonts?.FirstOrDefault()?.NextSibling?.NextSibling?.InnerText.Trim();
                        ficha.Resultado = $"{ placar1 }x{placar2}";

                        ficha.Time1 = descricaoPartida.FirstChild.InnerText;
                        ficha.Time2 = fonts?.LastOrDefault()?.LastChild?.InnerText;

                        ficha.CodigoFutebol80 = td26?.First()?.NextSibling?.NextSibling?.NextSibling?.NextSibling?.InnerText;
                        break;

                    case TEXTO:
                        var proximoHeight = item?.NextSibling?.NextSibling?.GetAttributeValue("height", "");

                        /*
                         *  Para os <TR> com Height de valor 21, verificar qual o tipo de variavel a capturar. 
                         */
                        if (proximoHeight.Equals(TEXTO))
                        {
                            tr21++;
                            bool capturado = false;

                            switch (tr21)
                            {
                                // DATA
                                case 1:
                                    var dataTD = item?.Descendants("td");
                                    var campo = dataTD?.FirstOrDefault()?.InnerText?.Trim();
                                    ficha.DataPartida = dataTD?.FirstOrDefault()?.InnerText?.Trim();
                                    break;

                                // CAMPEONATO
                                case 2:
                                    var campeonado = item?.Descendants("td").FirstOrDefault()?.InnerText?.Trim().Split("-");

                                    ficha.Campeonado = campeonado.FirstOrDefault().Trim();
                                    ficha.FaseCampeonato = campeonado.LastOrDefault().Replace("\r\n ", "").Replace("&nbsp;", "").Trim();
                                    break;

                                // LOCAL - ARBITRO
                                case 3:
                                    var localTD = item?.Descendants("td").FirstOrDefault();

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
                                    var rendaTD = item?.Descendants("td").FirstOrDefault();

                                    // Renda
                                    ficha.RendaPartida = rendaTD?.Elements("font").ToList()?.LastOrDefault()?.InnerText.Trim();

                                    // Publico
                                    var pub = rendaTD?.NextSibling?.NextSibling.NextSibling.NextSibling.NextSibling.NextSibling;
                                    ficha.QuantidadePublico = pub?.Elements("font").ToList()
                                        ?.LastOrDefault()?.InnerText?.Replace("\r\n ", "").Trim();

                                    break;

                                // CARTÃO AMARELO - EXPULSOS - GOLS
                                case 5:
                                    capturado = VerificarCampoParaCaptura(ficha, item);

                                    if (!capturado)
                                    {

                                    }

                                    break;

                                case 6:
                                    capturado = VerificarCampoParaCaptura(ficha, item);

                                    if (!capturado)
                                    {

                                    }

                                    break;

                                case 7:
                                case 8:
                                case 9:
                                case 10:
                                case 11:

                                    capturado = VerificarCampoParaCaptura(ficha, item);

                                    if (!capturado)
                                    {
                                        var titulo = item?.Descendants("td")?.FirstOrDefault()?.Elements("font")
                                                    ?.ToList().FirstOrDefault()?.InnerText.Trim();

                                        var td = item?.Descendants("td").FirstOrDefault();

                                        if (titulo.Equals(PALMEIRAS))
                                        {
                                            var lista = td?.Elements("font").ToList()?.LastOrDefault()?.InnerText
                                                .Replace(".", "").Replace("\r\n", "").Replace(" e ", ";").Trim().Split(";").ToList();
                                        }

                                        if (titulo.Equals(TECNICO))
                                        {
                                            var tec = td?.Elements("font").ToList()?.LastOrDefault().PreviousSibling.PreviousSibling.InnerText.Trim();
                                        }




                                    }

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

        private static bool VerificarCampoParaCaptura(FichaTecnica ficha, HtmlNode item)
        {
            bool retorno = false;

            var titulo = item?.Descendants("td")?.FirstOrDefault()?.Elements("font")
                ?.ToList().FirstOrDefault()?.InnerText.Trim();

            if (titulo.Equals(CARTAO_AMARELO) || titulo.Equals(EXPULSAO) || titulo.Equals(GOLS))
            {
                var td = item?.Descendants("td").FirstOrDefault();

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




        #endregion [ POC CRAWLING - SELENIUM ]



    }
}

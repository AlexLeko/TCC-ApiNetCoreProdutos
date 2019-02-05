using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;

namespace ApiProdutos.Controllers.Crawling
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class CrawlingCarrosController : ControllerBase
    {
        [HttpPost]
        [Route("crawlingCarros")]
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

    }
}
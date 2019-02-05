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


        

        



    }
}

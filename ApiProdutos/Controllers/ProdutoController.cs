using ApiProdutos.Models;
using ApiProdutos.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

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

        
        /// <summary>
        /// POST: api/produto
        /// Realiza a inserção de um novo produto.
        /// </summary>
        /// <param name="produto"></param>
        /// <returns></returns>
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

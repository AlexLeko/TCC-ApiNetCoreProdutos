using ApiProdutos.Models;
using ApiProdutos.Models.ViewModels;
using ApiProdutos.Repository;
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
        private readonly ICategoriaRepository _categoriaRepository;

        public ProdutoController(IProdutoRepository repository, ICategoriaRepository categoriaRepository)
        {
            _repository = repository;
            _categoriaRepository = categoriaRepository;
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

            //else
            //{
            //    var prod = new DetalheProdutosViewModel()
            //    {
            //        Codigo = produtoDB.Codigo,
            //        Titulo = produtoDB.Titulo,
            //        Descricao = produtoDB.Descricao,
            //        Valor = produtoDB.Valor,
            //        Categoria = produtoDB.Categoria.Titulo,
            //    };
            //}

            return new ObjectResult(produtoDB);
        }

        // POST: api/produto
        [HttpPost("produto")]
        public async Task<IActionResult> Post([FromBody]Produto produto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //if (!string.IsNullOrEmpty(produto.Categoria.Titulo))
            //{
            //    var categoria = await _repository.GetTitulo(produto.Categoria.Titulo);
            //    if (categoria == null)
            //        await Task.Run(() => _categoriaRepository.Create(categoria));

            //    produto.Categoria = categoria;
            //}

            produto.DataEntrada = DateTime.Now.Date;
            await Task.Run(() => _repository.Create(produto));

            //return CreatedAtAction(nameof(GetById), new { id = produto._id }, produto);
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



        #region [ AUXILIARES ]

        private ResultViewModel RetornoResult(bool status, string message, object data)
        {
            var retorno = new ResultViewModel
            {
                Sucess = status,
                Message = message,
                Data = data
            };

            return retorno;
        }

        #endregion [ AUXILIARES ]
    }
}

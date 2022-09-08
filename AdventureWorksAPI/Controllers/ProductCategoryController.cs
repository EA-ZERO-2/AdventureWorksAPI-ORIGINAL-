using Microsoft.AspNetCore.Mvc;
using AdventureWorksNS.Data;
using AdventureWorksAPI.Repositories;


namespace AdventureWorksAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ProductCategoryController : ControllerBase
    {
        private readonly IProductCategoryRepository repo;

        public ProductCategoryController(IProductCategoryRepository repository)
        {
            this.repo = repository;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ProductCategory>))]
        public async Task<IEnumerable<ProductCategory>> GetProductCategories(string? productCategoryName)
        {
            if (string.IsNullOrEmpty(productCategoryName))
            {
                return await repo.RetrieveAllAsync();
            }
            else
            {
                return (await repo.RetrieveAllAsync())
                        .Where(productCategory => productCategory.Name == productCategoryName);
            }
        }

        [HttpGet("{id}", Name  = nameof(GetProductCategories))] //Ruta
        [ProducesResponseType(200, Type = typeof(ProductCategory))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetProductCategories(int id)
        {
            ProductCategory? p = await repo.RetrieveAsync(id);
            if (p == null)
            {
                return NotFound(); //Error 404
            }
            return Ok(p);
        }

        [HttpPost]
        [ProducesResponseType(201, Type = typeof(ProductCategory))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create([FromBody] ProductCategory p)
        {
            if (p == null)
            {
                return BadRequest(); //400
            }
            ProductCategory? addProductoCategory = await repo.CreateAsync(p);
            if (addProductoCategory == null)
            {
                return BadRequest("El repositorio fallo al crear el productCategory");
            }
            else
            {
                return CreatedAtRoute(
                        routeName: nameof(GetProductCategories),
                        routeValues: new { id = addProductoCategory.ProductCategoryId },
                        value: addProductoCategory);
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(int id, [FromBody] ProductCategory p)
        {
            if (p == null || p.ProductCategoryId != id)
            {
                return BadRequest(); //400
            }
            ProductCategory? existe = await repo.RetrieveAsync(id);
            if (existe == null)
            {
                return NotFound(); //404
            }
            await repo.UpdateAsync(id, p);
            return new NoContentResult(); //204
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id)
        {
            ProductCategory? existe = await repo.RetrieveAsync(id);
            if (existe == null)
            {
                return NotFound(); //404
            }
            bool? deleted = await repo.DeleteAsync(id);
            if (deleted.HasValue && deleted.Value)
            {
                return new NoContentResult(); //201
            }
            return BadRequest($"Product y la Categoria con el id {id} no se pudo borrar");
        }

    }
}

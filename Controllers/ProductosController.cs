using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
// Nota: No necesitamos System.Threading.Tasks si los métodos del Repositorio no son 'async'
/* using tp7.Models;
using tp7.Repositorios; */

namespace tp7
{
    [ApiController]
    [Route("api/[controller]")] // Ruta base: /api/Producto
    public class ProductoController : ControllerBase
    {
        // Usamos el nombre que usaste en el ejemplo anterior para el manejo de la capa de datos
        private readonly ProductoRepository manejoDeProductos = new ProductoRepository();

        // ----------------------------------------------------
        // GET /api/Producto: Listar todos los Productos
        // ----------------------------------------------------
        [HttpGet] 
        public IActionResult ListarProductos() // Cambiamos a IActionResult para ser consistentes
        {
            var productos = manejoDeProductos.ListarProductos();
            return Ok(productos);
        }

        // ----------------------------------------------------
        // GET /api/Producto/{Id}: Obtener detalles por ID
        // ----------------------------------------------------
        [HttpGet("{id}")] 
        public IActionResult ObtenerPorId(int id)
        {
            var producto = manejoDeProductos.ObtenerProductoPorId(id);

            if (producto == null)
            {
                // Devolvemos el estado 404 (Not Found) con un mensaje.
                return NotFound($"Producto con ID {id} no encontrado."); 
            }

            return Ok(producto);
        }

        // ----------------------------------------------------
        // POST /api/Producto: Crear un nuevo Producto
        // ----------------------------------------------------
        [HttpPost]
        public IActionResult Crear([FromBody] Producto producto)
        {
            // Nota: En un caso real, validaríamos 'producto' antes de crearlo.
            manejoDeProductos.CrearProducto(producto);
            
            // Devolvemos el estado 200 OK con un mensaje.
            return Ok("Producto creado correctamente.");
        }

        // ----------------------------------------------------
        // PUT /api/Producto/{Id}: Modificar un Producto
        // ----------------------------------------------------
        [HttpPut("{id}")]
        public IActionResult Actualizar(int id, [FromBody] Producto producto)
        {
            var productoExistente = manejoDeProductos.ObtenerProductoPorId(id);
            if (productoExistente == null)
            {
                return NotFound($"No se puede actualizar. Producto con ID {id} no encontrado.");
            }
            
            // Asignamos el ID de la ruta al objeto para asegurar que el repositorio lo actualice.
            producto.IdProducto = id; 
            manejoDeProductos.ModificarProducto(id, producto);
            
            return Ok("Producto actualizado correctamente.");
        }

        // ----------------------------------------------------
        // DELETE /api/Producto/{Id}: Eliminar un Producto por ID
        // ----------------------------------------------------
        [HttpDelete("{id}")]
        public IActionResult Eliminar(int id)
        {
            var productoExistente = manejoDeProductos.ObtenerProductoPorId(id);
            if (productoExistente == null)
            {
                return NotFound($"No se puede eliminar. Producto con ID {id} no encontrado.");
            }
            
            manejoDeProductos.EliminarProducto(id);
            return Ok("Producto eliminado correctamente.");
        }
    }
}
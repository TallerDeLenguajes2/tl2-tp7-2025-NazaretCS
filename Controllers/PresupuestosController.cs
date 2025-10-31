using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
/* using tp7.Models;
using tp7.Repositorios; */

namespace tp7
{
    // Atributos del Controlador
    [ApiController]
    [Route("api/[controller]")] // Ruta base: /api/Presupuesto
    public class PresupuestosController : ControllerBase
    {
        // Instancia del Repositorio de Presupuestos
        private readonly PresupuestoRepository manejoDePresupuestos = new PresupuestoRepository();

        // ----------------------------------------------------
        // 1. GET /api/presupuesto: Listar todos los presupuestos
        // ----------------------------------------------------
        [HttpGet] 
        public IActionResult ListarPresupuestos()
        {
            var presupuestos = manejoDePresupuestos.ListarPresupuestos();
            return Ok(presupuestos);
        }

        // ----------------------------------------------------
        // 2. GET /api/Presupuesto/{id}: Obtener detalles por ID
        //    (Incluye Productos y Cantidades)
        // ----------------------------------------------------
        [HttpGet("{id}")] 
        public IActionResult ObtenerPorId(int id)
        {
            // Este método del Repositorio es el que se encarga de las dos consultas (Cabecera + Detalle)
            var presupuesto = manejoDePresupuestos.ObtenerPresupuestoPorId(id);

            if (presupuesto == null)
            {
                return NotFound($"Presupuesto con ID {id} no encontrado."); 
            }

            // Aquí podrías agregar la lógica para calcular los montos si no quieres que el cliente los calcule
            // double monto = presupuesto.MontoPresupuesto(); 
            
            return Ok(presupuesto);
        }

        // ----------------------------------------------------
        // 3. POST /api/Presupuesto: Crear un Presupuesto
        // ----------------------------------------------------
        [HttpPost]
        public IActionResult Crear([FromBody] Presupuesto presupuesto)
        {
            // Nota: Al crear un presupuesto por POST, usualmente se crea la cabecera
            // y luego se agregan los detalles con el endpoint de ProductoDetalle.
            manejoDePresupuestos.CrearPresupuesto(presupuesto);
            
            return Ok("Presupuesto de cabecera creado correctamente.");
        }
        
        // ----------------------------------------------------
        // 4. POST /api/Presupuesto/{id}/ProductoDetalle: Agregar un detalle
        // ----------------------------------------------------
        /// <summary>
        /// Agrega un Producto y Cantidad a un Presupuesto existente.
        /// </summary>
        /// <param name="id">ID del presupuesto al que agregar el detalle.</param>
        /// <param name="detalle">Objeto con el ID del Producto y la Cantidad.</param>
        [HttpPost("{id}/ProductoDetalle")]
        // Nota: Crearemos una clase simple para recibir solo el idProducto y la cantidad
        public IActionResult AgregarDetalle(int id, [FromBody] DetalleRequest detalle)
        {
            // 1. Opcional: Verificar si el Presupuesto existe (usando el repo)
            var presupuestoExistente = manejoDePresupuestos.ObtenerPresupuestoPorId(id);
            if (presupuestoExistente == null)
            {
                return NotFound($"No se puede agregar detalle. Presupuesto con ID {id} no encontrado.");
            }
            
            // 2. Ejecutar el método del Repositorio que hace el INSERT en PresupuestosDetalle
            manejoDePresupuestos.AgregarProductoADetalle(id, detalle.IdProducto, detalle.Cantidad);
            
            return Ok($"Detalle (Producto ID: {detalle.IdProducto}) agregado al Presupuesto {id}.");
        }
        
        // ----------------------------------------------------
        // 5. DETELE /api/Presupuesto/{id}: Eliminar un Presupuesto
        // ----------------------------------------------------
        [HttpDelete("{id}")]
        public IActionResult Eliminar(int id)
        {
            var presupuestoExistente = manejoDePresupuestos.ObtenerPresupuestoPorId(id);
            if (presupuestoExistente == null)
            {
                return NotFound($"No se puede eliminar. Presupuesto con ID {id} no encontrado.");
            }
            
            manejoDePresupuestos.EliminaPresupuesto(id);
            return Ok("Presupuesto y sus detalles (si la BD lo permite) eliminados correctamente.");
        }
    }
}
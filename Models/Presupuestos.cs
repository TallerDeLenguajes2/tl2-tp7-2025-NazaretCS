using System.Collections.Generic; // Necesito si quiero usar usar List<T>

namespace tp7
{
    public class Presupuesto
    {
        public int IdPresupuesto { get; set; }
        public required string NombreDestinatario { get; set; }
        
        // DateTime para la fecha
        public DateTime FechaCreacion { get; set; }

        public Presupuesto() { }

        public List<PresupuestosDetalle> Detalle { get; set; } = new List<PresupuestosDetalle>();
        /* Lista producto, cantidad  */

        
        /// Calcula el monto total del presupuesto sin aplicar IVA.
        public double MontoPresupuesto()
        {
            double montoBase = 0;
            
            // Recorre cada detalle en la lista 'Detalle'
            foreach (var item in Detalle)
            {
                // Multiplica el precio del producto por la cantidad y lo suma al total
                montoBase += item.Producto.Precio * item.Cantidad;
            }
            return montoBase;
        }

    
        /// Calcula el monto total del presupuesto aplicando un 21% de IVA.
        public double MontoPresupuestoConIva()
        {
            const double IVA = 0.21;
            double montoBase = MontoPresupuesto();
            return montoBase * (1 + IVA);
        }

        /// Cuenta el total de unidades de productos en el presupuesto.
        public int CantidadProductos()
        {
            int totalCantidad = 0;
            
            // Recorre cada detalle y suma su 'Cantidad'
            foreach (var item in Detalle)
            {
                totalCantidad += item.Cantidad;
            }
            return totalCantidad;
        }
    }
}
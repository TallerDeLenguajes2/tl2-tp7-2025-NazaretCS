using System.Runtime.Intrinsics.X86;

namespace tp7{
    public class PresupuestosDetalle
    {
        private Producto producto;
        private int cantidad;

        public Producto Producto { get => producto; set => producto = value; }
        public int Cantidad { get => cantidad; set => cantidad = value; }

        public PresupuestosDetalle(Producto prod, int cant)
        {
            producto = prod;
            cantidad = cant;
        }

        public PresupuestosDetalle() {}
    }
    
}
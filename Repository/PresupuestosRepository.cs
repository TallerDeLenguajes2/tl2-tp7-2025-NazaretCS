using System.Collections.Generic;
using Microsoft.Data.Sqlite; // Clase de conexión para SQLite
/* using tp7.Models; */  

namespace tp7
{
    public class PresupuestoRepository
    {
        private readonly string cadenaConexion = "Data Source=tienda.db;";

        // ----------------------------------------------------
        // 1. LISTAR TODOS LOS PRESUPUESTOS 
        // ----------------------------------------------------

        /* comentarios de document xml*/
        /// <summary>
        /// Obtiene una lista de todos los Presupuestos de la base de datos.
        /// </summary>
        /// <returns>Una List<Presupuestos> con todos los presupuestos registrados.</returns>

        public List<Presupuesto> ListarPresupuestos()
        {
            var presupuestos = new List<Presupuesto>();
            string sql = "SELECT idPresupuesto, NombreDestinatario, FechaCreacion FROM Presupuestos;";
            using (var conexion = new SqliteConnection(cadenaConexion))
            {
                conexion.Open();
                using (var comando = new SqliteCommand(sql, conexion))
                {
                    using (var lector = comando.ExecuteReader())
                    {
                        while (lector.Read())
                        {
                            var presupuesto = new Presupuesto
                            {
                                IdPresupuesto = Convert.ToInt32(lector["idPresupuesto"]),
                                NombreDestinatario = lector["NombreDestinatario"].ToString(),
                                FechaCreacion = Convert.ToDateTime(lector["FechaCreacion"])
                            };
                            presupuestos.Add(presupuesto);
                        }
                    }
                }
            }
            return presupuestos;
        }

        // ----------------------------------------------------
        // 2. OBTENER PRODUCTO POR ID
        // ----------------------------------------------------

        public Presupuesto? ObtenerPresupuestoPorId(int id)
        {
            Presupuesto? presupuesto = null;
            string sql = "SELECT idPresupuesto, NombreDestinatario, FechaCreacion FROM Presupuestos WHERE idPresupuesto = @Id;";

            using (var conexion = new SqliteConnection(cadenaConexion))
            {
                conexion.Open();
                using (var comando = new SqliteCommand(sql, conexion))
                {
                    comando.Parameters.AddWithValue("@Id", id);

                    using (var lector = comando.ExecuteReader())
                    {
                        if (lector.Read())
                        {
                            presupuesto = new Presupuesto
                            {
                                IdPresupuesto = Convert.ToInt32(lector["idPresupuesto"]),
                                NombreDestinatario = lector["NombreDestinatario"].ToString(),
                                FechaCreacion = Convert.ToDateTime(lector["FechaCreacion"])
                            };
                        }
                    }
                }

                // Si no se encontró el presupuesto, salimos.
                if (presupuesto == null) return null;

                // -------------------------------------------------------------------
                // 2. Obtener el DETALLE (productos y cantidades)
                // -------------------------------------------------------------------

                // Se unen las tablas Detalle y Productos para obtener todos los datos necesarios
                string sqlDetalle = @"
                SELECT
                    pd.cantidad,
                    p.idProducto,
                    p.descripcion,
                    p.precio
                FROM PresupuestosDetalle pd
                JOIN Productos p ON p.idProducto = pd.idProducto
                WHERE pd.idPresupuesto = @Id;";

                using (var comandoDetalle = new SqliteCommand(sqlDetalle, conexion))
                {
                    comandoDetalle.Parameters.AddWithValue("@Id", id);

                    using (var lectorDetalle = comandoDetalle.ExecuteReader())
                    {
                        while (lectorDetalle.Read()) // Itera por cada línea de detalle
                        {
                            // Mapeo del Producto
                            var producto = new Producto
                            {
                                IdProducto = Convert.ToInt32(lectorDetalle["idProducto"]),
                                Descripcion = lectorDetalle["descripcion"].ToString(),
                                Precio = Convert.ToInt32(lectorDetalle["precio"])
                            };

                            // Mapeo del Detalle del Presupuesto
                            var detalle = new PresupuestosDetalle
                            {
                                Producto = producto,
                                Cantidad = Convert.ToInt32(lectorDetalle["cantidad"])
                            };

                            // Agregar el detalle a la lista del objeto Presupuesto
                            presupuesto.Detalle.Add(detalle);
                        }
                    }
                }
            }
            return presupuesto;
        }
    


        /// <summary>
        /// Crea un nuevo Presupuesto en la base de datos.
        /// </summary>
        /// <param name="presupuesto">El objeto Producto con los datos a insertar.</param>

        public void CrearPresupuesto(Presupuesto presupuesto)
        {
            string sql = "INSERT INTO Presupuestos (NombreDestinatario, FechaCreacion) VALUES (@NombreDestinatario, @FechaCreacion);";

            using (var conexion = new SqliteConnection(cadenaConexion))
            {
                conexion.Open();
                using (var comando = new SqliteCommand(sql, conexion))
                {
                    comando.Parameters.AddWithValue("@NombreDestinatario", presupuesto.NombreDestinatario);
                    comando.Parameters.AddWithValue("@FechaCreacion", presupuesto.FechaCreacion);

                    comando.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Modifica los datos (NombreDestinatario y precio) de un Presupuesto existente.
        /// </summary>
        /// <param name="id">El ID del Presupuesto a modificar.</param>
        /// <param name="presupuesto">El objeto Presupuesto con la nueva información.</param>

        public void ModificarPresupuesto(int id, Presupuesto presupuesto)
        {
            string sql = "UPDATE Presupuesto SET NombreDestinatario = @NombreDestinatario, FechaCreacion = @FechaCreacion WHERE idProducto = @Id;";

            using (var conexion = new SqliteConnection(cadenaConexion))
            {
                conexion.Open();

                using (var comando = new SqliteCommand(sql, conexion))
                {
                    comando.Parameters.AddWithValue("@NombreDestinatario", presupuesto.NombreDestinatario);
                    comando.Parameters.AddWithValue("@FechaCreacion", presupuesto.FechaCreacion);

                    comando.Parameters.AddWithValue("@Id", id);
                    comando.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Elimina un Presupuesto de la base de datos por su ID.
        /// </summary>
        /// <param name="id">El ID del Presupuesto a eliminar.</param>

        public void EliminaPresupuesto(int id)
        {
            string sql = "DELETE FROM Presupuestos WHERE idPresupuesto = @Id;";
            using (var conexion = new SqliteConnection(cadenaConexion))
            {
                conexion.Open();
                using (var comando = new SqliteCommand(sql, conexion))
                {
                    comando.Parameters.AddWithValue("@Id", id);

                    comando.ExecuteNonQuery();
                }
            }
        }
        
        /// <summary>
        /// Agrega un producto y su cantidad a un presupuesto existente.
        /// </summary>
        /// <param name="idPresupuesto">El ID del Presupuesto al que se le agrega el detalle.</param>
        /// <param name="idProducto">El ID del Producto a agregar.</param>
        /// <param name="cantidad">La cantidad de unidades del Producto.</param>
        public void AgregarProductoADetalle(int idPresupuesto, int idProducto, int cantidad)
        {
            // La sentencia SQL inserta en la tabla intermedia (detalle)
            string sql = "INSERT INTO PresupuestosDetalle (idPresupuesto, idProducto, cantidad) VALUES (@IdPresupuesto, @IdProducto, @Cantidad);";

            using (var conexion = new SqliteConnection(cadenaConexion))
            {
                conexion.Open();
                using (var comando = new SqliteCommand(sql, conexion))
                {
                    comando.Parameters.AddWithValue("@IdPresupuesto", idPresupuesto);
                    comando.Parameters.AddWithValue("@IdProducto", idProducto);
                    comando.Parameters.AddWithValue("@Cantidad", cantidad);

                    comando.ExecuteNonQuery();
                }
            }
        }
    }
}
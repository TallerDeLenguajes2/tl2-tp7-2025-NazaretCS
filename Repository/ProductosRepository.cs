using System.Collections.Generic;
using Microsoft.Data.Sqlite; // Clase de conexión para SQLite
/* using tp7.Models; */  

namespace tp7
{
    public class ProductoRepository
    {
        private readonly string cadenaConexion = "Data Source=tienda.db;";

        // ----------------------------------------------------
        // 1. LISTAR TODOS LOS PRODUCTOS
        // ----------------------------------------------------

        /* comentarios de document xml*/
        /// <summary>
        /// Obtiene una lista de todos los Productos de la base de datos.
        /// </summary>
        /// <returns>Una List<Producto> con todos los productos registrados.</returns>
        public List<Producto> ListarProductos()
        {
            var productos = new List<Producto>();
            string sql = "SELECT idProducto, descripcion, precio FROM Productos;";

            // 1. Establecer la Conexión (usando 'using' para asegurar el cierre)
            using (var conexion = new SqliteConnection(cadenaConexion))
            {
                conexion.Open(); // Abre la conexión a la base de datos

                // 2. Crear y configurar el Comando
                using (var comando = new SqliteCommand(sql, conexion))
                {
                    // 3. Ejecutar y obtener el DataReader (SELECT devuelve filas)
                    using (var lector = comando.ExecuteReader())
                    {
                        // 4. Leer y Mapear los resultados
                        while (lector.Read()) // Itera por cada fila devuelta
                        {
                            // Mapear la fila (Row) del DataReader al objeto Producto
                            var producto = new Producto
                            {
                                // Accede a las columnas por su nombre y realiza la conversión al tipo C#
                                IdProducto = Convert.ToInt32(lector["idProducto"]),
                                Descripcion = lector["Descripcion"].ToString(),
                                Precio = Convert.ToInt32(lector["Precio"])

                                /* Id = Convert.ToInt32(lector["Id"]),
                                Nombre = lector["Nombre"].ToString(),
                                DNI = lector["DNI"].ToString(),
                                Telefono = lector["Telefono"].ToString() */

                                /*I dProducto = lector.GetInt32(lector.GetOrdinal("idProducto")),
                                Descripcion = lector.GetString(lector.GetOrdinal("descripcion")),
                                Precio = lector.GetInt32(lector.GetOrdinal("precio")) */
                            };

                            productos.Add(producto);
                        }
                    } // El lector se cierra aquí (por el 'using')
                } // El comando se desecha aquí (por el 'using')
            } // La conexión se cierra aquí (por el 'using')

            return productos;
        }

        // ----------------------------------------------------
        // 2. OBTENER PRODUCTO POR ID
        // ----------------------------------------------------

        public Producto? ObtenerProductoPorId(int id)
        {
            // El ? indica que puede devolver null si no encuentra el producto.
            string sql = "SELECT idProducto, Descripcion, Precio FROM Productos WHERE idProducto = @Id;";

            using (var conexion = new SqliteConnection(cadenaConexion))
            {
                conexion.Open();
                using (var comando = new SqliteCommand(sql, conexion))
                {
                    //Se usan parámetros para evitar Inyección SQL
                    comando.Parameters.AddWithValue("@Id", id);

                    using (var lector = comando.ExecuteReader())
                    {
                        if (lector.Read()) // Solo lee la primera fila (ya que el ID es único)
                        {
                            // Mapea y devuelve el objeto Producto
                            return new Producto
                            {
                                IdProducto = Convert.ToInt32(lector["idProducto"]),
                                Descripcion = lector["Descripcion"].ToString(),
                                Precio = Convert.ToInt32(lector["Precio"])

                                /* IdProducto = lector.GetInt32(lector.GetOrdinal("idProducto")),
                                Descripcion = lector.GetString(lector.GetOrdinal("descripcion")),
                                Precio = lector.GetInt32(lector.GetOrdinal("precio")) */
                            };
                        }
                    }
                }
            }
            return null; // Devuelve null si no se encontró el producto con ese ID
        }


        /// <summary>
        /// Crea un nuevo Producto en la base de datos.
        /// </summary>
        /// <param name="producto">El objeto Producto con los datos a insertar.</param>
        public void CrearProducto(Producto producto)
        {
            // Define el comando SQL para la inserción. 
            // NOTA: No insertamos el idProducto ya que se asume que la DB lo autoincrementa.
            string sql = "INSERT INTO Productos (Descripcion, Precio) VALUES (@Descripcion, @Precio);";

            using (var conexion = new SqliteConnection(cadenaConexion))
            {
                conexion.Open();
                using (var comando = new SqliteCommand(sql, conexion))
                {
                    // 1. Usar Parámetros para pasar los valores del objeto Producto.
                    comando.Parameters.AddWithValue("@Descripcion", producto.Descripcion);
                    comando.Parameters.AddWithValue("@Precio", producto.Precio);

                    // 2. Ejecutar ExecuteNonQuery() para ejecutar INSERT. 
                    // Devuelve el número de filas afectadas (1 en este caso).
                    comando.ExecuteNonQuery();
                }
            }
        }


        /// <summary>
        /// Modifica los datos (descripción y precio) de un Producto existente.
        /// </summary>
        /// <param name="id">El ID del Producto a modificar.</param>
        /// <param name="producto">El objeto Producto con la nueva información.</param>
        public void ModificarProducto(int id, Producto producto)
        {
            // Define el comando SQL para la actualización.
            string sql = "UPDATE Productos SET Descripcion = @Descripcion, Precio = @Precio WHERE idProducto = @Id;";

            using (var conexion = new SqliteConnection(cadenaConexion))
            {
                conexion.Open();
                using (var comando = new SqliteCommand(sql, conexion))
                {
                    // 1. Parámetros para los nuevos valores
                    comando.Parameters.AddWithValue("@Descripcion", producto.Descripcion);
                    comando.Parameters.AddWithValue("@Precio", producto.Precio);

                    // 2. Parámetro para la condición WHERE (el ID)
                    comando.Parameters.AddWithValue("@Id", id);

                    // 3. Ejecutar ExecuteNonQuery() para ejecutar UPDATE.
                    comando.ExecuteNonQuery();
                }
            }
        }


        /// <summary>
        /// Elimina un Producto de la base de datos por su ID.
        /// </summary>
        /// <param name="id">El ID del Producto a eliminar.</param>
        public void EliminarProducto(int id)
        {
            // Define el comando SQL para la eliminación.
            string sql = "DELETE FROM Productos WHERE idProducto = @Id;";

            using (var conexion = new SqliteConnection(cadenaConexion))
            {
                conexion.Open();
                using (var comando = new SqliteCommand(sql, conexion))
                {
                    // 1. Parámetro para la condición WHERE (el ID)
                    comando.Parameters.AddWithValue("@Id", id);

                    // 2. Ejecutar ExecuteNonQuery() para ejecutar DELETE.
                    comando.ExecuteNonQuery();
                }
            }
        }
    }
}
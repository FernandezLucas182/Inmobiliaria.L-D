using MySql.Data.MySqlClient;

namespace InmobiliariaMVC.Models
{
    public class InmuebleRepositorio
    {
        private readonly string connectionString = "server=localhost;port=3306;database=inmobiliaria;uid=root;";

        public List<Inmueble> ObtenerTodos()
        {
            var lista = new List<Inmueble>();
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"SELECT i.id_inmueble, i.direccion, i.uso, i.ambientes, i.precio, i.estado,
                                      p.id_propietario, p.apellido, p.nombre,
                                      t.id_tipo, t.nombre AS TipoNombre
                               FROM inmueble i
                               INNER JOIN propietario p ON i.id_propietario = p.id_propietario
                               INNER JOIN tipo t ON i.id_tipo = t.id_tipo;";

                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var inmueble = new Inmueble
                            {
                                IdInmueble = reader.GetInt32("id_inmueble"),
                                Direccion = reader.GetString("direccion"),
                                Uso = reader.GetString("uso"),
                                Ambientes = reader.GetInt32("ambientes"),
                                Precio = reader.GetDecimal("precio"),
                                Estado = reader.GetBoolean("estado"),
                                Propietario = new Propietario
                                {
                                    id_propietario = reader.GetInt32("id_Propietario"),
                                    apellido = reader.GetString("apellido"),
                                    nombre = reader.GetString("nombre")
                                },
                                Tipo = new Tipo
                                {
                                    id_tipo = reader.GetInt32("id_tipo"),
                                    nombre = reader.GetString("nombre")

                                }

                            };
                            lista.Add(inmueble);
                        }
                        ;
                    }
                }
                return lista;
            }

            // Métodos Crear, Editar, Eliminar vendrían después
        }

        public int Baja(int id)
        {
            int filasAfectadas;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"UPDATE inmueble 
                               SET estado = 'false'
                               WHERE id_inmueble = @id;";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    filasAfectadas = command.ExecuteNonQuery();
                }
            }
            return filasAfectadas;
        }
    
    
     public int Crear(Inmueble inmueble)
        {
            int nuevoId;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"INSERT INTO inmueble (direccion, uso, ambientes, precio, estado, id_propietario, id_tipo)
                               VALUES (@direccion, @uso, @ambientes, @precio, @estado, @id_propietario, @id_tipo);
                               SELECT LAST_INSERT_ID();";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@direccion", inmueble.Direccion);
                    command.Parameters.AddWithValue("@uso", inmueble.Uso);
                    command.Parameters.AddWithValue("@ambientes", inmueble.Ambientes);
                    command.Parameters.AddWithValue("@precio", inmueble.Precio);
                    command.Parameters.AddWithValue("@estado", inmueble.Estado);
                    command.Parameters.AddWithValue("@id_propietario", inmueble.Propietario?.id_propietario);
                    command.Parameters.AddWithValue("@id_tipo", inmueble.Tipo.id_tipo);

                    connection.Open();
                    nuevoId = Convert.ToInt32(command.ExecuteScalar());
                }
            }
            return nuevoId;
        }
         public int Editar(Inmueble inmueble)
        {
            int res = 0;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"UPDATE inmueble 
                               SET direccion = @direccion, 
                                   uso = @uso, 
                                   ambientes = @ambientes, 
                                   precio = @precio, 
                                   estado = @estado, 
                                   id_propietario = @id_propietario, 
                                   id_tipo = @id_tipo
                               WHERE id_inmueble = @id_inmueble;";

                using (var command = new MySqlCommand(sql, connection))
                {
                    // Parámetros
                    command.Parameters.AddWithValue("@direccion", inmueble.Direccion);
                    command.Parameters.AddWithValue("@uso", inmueble.Uso);
                    command.Parameters.AddWithValue("@ambientes", inmueble.Ambientes);
                    command.Parameters.AddWithValue("@precio", inmueble.Precio);
                    command.Parameters.AddWithValue("@estado", inmueble.Estado);
                    command.Parameters.AddWithValue("@id_propietario", inmueble.Propietario?.id_propietario);
                    command.Parameters.AddWithValue("@id_tipo", inmueble.Tipo.id_tipo);
                    command.Parameters.AddWithValue("@id_inmueble", inmueble.IdInmueble);

                    connection.Open();
                    res = command.ExecuteNonQuery(); // devuelve filas afectadas
                    connection.Close();
                }
            }
            return res; // si devuelve 1 significa que se actualizó bien
        }
        public Inmueble? ObtenerPorId(int id)
        {
            Inmueble? inmueble = null;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"SELECT i.id_inmueble, i.direccion, i.uso, i.ambientes, i.precio, i.estado,
                                      p.id_propietario, p.apellido, p.nombre,
                                      t.id_tipo, t.nombre AS TipoNombre
                               FROM inmueble i
                               INNER JOIN propietario p ON i.id_propietario = p.id_propietario
                               INNER JOIN tipo t ON i.id_tipo = t.id_tipo
                               WHERE i.id_inmueble = @id;";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            inmueble = new Inmueble
                            {
                                IdInmueble = reader.GetInt32("id_inmueble"),
                                Direccion = reader.GetString("direccion"),
                                Uso = reader.GetString("uso"),
                                Ambientes = reader.GetInt32("ambientes"),
                                Precio = reader.GetDecimal("precio"),
                                Estado = reader.GetBoolean("estado"),
                                Propietario = new Propietario
                                {
                                    id_propietario = reader.GetInt32("id_propietario"),
                                    apellido = reader.GetString("apellido"),
                                    nombre = reader.GetString("nombre")
                                },
                                Tipo = new Tipo
                                {
                                    id_tipo = reader.GetInt32("id_tipo"),
                                    nombre = reader.GetString("nombre")
                                }
                            };
                        }
                    }
                }
            }
            return inmueble;
        }
           public int Habilitar(int id)
           {
            using (var connection = new MySqlConnection(connectionString))
            {
        string sql = "UPDATE inmueble SET estado = 'activo' WHERE id_inmueble = @id;";
        using (var command = new MySqlCommand(sql, connection))
        {
            command.Parameters.AddWithValue("@id", id);
            connection.Open();
            return command.ExecuteNonQuery();
        }
    }

    }
   }
}

    


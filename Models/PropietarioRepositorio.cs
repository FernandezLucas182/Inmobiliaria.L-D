using MySql.Data.MySqlClient;

namespace InmobiliariaMVC.Models
{
    public class PropietarioRepositorio
    {
        private readonly string connectionString = "server=localhost;port=3306;database=inmobiliaria;uid=root;";

        public List<Propietario> ObtenerTodos()
        {
            var lista = new List<Propietario>();
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = "SELECT id_propietario, dni, apellido, nombre, telefono, email FROM Propietario;";
                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Propietario
                            {
                                id_propietario = reader.GetInt32("id_propietario"),
                                dni = reader.GetString("dni"),
                                apellido = reader.GetString("apellido"),
                                nombre = reader.GetString("nombre"),
                                telefono = reader.IsDBNull(reader.GetOrdinal("telefono")) ? "" : reader.GetString("telefono"),
                                email = reader.IsDBNull(reader.GetOrdinal("email")) ? "" : reader.GetString("email")
                            });
                        }
                    }
                }
            }
            return lista;
        }

        public Propietario? ObtenerPorId(int id)
        {
            Propietario? propietario = null;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = "SELECT id_propietario, dni, apellido, nombre, telefono, email FROM Propietario WHERE id_propietario=@id_propietario;";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id_propietario", id);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            propietario = new Propietario
                            {
                                id_propietario = reader.GetInt32("id_propietario"),
                                dni = reader.GetString("dni"),
                                apellido = reader.GetString("apellido"),
                                nombre = reader.GetString("nombre"),
                                telefono = reader.IsDBNull(reader.GetOrdinal("telefono")) ? "" : reader.GetString("telefono"),
                                email = reader.IsDBNull(reader.GetOrdinal("email")) ? "" : reader.GetString("email")
                            };
                        }
                    }
                }
            }
            return propietario;
        }

        public int Alta(Propietario propietario)
        {
            int idInsertado = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"INSERT INTO Propietario (dni, apellido, nombre, telefono, email) 
                               VALUES (@dni, @apellido, @nombre, @telefono, @correo);
                               SELECT LAST_INSERT_ID();";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@dni", propietario.dni);
                    command.Parameters.AddWithValue("@apellido", propietario.apellido);
                    command.Parameters.AddWithValue("@nombre", propietario.nombre);
                    command.Parameters.AddWithValue("@telefono", propietario.telefono);
                    command.Parameters.AddWithValue("@correo", propietario.email);

                    connection.Open();
                    idInsertado = Convert.ToInt32(command.ExecuteScalar());
                    propietario.id_propietario = idInsertado;
                }
            }
            return idInsertado;
        }

        public int Editar(Propietario propietario)
        {
            int filas = 0;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"UPDATE Propietario SET 
                                dni=@dni, apellido=@apellido, nombre=@nombre, 
                                telefono=@telefono, email=@correo
                               WHERE id_propietario=@id_propietario;";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@dni", propietario.dni);
                    command.Parameters.AddWithValue("@apellido", propietario.apellido);
                    command.Parameters.AddWithValue("@nombre", propietario.nombre);
                    command.Parameters.AddWithValue("@telefono", propietario.telefono);
                    command.Parameters.AddWithValue("@correo", propietario.email);
                    command.Parameters.AddWithValue("@id_propietario", propietario.id_propietario);

                    connection.Open();
                    filas = command.ExecuteNonQuery();
                }
            }
            return filas;
        }

        public int Borrar(int id)
        {
            int filas = 0;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = "DELETE FROM Propietario WHERE id_propietario=@id_propietario;";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id_propietario", id);
                    connection.Open();
                    filas = command.ExecuteNonQuery();
                }
            }
            return filas;
        }
    }
}

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
                string sql = "SELECT Id, Dni, Apellido, Nombre, Telefono, Email FROM Propietario;";
                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Propietario
                            {
                                Id = reader.GetInt32("Id"),
                                Dni = reader.GetString("Dni"),
                                Apellido = reader.GetString("Apellido"),
                                Nombre = reader.GetString("Nombre"),
                                Telefono = reader.IsDBNull(reader.GetOrdinal("Telefono")) ? "" : reader.GetString("Telefono"),
                                Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? "" : reader.GetString("Email")
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
                string sql = "SELECT Id, Dni, Apellido, Nombre, Telefono, Email FROM Propietario WHERE Id=@id;";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            propietario = new Propietario
                            {
                                Id = reader.GetInt32("Id"),
                                Dni = reader.GetString("Dni"),
                                Apellido = reader.GetString("Apellido"),
                                Nombre = reader.GetString("Nombre"),
                                Telefono = reader.IsDBNull(reader.GetOrdinal("Telefono")) ? "" : reader.GetString("Telefono"),
                                Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? "" : reader.GetString("Email")
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
                string sql = @"INSERT INTO Propietario (Dni, Apellido, Nombre, Telefono, Email) 
                               VALUES (@dni, @apellido, @nombre, @telefono, @correo);
                               SELECT LAST_INSERT_ID();";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@dni", propietario.Dni);
                    command.Parameters.AddWithValue("@apellido", propietario.Apellido);
                    command.Parameters.AddWithValue("@nombre", propietario.Nombre);
                    command.Parameters.AddWithValue("@telefono", propietario.Telefono);
                    command.Parameters.AddWithValue("@correo", propietario.Email);

                    connection.Open();
                    idInsertado = Convert.ToInt32(command.ExecuteScalar());
                    propietario.Id = idInsertado;
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
                                Dni=@dni, Apellido=@apellido, Nombre=@nombre, 
                                Telefono=@telefono, Email=@correo
                               WHERE Id=@id;";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@dni", propietario.Dni);
                    command.Parameters.AddWithValue("@apellido", propietario.Apellido);
                    command.Parameters.AddWithValue("@nombre", propietario.Nombre);
                    command.Parameters.AddWithValue("@telefono", propietario.Telefono);
                    command.Parameters.AddWithValue("@correo", propietario.Email);
                    command.Parameters.AddWithValue("@id", propietario.Id);

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
                string sql = "DELETE FROM Propietario WHERE Id=@id;";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    filas = command.ExecuteNonQuery();
                }
            }
            return filas;
        }
    }
}

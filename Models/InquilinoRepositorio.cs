using MySql.Data.MySqlClient;

namespace InmobiliariaMVC.Models
{
    public class InquilinoRepositorio
    {
        private readonly string connectionString = "server=localhost;port=3306;database=inmobiliaria;uid=root;";

        // 📌 Alta (Insertar)
        public int Alta(Inquilino inquilino)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"INSERT INTO inquilino (dni, nombre, apellido, telefono, email)
                               VALUES (@dni, @nombreCompleto, @telefono, @email);
                               SELECT LAST_INSERT_ID();";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@dni", inquilino.dni);
                    command.Parameters.AddWithValue("@nombre", inquilino.nombre);
                    command.Parameters.AddWithValue("@apellido", inquilino.apellido);
                    command.Parameters.AddWithValue("@telefono", inquilino.telefono ?? "");
                    command.Parameters.AddWithValue("@email", inquilino.email ?? "");
                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                    inquilino.id_inquilino = res;
                    connection.Close();
                }
            }
            return res;
        }

        // 📌 Obtener Todos
        public IList<Inquilino> ObtenerTodos()
        {
            var lista = new List<Inquilino>();
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"SELECT id_inquilino, dni, nombre, apellido, telefono, email FROM inquilino;";
                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var i = new Inquilino
                        {
                            id_inquilino= reader.GetInt32("id_inquilino"),
                            dni = reader.GetString("dni"),
                            nombre = reader.GetString("nombre"),
                            apellido = reader.GetString("apellido"),
                            telefono = reader["telefono"] != DBNull.Value ? reader.GetString("telefono") : "",
                            email = reader["email"] != DBNull.Value ? reader.GetString("email") : ""
                        };
                        lista.Add(i);
                    }
                    connection.Close();
                }
            }
            return lista;
        }

        // 📌 Obtener Por Id
        public Inquilino? ObtenerPorId(int id)
        {
            Inquilino? inquilino = null;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"SELECT id_inquilino, dni, nombre, apellido, telefono, email
                               FROM inquilino WHERE id_inquilino= @id_inquilino;";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id_inquilino", id);
                    connection.Open();
                    var reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        inquilino = new Inquilino
                        {
                            id_inquilino = reader.GetInt32("id_inquilino"),
                            dni = reader.GetString("dni"),
                            nombre = reader.GetString("nombre"),
                            apellido = reader.GetString("apellido"),
                            telefono = reader["telefono"] != DBNull.Value ? reader.GetString("telefono") : "",
                            email = reader["email"] != DBNull.Value ? reader.GetString("email") : ""
                        };
                    }
                    connection.Close();
                }
            }
            return inquilino;
        }

        // 📌 Modificar (Update)
        public int Modificar(Inquilino inquilino)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"UPDATE inquilino 
                               SET dni = @dni, nombre = @nombre, apellido = @apellido,
                                   telefono = @telefono, email = @email
                               WHERE id_inquilino = @id_inquilino;";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@dni", inquilino.dni);
                    command.Parameters.AddWithValue("@nombre", inquilino.nombre);
                    command.Parameters.AddWithValue("@apellido", inquilino.apellido);
                    command.Parameters.AddWithValue("@telefono", inquilino.telefono ?? "");
                    command.Parameters.AddWithValue("@email", inquilino.email ?? "");
                    command.Parameters.AddWithValue("@id_inquilino", inquilino.id_inquilino);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return res;
        }

        // 📌 Baja (Delete)
        public int Baja(int id)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"DELETE FROM inquilino WHERE id_inquilino = @id_inquilino;";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id_inquilino", id);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return res;
        }
    }
}

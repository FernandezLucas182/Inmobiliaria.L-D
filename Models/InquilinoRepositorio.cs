using MySql.Data.MySqlClient;
using System.Collections.Generic;

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
                string sql = @"INSERT INTO inquilino (Dni, NombreCompleto, Telefono, Email)
                               VALUES (@dni, @nombreCompleto, @telefono, @email);
                               SELECT LAST_INSERT_ID();";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@dni", inquilino.Dni);
                    command.Parameters.AddWithValue("@nombreCompleto", inquilino.NombreCompleto);
                    command.Parameters.AddWithValue("@telefono", inquilino.Telefono ?? "");
                    command.Parameters.AddWithValue("@email", inquilino.Email ?? "");
                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                    inquilino.Id = res;
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
                string sql = @"SELECT Id, Dni, NombreCompleto, Telefono, Email FROM inquilino;";
                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var i = new Inquilino
                        {
                            Id = reader.GetInt32("Id"),
                            Dni = reader.GetString("Dni"),
                            NombreCompleto = reader.GetString("NombreCompleto"),
                            Telefono = reader["Telefono"] != DBNull.Value ? reader.GetString("Telefono") : "",
                            Email = reader["Email"] != DBNull.Value ? reader.GetString("Email") : ""
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
                string sql = @"SELECT Id, Dni, NombreCompleto, Telefono, Email
                               FROM inquilino WHERE Id = @id;";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    var reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        inquilino = new Inquilino
                        {
                            Id = reader.GetInt32("Id"),
                            Dni = reader.GetString("Dni"),
                            NombreCompleto = reader.GetString("NombreCompleto"),
                            Telefono = reader["Telefono"] != DBNull.Value ? reader.GetString("Telefono") : "",
                            Email = reader["Email"] != DBNull.Value ? reader.GetString("Email") : ""
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
                               SET Dni = @dni, NombreCompleto = @nombreCompleto, 
                                   Telefono = @telefono, Email = @email
                               WHERE Id = @id;";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@dni", inquilino.Dni);
                    command.Parameters.AddWithValue("@nombreCompleto", inquilino.NombreCompleto);
                    command.Parameters.AddWithValue("@telefono", inquilino.Telefono ?? "");
                    command.Parameters.AddWithValue("@email", inquilino.Email ?? "");
                    command.Parameters.AddWithValue("@id", inquilino.Id);
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
                string sql = @"DELETE FROM inquilino WHERE Id = @id;";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return res;
        }
    }
}

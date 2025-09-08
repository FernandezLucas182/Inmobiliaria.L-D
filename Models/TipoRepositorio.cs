using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace InmobiliariaMVC.Models
{
    public class TipoRepositorio
    {
        private readonly string connectionString = "server=localhost;user=root;database=inmobiliaria;";

        // 🔹 Obtener todos los tipos de inmuebles
        public List<Tipo> ObtenerTodos()
        {
            var lista = new List<Tipo>();

            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = "SELECT id_tipo, nombre FROM tipo;";

                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        var t = new Tipo
                        {
                            id_tipo = reader.GetInt32("id_tipo"),
                            nombre = reader.GetString("nombre")
                        };
                        lista.Add(t);
                    }
                }
            }

            return lista;
        }

        // 🔹 Obtener tipo por ID
        public Tipo ObtenerPorId(int id)
        {
            Tipo? tipo = null;

            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = "SELECT id_tipo, nombre FROM tipo WHERE id_tipo = @id;";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    connection.Open();
                    var reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        tipo = new Tipo
                        {
                            id_tipo = reader.GetInt32("id_tipo"),
                            nombre = reader.GetString("nombre")
                        };
                    }
                }
            }

            return tipo!;
        }

        // 🔹 Crear un nuevo tipo
        public int Crear(Tipo tipo)
        {
            int nuevoId = 0;

            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"INSERT INTO tipo (nombre) VALUES (@nombre);
                               SELECT LAST_INSERT_ID();";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@nombre", tipo.nombre);

                    connection.Open();
                    nuevoId = Convert.ToInt32(command.ExecuteScalar());
                }
            }

            return nuevoId;
        }

        // 🔹 Editar un tipo
        public int Editar(Tipo tipo)
        {
            int filasAfectadas = 0;

            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"UPDATE tipo SET nombre = @nombre WHERE id_tipo = @id_tipo;";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@nombre", tipo.nombre);
                    command.Parameters.AddWithValue("@id_tipo", tipo.id_tipo);

                    connection.Open();
                    filasAfectadas = command.ExecuteNonQuery();
                }
            }

            return filasAfectadas;
        }

        // 🔹 Eliminar un tipo
        public int Eliminar(int id)
        {
            int filasAfectadas = 0;

            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"DELETE FROM tipo WHERE id_tipo = @id;";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    connection.Open();
                    filasAfectadas = command.ExecuteNonQuery();
                }
            }

            return filasAfectadas;
        }
    }
}

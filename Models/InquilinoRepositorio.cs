using MySql.Data.MySqlClient;

namespace InmobiliariaMVC.Models
{
    public class InquilinoRepositorio
    {
        private readonly string connectionString = "server=localhost;port=3306;database=inmobiliaria;uid=root;pwd=tu_password;";

        public List<Inquilino> ObtenerTodos()
        {
            var lista = new List<Inquilino>();
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = "SELECT Id, Dni, NombreCompleto, Telefono, Email FROM Inquilino;";
                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Inquilino
                            {
                                Id = reader.GetInt32("Id"),
                                Dni = reader.GetString("Dni"),
                                NombreCompleto = reader.GetString("NombreCompleto"),
                                Telefono = reader.IsDBNull(reader.GetOrdinal("Telefono")) ? "" : reader.GetString("Telefono"),
                                Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? "" : reader.GetString("Email")
                            });
                        }
                    }
                }
            }
            return lista;
        }

        public int Alta(Inquilino inquilino)
        {
            int idInsertado = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"INSERT INTO Inquilino (Dni, NombreCompleto, Telefono, Email) 
                               VALUES (@dni, @nombre, @telefono, @correo);
                               SELECT LAST_INSERT_ID();";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@dni", inquilino.Dni);
                    command.Parameters.AddWithValue("@nombre", inquilino.NombreCompleto);
                    command.Parameters.AddWithValue("@telefono", inquilino.Telefono);
                    command.Parameters.AddWithValue("@correo", inquilino.Email);

                    connection.Open();
                    idInsertado = Convert.ToInt32(command.ExecuteScalar());
                    inquilino.Id = idInsertado;
                }
            }
            return idInsertado;
        }
    }
}

using System.Data;
using MySql.Data.MySqlClient;

namespace InmobiliariaMVC.Models
{
    public class ContratoRepositorio
    {
        private readonly string connectionString = "Server=localhost;Database=inmobiliaria;User=root;Password=;";

        public IList<Contrato> ObtenerTodos()
        {
            var lista = new List<Contrato>();
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"
                    SELECT 
                        c.id_contrato, c.id_inquilino, c.id_inmueble, c.monto, 
                        c.fecha_desde, c.fecha_hasta, c.estado,
                        c.CreatedByUserId, c.CreatedAt, c.ClosedByUserId, c.ClosedAt,
                        i.nombre AS InquilinoNombre, 
                        inm.direccion AS InmuebleDireccion, 
                        t.id_tipo, t.nombre AS TipoNombre,
                        p.id_propietario, p.nombre AS PropietarioNombre, p.apellido AS PropietarioApellido
                    FROM contrato c
                    INNER JOIN inquilino i ON c.id_inquilino = i.id_inquilino
                    INNER JOIN inmueble inm ON c.id_inmueble = inm.id_inmueble
                    INNER JOIN tipo t ON inm.id_tipo = t.id_tipo
                    INNER JOIN propietario p ON inm.id_propietario = p.id_propietario;";

                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var contrato = new Contrato
                        {
                            id_contrato = reader.GetInt32("id_contrato"),
                            id_inquilino = reader.GetInt32("id_inquilino"),
                            id_inmueble = reader.GetInt32("id_inmueble"),
                            monto = reader.GetDecimal("monto"),
                            fecha_inicio = reader.GetDateTime("fecha_desde"),
                            fecha_fin = reader.GetDateTime("fecha_hasta"),
                            estado = reader.GetBoolean("estado"),

                            CreatedByUserId = reader.GetInt32("CreatedByUserId"),
                            CreatedAt = reader.GetDateTime("CreatedAt"),
                            ClosedByUserId = reader.IsDBNull("ClosedByUserId") ? (int?)null : reader.GetInt32("ClosedByUserId"),
                            ClosedAt = reader.IsDBNull("ClosedAt") ? (DateTime?)null : reader.GetDateTime("ClosedAt"),

                            Inquilino = new Inquilino
                            {
                                id_inquilino = reader.GetInt32("id_inquilino"),
                                nombre = reader.GetString("InquilinoNombre")
                            },
                            Inmueble = new Inmueble
                            {
                                id_inmueble = reader.GetInt32("id_inmueble"),
                                direccion = reader.GetString("InmuebleDireccion"),
                                Tipo = new Tipo
                                {
                                    id_tipo = reader.GetInt32("id_tipo"),
                                    nombre = reader.GetString("TipoNombre")
                                },
                                Propietario = new Propietario
                                {
                                    id_propietario = reader.GetInt32("id_propietario"),
                                    nombre = reader.GetString("PropietarioNombre"),
                                    apellido = reader.GetString("PropietarioApellido")
                                }
                            }
                        };
                        lista.Add(contrato);
                    }
                }
            }
            return lista;
        }

        public Contrato? ObtenerPorId(int id)
{
    using var connection = new MySqlConnection(connectionString);
    string sql = "SELECT * FROM contrato WHERE id_contrato=@id";
    using var command = new MySqlCommand(sql, connection);
    command.Parameters.AddWithValue("@id", id);
    connection.Open();
    using var reader = command.ExecuteReader();
    if (reader.Read())
    {
        return new Contrato
        {
            id_contrato = reader.GetInt32("id_contrato"),
            id_inquilino = reader.GetInt32("id_inquilino"),
            id_inmueble = reader.GetInt32("id_inmueble"),
            monto = reader.GetDecimal("monto"),
            fecha_inicio = reader.GetDateTime("fecha_desde"),
            fecha_fin = reader.GetDateTime("fecha_hasta"),
            estado = reader.GetBoolean("estado"),
            CreatedByUserId = reader.GetInt32("CreatedByUserId"),
            CreatedAt = reader.GetDateTime("CreatedAt"),
            ClosedByUserId = reader.IsDBNull("ClosedByUserId") ? (int?)null : reader.GetInt32("ClosedByUserId"),
            ClosedAt = reader.IsDBNull("ClosedAt") ? (DateTime?)null : reader.GetDateTime("ClosedAt")
        };
    }

    return null; // Ahora está permitido
}


        public int Alta(Contrato contrato, int userId)
        {
            using var connection = new MySqlConnection(connectionString);

            // Verifica solapamiento de fechas
            string checkSql = @"SELECT COUNT(*) FROM contrato
                                WHERE id_inmueble=@id_inmueble AND estado=1
                                AND NOT (@fecha_fin <= fecha_desde OR @fecha_inicio >= fecha_hasta)";
            using (var checkCmd = new MySqlCommand(checkSql, connection))
            {
                checkCmd.Parameters.AddWithValue("@id_inmueble", contrato.id_inmueble);
                checkCmd.Parameters.AddWithValue("@fecha_inicio", contrato.fecha_inicio);
                checkCmd.Parameters.AddWithValue("@fecha_fin", contrato.fecha_fin);

                connection.Open();
                int existe = Convert.ToInt32(checkCmd.ExecuteScalar());
                connection.Close();
                if (existe > 0) throw new Exception("El inmueble ya tiene un contrato en esas fechas.");
            }

            string sql = @"INSERT INTO contrato
                           (id_inquilino, id_inmueble, fecha_desde, fecha_hasta, monto, estado, CreatedByUserId, CreatedAt)
                           VALUES (@id_inquilino, @id_inmueble, @fecha_inicio, @fecha_fin, @monto, 1, @userId, NOW());
                           SELECT LAST_INSERT_ID();";

            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id_inquilino", contrato.id_inquilino);
            command.Parameters.AddWithValue("@id_inmueble", contrato.id_inmueble);
            command.Parameters.AddWithValue("@fecha_inicio", contrato.fecha_inicio);
            command.Parameters.AddWithValue("@fecha_fin", contrato.fecha_fin);
            command.Parameters.AddWithValue("@monto", contrato.monto);
            command.Parameters.AddWithValue("@userId", userId);

            connection.Open();
            int id = Convert.ToInt32(command.ExecuteScalar());
            connection.Close();
            return id;
        }

        public int Modificacion(Contrato contrato)
        {
            using var connection = new MySqlConnection(connectionString);
            string sql = @"UPDATE contrato
                           SET id_inquilino=@id_inquilino,
                               id_inmueble=@id_inmueble,
                               monto=@monto,
                               fecha_desde=@fecha_inicio,
                               fecha_hasta=@fecha_fin,
                               estado=@estado
                           WHERE id_contrato=@id_contrato";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id_inquilino", contrato.id_inquilino);
            command.Parameters.AddWithValue("@id_inmueble", contrato.id_inmueble);
            command.Parameters.AddWithValue("@monto", contrato.monto);
            command.Parameters.AddWithValue("@fecha_inicio", contrato.fecha_inicio);
            command.Parameters.AddWithValue("@fecha_fin", contrato.fecha_fin);
            command.Parameters.AddWithValue("@estado", contrato.estado);
            command.Parameters.AddWithValue("@id_contrato", contrato.id_contrato);

            connection.Open();
            return command.ExecuteNonQuery();
        }

        public void TerminarContrato(int idContrato, int userId)
        {
            using var connection = new MySqlConnection(connectionString);
            string sql = @"UPDATE contrato
                           SET estado=0, ClosedByUserId=@userId, ClosedAt=NOW()
                           WHERE id_contrato=@idContrato";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@userId", userId);
            command.Parameters.AddWithValue("@idContrato", idContrato);
            connection.Open();
            command.ExecuteNonQuery();
        }

        public int Baja(int id)
        {
            using var connection = new MySqlConnection(connectionString);
            string sql = "DELETE FROM contrato WHERE id_contrato=@id";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);
            connection.Open();
            return command.ExecuteNonQuery();
        }
    }
}

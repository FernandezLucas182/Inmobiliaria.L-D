using MySql.Data.MySqlClient;

namespace InmobiliariaMVC.Models
{
    public class PagoRepositorio
    {
        private readonly string connectionString = "Server=localhost;Database=inmobiliaria;User=root;Password=;";

        public IList<Pago> ObtenerTodos()
        {
            var lista = new List<Pago>();
            using var connection = new MySqlConnection(connectionString);

            string sql = @"
                SELECT p.id_pago, p.id_contrato, p.fecha, p.importe, p.nro_pago, p.estado,
                       c.monto, c.fecha_desde, c.fecha_hasta
                FROM pago p
                INNER JOIN contrato c ON p.id_contrato=c.id_contrato";

            using var command = new MySqlCommand(sql, connection);
            connection.Open();
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var pago = new Pago
                {
                    id_pago = reader.GetInt32("id_pago"),
                    id_contrato = reader.GetInt32("id_contrato"),
                    fecha = reader.GetDateTime("fecha"),
                    importe = reader.GetDecimal("importe"),
                    nro_pago = reader.GetInt32("nro_pago"),
                    estado = reader.GetBoolean("estado"),
                    Contrato = new Contrato
                    {
                        monto = reader.GetDecimal("monto"),
                        fecha_inicio = reader.GetDateTime("fecha_desde"),
                        fecha_fin = reader.GetDateTime("fecha_hasta")
                    }
                };
                lista.Add(pago);
            }

            return lista;
        }

       public Pago? ObtenerPorId(int id)
{
    using var connection = new MySqlConnection(connectionString);
    string sql = "SELECT * FROM pago WHERE id_pago=@id";
    using var command = new MySqlCommand(sql, connection);
    command.Parameters.AddWithValue("@id", id);
    connection.Open();
    using var reader = command.ExecuteReader();
    if (reader.Read())
    {
        return new Pago
        {
            id_pago = reader.GetInt32("id_pago"),
            id_contrato = reader.GetInt32("id_contrato"),
            fecha = reader.GetDateTime("fecha"),
            importe = reader.GetDecimal("importe"),
            nro_pago = reader.GetInt32("nro_pago"),
            estado = reader.GetBoolean("estado")
        };
    }

    return null;
}

        public int Alta(Pago pago, int userId)
        {
            using var connection = new MySqlConnection(connectionString);
            string sql = @"INSERT INTO pago
                           (id_contrato, fecha, importe, nro_pago, estado, CreatedByUserId, CreatedAt)
                           VALUES (@id_contrato, @fecha, @importe, @nro_pago, 1, @userId, NOW());
                           SELECT LAST_INSERT_ID();";

            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id_contrato", pago.id_contrato);
            command.Parameters.AddWithValue("@fecha", pago.fecha);
            command.Parameters.AddWithValue("@importe", pago.importe);
            command.Parameters.AddWithValue("@nro_pago", pago.nro_pago);
            command.Parameters.AddWithValue("@userId", userId);

            connection.Open();
            int id = Convert.ToInt32(command.ExecuteScalar());
            return id;
        }

        public int Modificacion(Pago pago)
        {
            using var connection = new MySqlConnection(connectionString);
            string sql = @"UPDATE pago
                           SET id_contrato=@id_contrato,
                               fecha=@fecha,
                               importe=@importe,
                               nro_pago=@nro_pago,
                               estado=@estado
                           WHERE id_pago=@id_pago";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id_contrato", pago.id_contrato);
            command.Parameters.AddWithValue("@fecha", pago.fecha);
            command.Parameters.AddWithValue("@importe", pago.importe);
            command.Parameters.AddWithValue("@nro_pago", pago.nro_pago);
            command.Parameters.AddWithValue("@estado", pago.estado);
            command.Parameters.AddWithValue("@id_pago", pago.id_pago);

            connection.Open();
            return command.ExecuteNonQuery();
        }

        public int Baja(int id)
        {
            using var connection = new MySqlConnection(connectionString);
            string sql = "DELETE FROM pago WHERE id_pago=@id";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);
            connection.Open();
            return command.ExecuteNonQuery();
        }
    }
}

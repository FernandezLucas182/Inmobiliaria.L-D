using MySql.Data.MySqlClient;

namespace InmobiliariaMVC.Models
{
    public class PagoRepositorio
    {
        private readonly string connectionString = "Server=localhost;Database=inmobiliaria;User=root;Password=;";

        #region Obtener Pagos

        public IList<Pago> ObtenerTodos()
        {
            var lista = new List<Pago>();
            using var connection = new MySqlConnection(connectionString);

            string sql = @"
SELECT 
    p.id_pago, p.id_contrato, p.fecha, p.importe, p.nro_pago, p.detalle, p.estado,
    p.CreatedByUserId, p.CreatedAt, p.ClosedByUserId, p.ClosedAt,
    u1.Nombre AS CreatedByNombre, u1.Apellido AS CreatedByApellido,
    u2.Nombre AS ClosedByNombre, u2.Apellido AS ClosedByApellido,
    c.monto, c.fecha_desde, c.fecha_hasta,
    i.id_inquilino, i.nombre AS InquilinoNombre, i.apellido AS InquilinoApellido,
    im.id_inmueble, im.direccion AS InmuebleDireccion,
    t.id_tipo AS TipoId, t.nombre AS TipoNombre
FROM pago p
INNER JOIN contrato c ON p.id_contrato = c.id_contrato
INNER JOIN inquilino i ON c.id_inquilino = i.id_inquilino
INNER JOIN inmueble im ON c.id_inmueble = im.id_inmueble
INNER JOIN tipo t ON im.id_tipo = t.id_tipo
LEFT JOIN usuario u1 ON p.CreatedByUserId = u1.id_usuario
LEFT JOIN usuario u2 ON p.ClosedByUserId = u2.id_usuario
ORDER BY p.id_pago;";

            using var command = new MySqlCommand(sql, connection);
            connection.Open();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(MapPago(reader));
            }

            return lista;
        }

        public Pago? ObtenerPorId(int id)
        {
            using var connection = new MySqlConnection(connectionString);
            string sql = @"
SELECT 
    p.id_pago, p.id_contrato, p.fecha, p.importe, p.nro_pago, p.detalle, p.estado,
    p.CreatedByUserId, p.CreatedAt, p.ClosedByUserId, p.ClosedAt,
    u1.Nombre AS CreatedByNombre, u1.Apellido AS CreatedByApellido,
    u2.Nombre AS ClosedByNombre, u2.Apellido AS ClosedByApellido,
    c.monto, c.fecha_desde, c.fecha_hasta,
    i.id_inquilino, i.nombre AS InquilinoNombre, i.apellido AS InquilinoApellido,
    im.id_inmueble, im.direccion AS InmuebleDireccion,
    t.id_tipo AS TipoId, t.nombre AS TipoNombre
FROM pago p
INNER JOIN contrato c ON p.id_contrato = c.id_contrato
INNER JOIN inquilino i ON c.id_inquilino = i.id_inquilino
INNER JOIN inmueble im ON c.id_inmueble = im.id_inmueble
INNER JOIN tipo t ON im.id_tipo = t.id_tipo
LEFT JOIN usuario u1 ON p.CreatedByUserId = u1.id_usuario
LEFT JOIN usuario u2 ON p.ClosedByUserId = u2.id_usuario
WHERE p.id_pago=@id;";

            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);
            connection.Open();
            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return MapPago(reader);
            }

            return null;
        }

        public IList<Pago> ObtenerPorContrato(int idContrato)
        {
            var lista = new List<Pago>();
            using var connection = new MySqlConnection(connectionString);

            string sql = @"
SELECT 
     p.id_pago, p.id_contrato, p.fecha, p.importe, p.nro_pago, p.detalle, p.estado,
    p.CreatedByUserId, p.CreatedAt, p.ClosedByUserId, p.ClosedAt,
    u1.Nombre AS CreatedByNombre, u1.Apellido AS CreatedByApellido,
    u2.Nombre AS ClosedByNombre, u2.Apellido AS ClosedByApellido,
    c.monto, c.fecha_desde, c.fecha_hasta,
    i.id_inquilino, i.nombre AS InquilinoNombre, i.apellido AS InquilinoApellido,
    im.id_inmueble, im.direccion AS InmuebleDireccion,
    t.id_tipo AS TipoId, t.nombre AS TipoNombre
FROM pago p
INNER JOIN contrato c ON p.id_contrato = c.id_contrato
INNER JOIN inquilino i ON c.id_inquilino = i.id_inquilino
INNER JOIN inmueble im ON c.id_inmueble = im.id_inmueble
INNER JOIN tipo t ON im.id_tipo = t.id_tipo
LEFT JOIN usuario u1 ON p.CreatedByUserId = u1.id_usuario
LEFT JOIN usuario u2 ON p.ClosedByUserId = u2.id_usuario
WHERE p.id_contrato = @idContrato
ORDER BY p.nro_pago;";

            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@idContrato", idContrato);
            connection.Open();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(MapPago(reader));
            }

            return lista;
        }

        #endregion

        #region ABM

        public void Alta(Pago pago, int userId)
        {
            using var connection = new MySqlConnection(connectionString);
            string sql = @"
INSERT INTO pago (id_contrato, fecha, importe, nro_pago, detalle, estado, CreatedByUserId, CreatedAt)
VALUES (@id_contrato, @fecha, @importe, @nro_pago, @detalle, 1, @CreatedByUserId, NOW());
SELECT LAST_INSERT_ID();";

            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id_contrato", pago.id_contrato);
            command.Parameters.AddWithValue("@fecha", pago.fecha);
            command.Parameters.AddWithValue("@importe", pago.importe);
            command.Parameters.AddWithValue("@nro_pago", pago.nro_pago);
            command.Parameters.AddWithValue("@detalle", pago.detalle);
            command.Parameters.AddWithValue("@CreatedByUserId", userId);

            connection.Open();
            pago.id_pago = Convert.ToInt32(command.ExecuteScalar());
        }

        public int Modificacion(Pago pago)
        {
            using var connection = new MySqlConnection(connectionString);
            string sql = @"UPDATE pago SET detalle=@detalle WHERE id_pago=@id_pago";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@detalle", pago.detalle);
            command.Parameters.AddWithValue("@id_pago", pago.id_pago);

            connection.Open();
            return command.ExecuteNonQuery();
        }

        public void CerrarPago(int idPago, int userId)
        {
            using var connection = new MySqlConnection(connectionString);
            string sql = @"UPDATE pago
                           SET estado=0, ClosedByUserId=@userId, ClosedAt=NOW()
                           WHERE id_pago=@idPago";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@userId", userId);
            command.Parameters.AddWithValue("@idPago", idPago);
            connection.Open();
            command.ExecuteNonQuery();
        }

        public int Baja(int id, int userId)
        {
            CerrarPago(id, userId);
            return 1;
        }

        #endregion

        #region Mapeo privado

        private Pago MapPago(MySqlDataReader reader)
        {
            return new Pago
            {
                id_pago = reader.GetInt32("id_pago"),
                id_contrato = reader.GetInt32("id_contrato"),
                fecha = reader.GetDateTime("fecha"),
                importe = reader.GetDecimal("importe"),
                nro_pago = reader.GetInt32("nro_pago"),
                detalle = reader.GetString("detalle"),
                estado = reader.GetBoolean("estado"),
                CreatedByUserId = reader.IsDBNull(reader.GetOrdinal("CreatedByUserId")) ? null : reader.GetInt32("CreatedByUserId"),
                CreatedAt = reader.IsDBNull(reader.GetOrdinal("CreatedAt")) ? null : reader.GetDateTime("CreatedAt"),
                ClosedByUserId = reader.IsDBNull(reader.GetOrdinal("ClosedByUserId")) ? null : reader.GetInt32("ClosedByUserId"),
                ClosedAt = reader.IsDBNull(reader.GetOrdinal("ClosedAt")) ? null : reader.GetDateTime("ClosedAt"),
                CreatedByUser = reader.IsDBNull(reader.GetOrdinal("CreatedByUserId")) ? null :
                    new Usuario
                    {
                        id_usuario = reader.GetInt32("CreatedByUserId"),
                        nombre = reader.GetString("CreatedByNombre"),
                        apellido = reader.GetString("CreatedByApellido")
                    },
                ClosedByUser = reader.IsDBNull(reader.GetOrdinal("ClosedByUserId")) ? null :
                    new Usuario
                    {
                        id_usuario = reader.GetInt32("ClosedByUserId"),
                        nombre = reader.IsDBNull(reader.GetOrdinal("ClosedByNombre")) ? "" : reader.GetString("ClosedByNombre"),
                        apellido = reader.IsDBNull(reader.GetOrdinal("ClosedByApellido")) ? "" : reader.GetString("ClosedByApellido")
                    },
                Contrato = new Contrato
                {
                    id_contrato = reader.GetInt32("id_contrato"),
                    monto = reader.GetDecimal("monto"),
                    fecha_inicio = reader.GetDateTime("fecha_desde"),
                    fecha_fin = reader.GetDateTime("fecha_hasta"),
                    Inquilino = new Inquilino
                    {
                        id_inquilino = reader.GetInt32("id_inquilino"),
                        nombre = reader.GetString("InquilinoNombre"),
                        apellido = reader.GetString("InquilinoApellido")
                    },
                    Inmueble = new Inmueble
                    {
                        id_inmueble = reader.GetInt32("id_inmueble"),
                        direccion = reader.GetString("InmuebleDireccion"),
                        Tipo = new Tipo
                        {
                            id_tipo = reader.GetInt32("TipoId"),
                            nombre = reader.GetString("TipoNombre")
                        }
                    }
                }
            };
        }

        #endregion
    }
}

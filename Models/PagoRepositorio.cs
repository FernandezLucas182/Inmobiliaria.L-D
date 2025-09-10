using MySql.Data.MySqlClient;

namespace InmobiliariaMVC.Models
{
    public class PagoRepositorio
    {
       private readonly string connectionString = "server=localhost;port=3306;database=inmobiliaria;uid=root;";

        // Alta (insertar un nuevo pago)
        public int Alta(Pago p)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"INSERT INTO Pagos (id_contrato, fehca, importe, nro_pago, estado, detalle) 
                            VALUES (@id_contrato, @fehca, @importe, @nro_pago, @estado, @detalle)";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id_contrato", p.id_contrato);
                    command.Parameters.AddWithValue("@fehca", p.fehca);
                    command.Parameters.AddWithValue("@importe", p.importe);
                    command.Parameters.AddWithValue("@nro_pago", p.nro_pago);
                    command.Parameters.AddWithValue("@estado", p.estado);
                    command.Parameters.AddWithValue("@detalle", p.detalle);

                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }

        // Baja (eliminar un pago)
        public int Baja(int id)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = "DELETE FROM Pagos WHERE id_pago = @id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }

        // Modificación (editar un pago)
        public int Modificacion(Pago p)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"UPDATE Pagos SET id_contrato = @id_contrato, fehca = @fehca, 
                            importe = @importe, nro_pago = @nro_pago, estado = @estado, detalle = @detalle
                            WHERE id_pago = @id_pago";
               using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id_contrato", p.id_contrato);
                    command.Parameters.AddWithValue("@fehca", p.fehca);
                    command.Parameters.AddWithValue("@importe", p.importe);
                    command.Parameters.AddWithValue("@nro_pago", p.nro_pago);
                    command.Parameters.AddWithValue("@estado", p.estado);
                    command.Parameters.AddWithValue("@detalle", p.detalle);
                    command.Parameters.AddWithValue("@id_pago", p.id_pago);

                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }

        // Obtener todos los pagos
        public List<Pago> ObtenerTodos()
        {
            var lista = new List<Pago>();
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"SELECT id_pago, id_contrato, fehca, importe, nro_pago, estado, detalle 
                            FROM Pagos";
                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var p = new Pago
                        {
                            id_pago = reader.GetInt32(0),
                            id_contrato = reader.GetInt32(1),
                            fehca = reader.GetDateTime(2),
                            importe = reader.GetInt32(3),
                            nro_pago = reader.GetInt32(4),
                            estado = reader.GetBoolean(5),
                            detalle = reader.GetString(6)
                        };
                        lista.Add(p);
                    }
                }
            }
            return lista;
        }

        // Obtener un pago por ID
        public Pago? ObtenerPorId(int id)
        {
            Pago? p = null;
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"SELECT id_pago, id_contrato, fehca, importe, nro_pago, estado, detalle 
                            FROM Pagos WHERE id_pago = @id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    var reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        p = new Pago
                        {
                            id_pago = reader.GetInt32(0),
                            id_contrato = reader.GetInt32(1),
                            fehca = reader.GetDateTime(2),
                            importe = reader.GetInt32(3),
                            nro_pago = reader.GetInt32(4),
                            estado = reader.GetBoolean(5),
                            detalle = reader.GetString(6)
                        };
                    }
                }
            }
            return p;
        }
    }
}

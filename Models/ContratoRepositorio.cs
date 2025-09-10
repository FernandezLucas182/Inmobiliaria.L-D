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
                string sql = @"SELECT 
                            c.id_contrato, c.id_inquilino, c.id_inmueble, c.monto, 
                            c.fecha_desde, c.fecha_hasta, c.estado,
                            i.id_inquilino, i.nombre AS InquilinoNombre, 
                            inm.id_inmueble, inm.direccion AS InmuebleDireccion, inm.id_tipo, inm.id_propietario,
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
                            Id = reader.GetInt32("id_contrato"),
                            InquilinoId = reader.GetInt32("id_inquilino"),
                            InmuebleId = reader.GetInt32("id_inmueble"),
                            MontoMensual = reader.GetDecimal("monto"),
                            FechaInicio = reader.GetDateTime("fecha_desde"),
                            FechaFin = reader.GetDateTime("fecha_hasta"),
                            Estado = reader.GetBoolean("estado"),

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


        public int Alta(Contrato contrato)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"INSERT INTO contrato (id_inquilino, id_inmueble, monto, fecha_desde, fecha_hasta, estado)
                               VALUES (@id_inquilino, @id_inmueble, @monto, @fecha_desde, @fecha_hasta, @estado);
                               SELECT LAST_INSERT_ID();";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id_inquilino", contrato.InquilinoId);
                    command.Parameters.AddWithValue("@id_inmueble", contrato.InmuebleId);
                    command.Parameters.AddWithValue("@monto", contrato.MontoMensual);
                    command.Parameters.AddWithValue("@fecha_desde", contrato.FechaInicio);
                    command.Parameters.AddWithValue("@fecha_hasta", contrato.FechaFin);
                    command.Parameters.AddWithValue("@estado", contrato.Estado);

                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                    contrato.Id = res;
                }
            }
            return res;
        }

        public int Modificacion(Contrato contrato)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"UPDATE contrato 
                               SET id_inquilino=@id_inquilino, id_inmueble=@id_inmueble, monto=@monto, 
                                   fecha_desde=@fecha_desde, fecha_hasta=@fecha_hasta, estado=@estado 
                               WHERE id_contrato=@id_contrato;";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id_inquilino", contrato.InquilinoId);
                    command.Parameters.AddWithValue("@id_inmueble", contrato.InmuebleId);
                    command.Parameters.AddWithValue("@monto", contrato.MontoMensual);
                    command.Parameters.AddWithValue("@fecha_desde", contrato.FechaInicio);
                    command.Parameters.AddWithValue("@fecha_hasta", contrato.FechaFin);
                    command.Parameters.AddWithValue("@estado", contrato.Estado);
                    command.Parameters.AddWithValue("@id_contrato", contrato.Id);

                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }

        public int Baja(int id)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = "DELETE FROM contrato WHERE id_contrato=@id;";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }

        public Contrato? ObtenerPorId(int id)
        {
            Contrato? contrato = null;
            using (var connection = new MySqlConnection(connectionString))

            {
                string sql = @"SELECT c.id_contrato, c.id_inquilino, c.id_inmueble, c.monto, 
                            c.fecha_desde, c.fecha_hasta, c.estado,
                            i.id_inquilino, i.nombre AS InquilinoNombre, 
                            inm.id_inmueble, inm.direccion AS InmuebleDireccion, inm.id_tipo, inm.id_propietario,
                            t.id_tipo, t.nombre AS TipoNombre,
                            p.id_propietario, p.nombre AS PropietarioNombre, p.apellido AS PropietarioApellido
                       FROM contrato c
                       INNER JOIN inquilino i ON c.id_inquilino = i.id_inquilino
                       INNER JOIN inmueble inm ON c.id_inmueble = inm.id_inmueble
                       INNER JOIN tipo t ON inm.id_tipo = t.id_tipo
                       INNER JOIN propietario p ON inm.id_propietario = p.id_propietario
                       WHERE c.id_contrato = @id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            contrato = new Contrato
                            {
                                Id = reader.GetInt32("id_contrato"),
                                InquilinoId = reader.GetInt32("id_inquilino"),
                                InmuebleId = reader.GetInt32("id_inmueble"),
                                MontoMensual = reader.GetDecimal("monto"),
                                FechaInicio = reader.GetDateTime("fecha_desde"),
                                FechaFin = reader.GetDateTime("fecha_hasta"),
                                Estado = reader.GetBoolean("estado"),

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
                        }
                    }
                }
            }
            return contrato;

        }
    }
}

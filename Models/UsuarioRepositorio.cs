using MySql.Data.MySqlClient;


namespace InmobiliariaMVC.Models
{
    public class UsuarioRepositorio
    {
        private readonly string connectionString;
        public UsuarioRepositorio(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public Usuario? GetById(int id)
        {
            using var conn = new MySqlConnection(connectionString);
            string sql = "SELECT * FROM usuario WHERE id_usuario = @id";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            using var r = cmd.ExecuteReader();
            if (r.Read())
            {
                return Map(r);
            }
            return null;
        }

        public Usuario? GetByEmail(string email)
        {
            using var conn = new MySqlConnection(connectionString);
            string sql = "SELECT * FROM usuario WHERE email = @email";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@email", email);
            conn.Open();
            using var r = cmd.ExecuteReader();
            if (r.Read()) return Map(r);
            return null;
        }

        public List<Usuario> GetAll()
        {
            var list = new List<Usuario>();
            using var conn = new MySqlConnection(connectionString);
            string sql = "SELECT * FROM usuario ORDER BY apellido, nombre";
            using var cmd = new MySqlCommand(sql, conn);
            conn.Open();
            using var r = cmd.ExecuteReader();
            while (r.Read()) list.Add(Map(r));
            return list;
        }
        #region CREATE
        public int Create(Usuario u)
        {
            using var conn = new MySqlConnection(connectionString);
            string sql = @"INSERT INTO usuario (email,password_hash,nombre,apellido,avatar_path,rol)
                           VALUES (@email,@password_hash,@nombre,@apellido,@avatar_path,@rol);
                           SELECT LAST_INSERT_ID();";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@email", u.email);
            cmd.Parameters.AddWithValue("@password_hash", u.password_hash);
            cmd.Parameters.AddWithValue("@nombre", u.nombre ?? "");
            cmd.Parameters.AddWithValue("@apellido", u.apellido ?? "");
            
            var avatar = string.IsNullOrEmpty(u.avatar_path) ? "/images/imgdef.png" : u.avatar_path;

            cmd.Parameters.AddWithValue("@avatar_path", u.avatar_path);
            cmd.Parameters.AddWithValue("@rol", u.rol ?? "Empleado");
            conn.Open();
            var id = Convert.ToInt32(cmd.ExecuteScalar());
            u.id_usuario = id;
            return id;
        }
        #endregion
        #region UPDATE
        public void Update(Usuario u)
        {
            using var conn = new MySqlConnection(connectionString);
            string sql = @"UPDATE usuario SET email=@email, nombre=@nombre, apellido=@apellido, avatar_path=@avatar_path, rol=@rol WHERE id_usuario=@id";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@email", u.email);
            cmd.Parameters.AddWithValue("@nombre", u.nombre ?? "");
            cmd.Parameters.AddWithValue("@apellido", u.apellido ?? "");
            cmd.Parameters.AddWithValue("@avatar_path", u.avatar_path ?? "");
            cmd.Parameters.AddWithValue("@rol", u.rol ?? "Empleado");
            cmd.Parameters.AddWithValue("@id", u.id_usuario);
            conn.Open();
            cmd.ExecuteNonQuery();
        }
        #endregion
        #region UpdatePassword
        public void UpdatePassword(int id, string passwordHash)
        {
            using var conn = new MySqlConnection(connectionString);
            string sql = "UPDATE usuario SET password_hash=@ph WHERE id_usuario=@id";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@ph", passwordHash);
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            cmd.ExecuteNonQuery();
        }
        #endregion
        #region DELETE
        public void Delete(int id)
        {
            using var conn = new MySqlConnection(connectionString);
            string sql = "DELETE FROM usuario WHERE id_usuario=@id";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            cmd.ExecuteNonQuery();
        }
        #endregion
        #region Map
        private Usuario Map(MySqlDataReader r)
        {
            return new Usuario
            {
                id_usuario = r["id_usuario"] != DBNull.Value ? Convert.ToInt32(r["id_usuario"]) : 0,
                email = r["email"]?.ToString() ?? string.Empty,
                password_hash = r["password_hash"]?.ToString() ?? string.Empty,
                nombre = r["nombre"]?.ToString() ?? string.Empty,
                apellido = r["apellido"]?.ToString() ?? string.Empty,
                avatar_path = r["avatar_path"]?.ToString() ?? string.Empty,
                rol = r["rol"]?.ToString() ?? "Empleado",
                created_at = r["created_at"] != DBNull.Value ? Convert.ToDateTime(r["created_at"]) : DateTime.MinValue,
                updated_at = r["updated_at"] != DBNull.Value ? Convert.ToDateTime(r["updated_at"]) : DateTime.MinValue
            };
        }
#endregion

    }
}

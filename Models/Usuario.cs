namespace InmobiliariaMVC.Models
{
    public class Usuario
    {
        public int id_usuario { get; set; }
        public string email { get; set; } = "";
        public string password_hash { get; set; } = "";
        public string nombre { get; set; } = "";
        public string apellido { get; set; } = "";
        public string? avatar_path { get; set; } = "/images/imgdef.png";
        public string rol { get; set; } = ""; 
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }
}

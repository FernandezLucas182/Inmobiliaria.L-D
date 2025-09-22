namespace InmobiliariaMVC.Models
{
    public class Inquilino
    {
        public int id_inquilino { get; set; }
        public string dni { get; set; } = "";
        public string? nombre { get; set; } = "";
        public string? apellido { get; set; } = "";
        public string telefono { get; set; } = "";
        public string email { get; set; } = "";
    }
}
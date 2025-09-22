

namespace InmobiliariaMVC.Models
{
    public class Pago
    {
        public int id_pago { get; set; }
        public int id_contrato { get; set; }
        public DateTime fecha { get; set; }
        public int importe { get; set; }
        public int nro_pago { get; set; }
        public bool estado { get; set; } = true;

        // --- Auditoría ---
        public int CreatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? CanceledByUserId { get; set; }
        public DateTime? CanceledAt { get; set; }

        public Usuario? CreatedByUser { get; set; }
        public Usuario? CanceledByUser { get; set; }

        public Contrato? Contrato { get; set; }
    }
}


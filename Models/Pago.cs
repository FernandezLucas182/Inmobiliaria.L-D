namespace InmobiliariaMVC.Models
{
    public class Pago
    {
        public int id_pago { get; set; }
        public int id_contrato { get; set; }
        public DateTime fecha { get; set; }
        public decimal importe { get; set; }  
        public int nro_pago { get; set; }

        public string? detalle { get; set; } = "";
        public bool estado { get; set; } = true;

        // --- Auditoría ---
        public int? CreatedByUserId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? ClosedByUserId { get; set; }
        public DateTime? ClosedAt { get; set; }

        // Relaciones 
        public Usuario? CreatedByUser { get; set; }
        public Usuario? ClosedByUser { get; set; }

        public Contrato? Contrato { get; set; }

    }
}

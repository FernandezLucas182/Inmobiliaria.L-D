using System;
using System.ComponentModel.DataAnnotations;

namespace InmobiliariaMVC.Models
{
    public class Pago
    {
        public int id_pago { get; set; }

        [Required]
        public int id_contrato { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime fecha { get; set; }

        [Required]
        public decimal importe { get; set; }

        [Required]
        public int nro_pago { get; set; }

        [Required]
        public string detalle { get; set; } = "";

        public bool estado { get; set; } = true;

        // --- Auditoría ---
        public int CreatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; }

        public int? AnulledByUserId { get; set; }
        public DateTime? AnulledAt { get; set; }

        public Contrato? Contrato { get; set; }


    }
}

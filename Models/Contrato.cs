using System;
using System.ComponentModel.DataAnnotations;

namespace InmobiliariaMVC.Models
{
    public class Contrato
{
    public int id_contrato { get; set; }

    [Required]
    public int id_inquilino { get; set; }

    [Required]
    public int id_inmueble { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime fecha_inicio { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime fecha_fin { get; set; }
    public bool estado { get; set; } = true;

    [Required]
    public decimal monto { get; set; }

    // --- Auditoría ---
    public int CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? ClosedByUserId { get; set; }
    public DateTime? ClosedAt { get; set; }

    public Usuario? CreatedByUser { get; set; }   // ← agregado
    public Usuario? ClosedByUser { get; set; }    // ← agregado

    public Inmueble? Inmueble { get; set; }
    public Inquilino? Inquilino { get; set; }
    public Usuario? Usuario { get; set; }
}

}

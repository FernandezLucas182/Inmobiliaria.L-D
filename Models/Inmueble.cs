using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace InmobiliariaMVC.Models
{
	[Table("Inmuebles")]
	public class Inmueble
	{
		[Display(Name = "Nº")]
		public int id_inmueble { get; set; }
		//[Required]
		[Display(Name = "Dirección")]
		[Required(ErrorMessage = "La dirección es requerida")]
		public string? direccion { get; set; }
		[Required]
		public int ambiente { get; set; }
		[Required]
		public int superficie { get; set; }
		public decimal latitud { get; set; }
		public decimal longitud { get; set; }
		[Display(Name = "Dueño")]
		public int id_propietario{ get; set; }
		[ForeignKey(nameof(id_propietario))]
    [BindNever]
		public Propietario? nombre { get; set; }

		public Propietario? apellido { get; set; }

		public string? portada { get; set; }
		[NotMapped]//Para EF
		public bool habilitado { get; set; } = true;
	}
	
}
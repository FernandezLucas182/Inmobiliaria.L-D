namespace InmobiliariaMVC.Models
{
	public class Inmueble
	{
		public int id_inmueble { get; set; }
		public int id_propietario { get; set; }
		public string? direccion { get; set; }
		public string uso { get; set; } = "";// "Residencial" o "Comercial"
		public int ambiente { get; set; }
		public decimal precio { get; set; }
		public bool estado { get; set; } = true;// "Disponible" o "Suspendido"
		public required Tipo Tipo { get; set; }   // acá guardás "Casa", "Depto", "Local"

		// Relación
		public Propietario? Propietario { get; set; }
	}
}

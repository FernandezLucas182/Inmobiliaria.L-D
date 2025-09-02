namespace InmobiliariaMVC.Models
{
public class Inmueble
{
	public int IdInmueble { get; set; }
	public int IdPropietario { get; set; }
	public  string? Direccion { get; set; }
	public string Uso { get; set; } = "";// "Residencial" o "Comercial"
	public int Ambientes { get; set; }
	public decimal Precio { get; set; }
	public bool Estado { get; set; } = true;// "Disponible" o "Suspendido"
	public required Tipo Tipo { get; set; }   // acá guardás "Casa", "Depto", "Local"

	// Relación
	public Propietario? Propietario { get; set; }
}
}

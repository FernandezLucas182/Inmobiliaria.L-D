using InmobiliariaMVC.Models;

namespace InmobiliariaMVC
{
    public class Pago
    {
        public int id_pago { get; set; }

        public int id_contrato { get; set; }

        public DateTime fehca { get; set; }

        public int importe { get; set; }

        public int nro_pago { get; set; }

        public bool estado { get; set; } = true;

        public string detalle { get; set; } = "";

        public Contrato? Contrato { get; set; }

    }
}
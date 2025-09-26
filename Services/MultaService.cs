using InmobiliariaMVC.Models;
using System;

namespace InmobiliariaMVC.Services
{
    public class MultaService
    {
        public decimal CalcularMulta(Contrato contrato)
        {
            if (contrato == null)
                throw new ArgumentNullException(nameof(contrato));

            int duracionTotal = (contrato.fecha_fin - contrato.fecha_inicio).Days;
            int diasCumplidos = (DateTime.Now - contrato.fecha_inicio).Days;

            int mesesMulta = diasCumplidos < duracionTotal / 2 ? 2 : 1;

            return contrato.monto * mesesMulta;
        }
    }
}

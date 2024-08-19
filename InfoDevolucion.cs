using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVLaJoya
{
    internal class InfoDevolucion
    {
        public string FolioVenta { get; set; }
        public string Cliente { get; set; }
        public string FechaVenta { get; set; }
        public string FechaDevolucion { get; set; }
        public string Atendio { get; set; }
        public string Sucursal { get; set; }
        public string Subtotal { get; set; }
        public string IvaIeps { get; set; }
        public string Total { get; set; }
        public string MontoLetra { get; set; }
        public string MontoMonederoAnterior { get; set; }
        public string MontoMonederoActual { get; set; }
        public string Motivo { get; set; }
        public string Accion { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVLaJoya
{
    public class InfoTicket
    {
        public string FolioVenta { get; set; }
        public string Cliente { get; set; }
        public string FechaVenta{ get; set; }
        public string Atendio { get; set; }
        public string Sucursal { get; set; }
        public string Subtotal { get; set; }
        public string IvaIeps { get; set; }
        public string Total { get; set; }
        public string Recibido { get; set; }
        public string Cambio { get; set; }
        public string MontoLetra { get; set; }
        public byte[] CodigoBarras { get; set; }
        public string MontoMonedero { get; set; }
        public string MontoPromedio { get; set; }
    }
}

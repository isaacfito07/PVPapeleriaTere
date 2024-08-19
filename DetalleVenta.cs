using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVLaJoya
{
    public class DetalleVenta
    {
        public string Producto { get; set; }
        public float Cantidad { get; set; }
        public float Precio { get; set; }

        public float Descuento { get; set; }
        public float Total { get; set; }
    }
}

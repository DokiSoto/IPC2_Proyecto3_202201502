using System.Collections.Generic;
namespace Server_Backend.Models
{
    public class DataStore
    {
        public static List<Cliente> Clientes = new List<Cliente>();
        public static List<Banco> Bancos = new List<Banco>();

        public static List<Factura> Facturas = new List<Factura>();
        public static List<Pago> Pagos = new List<Pago>();
    }
}

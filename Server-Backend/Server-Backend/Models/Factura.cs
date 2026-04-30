namespace Server_Backend.Models
{
    public class Factura
    {
        public string Numero { get; set; }
        public string NIT {  get; set; }
        public DateTime Fecha { get; set; }
        public double Total {  get; set; }
        public double SaldoPendiente { get; set; }

    }
}

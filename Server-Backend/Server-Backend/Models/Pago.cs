namespace Server_Backend.Models
{
    public class Pago
    {
        public int CodigoBanco {  get; set; }
        public string NIT {  get; set; }
        public DateTime Fecha { get; set; }
        public double Monto {  get; set; }
    }
}

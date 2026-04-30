namespace Server_Backend.Models
{
    public class RespuestaTransac
    {
        public int NuevasFactiras {  get; set; }
        public int FacturasDuplicadas { get; set; }
        public int FacturasconError { get; set; }
        public int NuevosPagos { get; set; }
        public int PagosDuplicados { get; set; }
        public int PagosconError { get; set; }

    }
}

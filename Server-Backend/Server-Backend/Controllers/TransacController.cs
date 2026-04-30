using Microsoft.AspNetCore.Mvc;
using Server_Backend.Services;
using System.Text;
namespace Server_Backend.Controllers
{
    [ApiController]
    [Route("api/transac")]
    public class TransacController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CargarTransacciones()
        {
            using( var reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                var xml = await reader.ReadToEndAsync();

                TransacService service = new TransacService();
                var respuesta = service.Procesar(xml);
                string xmlRespuesta = $@"
<transacciones>
 <facturas>
  <nuevasFacturas>{respuesta.NuevasFactiras}</nuevasFacturas>
  <facturasDuplicadas>{respuesta.FacturasDuplicadas}</facturasDuplicadas>
  <facturasConError>{respuesta.FacturasconError}</facturasConError>
 </facturas>
 <pagos>
  <nuevosPagos>{respuesta.NuevosPagos}</nuevosPagos>
  <pagosDuplicados>{respuesta.PagosDuplicados}</pagosDuplicados>
  <pagosConError>{respuesta.PagosconError}</pagosConError>
 </pagos>
</transacciones>";

                return Content(xmlRespuesta, "application/xml");
            }
        }

    }
}

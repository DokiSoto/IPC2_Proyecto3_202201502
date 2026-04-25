using Microsoft.AspNetCore.Mvc;
using Server_Backend.Services;
using System.Text;

namespace Server_Backend.Controllers
{
    [ApiController]
    [Route("api/config")]
    public class ConfigController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CargarConfig()
        {
            using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                var xml = await reader.ReadToEndAsync(); // 🔥 AQUÍ ESTÁ EL CAMBIO

                ConfigService service = new ConfigService();
                var respuesta = service.ProcesarXML(xml);

                string xmlRespuesta = $@"
<respuesta>
    <clientes>
        <creados>{respuesta.ClientesCreados}</creados>
        <actualizados>{respuesta.ClientesActualizados}</actualizados>
    </clientes>
    <bancos>
        <creados>{respuesta.BancosCreados}</creados>
        <actualizados>{respuesta.BancosActualizados}</actualizados>
    </bancos>
</respuesta>";

                return Content(xmlRespuesta, "application/xml");
            }
        }
    }
}
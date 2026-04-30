using Microsoft.AspNetCore.Mvc;
using Server_Backend.Models;
using System.Globalization;
using System.Text;

namespace Server_Backend.Controllers
{
    [ApiController]
    [Route("api/ingresos")]
    public class IngresosController : ControllerBase
    {
        [HttpGet("{mes}")]
        public IActionResult ObtenerIngresos(string mes)
        {
            DateTime fechaBase;
            if (!DateTime.TryParseExact(mes, "MM-yyyy", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out fechaBase))
            {
                return Content("<error>Formato inválido (usar MM-yyyy)</error>", "application/xml");
            }

            var meses = new List<DateTime>
            {
                fechaBase,
                fechaBase.AddMonths(-1),
                fechaBase.AddMonths(-2)
            };

            var bancos = DataStore.Bancos;

            StringBuilder xml = new StringBuilder();
            xml.AppendLine("<ingresos>");

            foreach (var banco in bancos)
            {
                xml.AppendLine("<banco>");
                xml.AppendLine($"<nombre>{banco.Nombre}</nombre>");

                foreach (var m in meses)
                {
                    var total = DataStore.Pagos
                        .Where(p => p.CodigoBanco == banco.Codigo &&
                                    p.Fecha.Month == m.Month &&
                                    p.Fecha.Year == m.Year)
                        .Sum(p => p.Monto);

                    xml.AppendLine("<mes>");
                    xml.AppendLine($"<fecha>{m:MM/yyyy}</fecha>");
                    xml.AppendLine($"<total>{total}</total>");
                    xml.AppendLine("</mes>");
                }

                xml.AppendLine("</banco>");
            }

            xml.AppendLine("</ingresos>");

            return Content(xml.ToString(), "application/xml");
        }
    }
}
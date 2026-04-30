using Microsoft.AspNetCore.Mvc;
using Server_Backend.Models;
using System.Linq;
using System.Text;
using System.Xml;

namespace Server_Backend.Controllers
{
    [ApiController]
    [Route("api/estadoCuenta")]
    public class EstadoCuentaController : ControllerBase
    {
        [HttpGet("{nit}")]
        public IActionResult ObtenerEstadoCuenta(string nit)
        {

            var cliente = DataStore.Clientes
                .FirstOrDefault(c => c.NIT == nit);

            if (cliente == null)
            {

                return Content("<error>Cliente no encontrado</error>", "application/xml");
            }

            var facturas = DataStore.Facturas
                .Where(f => f.NIT == nit)
                .OrderByDescending(f => f.Fecha)
                .ToList();


            var pagos = DataStore.Pagos
                .Where(p => p.NIT == nit)
                .OrderByDescending(p => p.Fecha)
                .ToList();

            StringBuilder xml = new StringBuilder();

            xml.AppendLine("<estadoCuenta>");

            xml.AppendLine("<cliente>");
            xml.AppendLine($"<NIT>{cliente.NIT}</NIT>");
            xml.AppendLine($"<nombre>{cliente.Nombre}</nombre>");
            xml.AppendLine($"<saldoFavor>{cliente.SaldoFavor}</saldoFavor>");
            xml.AppendLine("</cliente>");

            xml.AppendLine("<transacciones>");
            foreach (var f in facturas)
            {
                xml.AppendLine("<transaccion>");
                xml.AppendLine($"<fecha>{f.Fecha:dd/MM/yyyy}</fecha>");
                xml.AppendLine($"<tipo>Factura</tipo>");
                xml.AppendLine($"<numero>{f.Numero}</numero>");
                xml.AppendLine($"<monto>{f.Total}</monto>");
                xml.AppendLine($"<saldoPendiente>{f.SaldoPendiente}</saldoPendiente>");
                xml.AppendLine("</transaccion>");
            }

            foreach (var p in pagos)
            {
                xml.AppendLine("<transaccion>");
                xml.AppendLine($"<fecha>{p.Fecha:dd/MM/yyyy}</fecha>");
                xml.AppendLine($"<tipo>Pago</tipo>");
                xml.AppendLine($"<banco>{p.CodigoBanco}</banco>");
                xml.AppendLine($"<monto>{p.Monto}</monto>");
                xml.AppendLine("</transaccion>");
            }

            xml.AppendLine("</transacciones>");
            xml.AppendLine("</estadoCuenta>");

            return Content(xml.ToString(), "application/xml");
        }
    
    [HttpGet("todos")]
        public IActionResult EstadoCuentaTodos()
        {
            var clientes = DataStore.Clientes
                .OrderBy(c => c.NIT)
                .ToList();

            XmlDocument doc = new XmlDocument();
            XmlElement root = doc.CreateElement("clientes");
            doc.AppendChild(root);

            foreach (var c in clientes)
            {
                XmlElement cliente = doc.CreateElement("cliente");

                XmlElement nit = doc.CreateElement("NIT");
                nit.InnerText = c.NIT;

                XmlElement nombre = doc.CreateElement("nombre");
                nombre.InnerText = c.Nombre;

                XmlElement saldo = doc.CreateElement("saldoFavor");
                saldo.InnerText = c.SaldoFavor.ToString();

                cliente.AppendChild(nit);
                cliente.AppendChild(nombre);
                cliente.AppendChild(saldo);

                root.AppendChild(cliente);
            }

            return Content(doc.OuterXml, "application/xml");
        } }
    }
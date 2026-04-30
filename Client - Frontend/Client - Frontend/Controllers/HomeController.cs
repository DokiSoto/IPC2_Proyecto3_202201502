using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using System.IO;
using Rotativa.AspNetCore;
using Client___Frontend.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text;
using Microsoft.AspNetCore.Routing.Constraints;

namespace Client___Frontend.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Peticiones()
        {
            return View();
        }

        public IActionResult EstadoCuenta()
        {
            return View();
        }

        public IActionResult IngrersosView()
        {
            return View();
        }

        public IActionResult Ayuda()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Reset()
        {
            using (HttpClient client=new HttpClient())
            {
                var response = await client.PostAsync("https://localhost:7196/api/reset", null);
                var data = await response.Content.ReadAsStringAsync();

                ViewBag.Respuesta = data;
            }
            return View("Index");
        }

        [HttpPost]
        public async Task<IActionResult> CargarConfig(string xml)
        {
            using(HttpClient client=new HttpClient())
            {
                var content = new StringContent(xml, Encoding.UTF8, "application/xml");
                var response = await client.PostAsync("https://localhost:7196/api/config", content);

                var data = await response.Content.ReadAsStringAsync();

                ViewBag.Respuesta = data;
            }
            return View("Index");
        }

        [HttpPost]
        public async Task<IActionResult> CargarTransac(string xml)
        {
            using (HttpClient client = new HttpClient())
            {
                var content = new StringContent(xml, Encoding.UTF8, "application/xml");
                var response = await client.PostAsync("https://localhost:7196/api/transac", content);

                var data = await response.Content.ReadAsStringAsync();

                ViewBag.Respuesta = data;
            }

            return View("Index");
        }

        [HttpGet]
        public async Task<IActionResult> EstadoCuentaview(string nit)
        {
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync($"https://localhost:7196/api/estadoCuenta/{nit}");
                var xml = await response.Content.ReadAsStringAsync();

                var doc = new System.Xml.XmlDocument();
                doc.LoadXml(xml);

                ViewBag.NIT = doc.SelectSingleNode("//cliente/NIT")?.InnerText;
                ViewBag.Nombre = doc.SelectSingleNode("//cliente/nombre")?.InnerText;
                ViewBag.Saldo = doc.SelectSingleNode("//cliente/saldoFavor")?.InnerText;

                var transacciones = doc.SelectNodes("//transaccion");

                List<dynamic> lista = new List<dynamic>();

                foreach (System.Xml.XmlNode t in transacciones)
                {
                    lista.Add(new
                    {
                        Fecha = t["fecha"]?.InnerText,
                        Tipo = t["tipo"]?.InnerText,
                        Monto = t["monto"]?.InnerText,
                        Banco = t["banco"]?.InnerText,
                        Numero = t["numero"]?.InnerText
                    });
                }

                ViewBag.Transacciones = lista;
            }

            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Ingresos(string mes)
        {
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync($"https://localhost:7196/api/ingresos/{mes}");
                var xml = await response.Content.ReadAsStringAsync();

                var doc = new System.Xml.XmlDocument();
                doc.LoadXml(xml);

                var bancos = doc.SelectNodes("//banco");

                List<string> nombres = new List<string>();
                List<decimal> valores = new List<decimal>();

                if (bancos != null)
                {
                    foreach (System.Xml.XmlNode b in bancos)
                    {
                        string nombre = b["nombre"]?.InnerText;
                        decimal total = 0;

                        if (b["monto"] != null)
                        {
                            decimal.TryParse(b["monto"].InnerText, out total);
                        }
                        else
                        {
                            var meses = b.SelectNodes("mes");

                            if (meses != null)
                            {
                                foreach (System.Xml.XmlNode m in meses)
                                {
                                    decimal temp = 0;
                                    decimal.TryParse(m["total"]?.InnerText, out temp);
                                    total += temp;
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(nombre))
                        {
                            nombres.Add(nombre);
                            valores.Add(total);
                        }
                    }
                }

                ViewBag.Mes = mes;

                ViewBag.Nombres = nombres;
                ViewBag.Valores = valores;
            }

            return View("IngrersosView");
        }
        

        public async Task<IActionResult> EstadoCuentaPDF(string nit)
        {
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync($"https://localhost:7196/api/estadoCuenta/{nit}");
                var xml = await response.Content.ReadAsStringAsync();

                var docXml = new System.Xml.XmlDocument();
                docXml.LoadXml(xml);

                var transacciones = docXml.SelectNodes("//transaccion");

                MemoryStream ms = new MemoryStream();

                PdfWriter writer = new PdfWriter(ms);
                PdfDocument pdf = new PdfDocument(writer);
                Document document = new Document(pdf);

                document.Add(new Paragraph("Estado de Cuenta"));
                document.Add(new Paragraph(" "));

                Table table = new Table(3);

                table.AddHeaderCell("FECHA");
                table.AddHeaderCell("CARGO");
                table.AddHeaderCell("ABONO");

                if (transacciones != null)
                {
                    foreach (System.Xml.XmlNode t in transacciones)
                    {
                        string fecha = t["fecha"]?.InnerText;
                        string tipo = t["tipo"]?.InnerText;
                        string monto = t["monto"]?.InnerText;
                        string banco = t["banco"]?.InnerText;
                        string numero = t["numero"]?.InnerText;

                        table.AddCell(fecha);

                        if (tipo == "Pago")
                        {
                            table.AddCell("Q " + monto + " (" + banco + ")");
                            table.AddCell("");
                        }
                        else
                        {
                            table.AddCell("");
                            table.AddCell("Q " + monto + " (Fact. " + numero + ")");
                        }
                    }
                }

                document.Add(table);
                document.Close();

                return File(ms.ToArray(), "application/pdf", "EstadoCuenta.pdf");
            }
        }

        public async Task<IActionResult> IngresosPDF(string mes)
        {
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync($"https://localhost:7196/api/ingresos/{mes}");
                var xml = await response.Content.ReadAsStringAsync();

                var docXml = new System.Xml.XmlDocument();
                docXml.LoadXml(xml);

                var nodos = docXml.SelectNodes("//banco");

                MemoryStream ms = new MemoryStream();

                PdfWriter writer = new PdfWriter(ms);
                PdfDocument pdf = new PdfDocument(writer);
                Document document = new Document(pdf);

                document.Add(new Paragraph("Reporte de Ingresos"));
                document.Add(new Paragraph("Mes: " + mes));
                document.Add(new Paragraph(" "));

                Table table = new Table(2);

                table.AddHeaderCell("BANCO");
                table.AddHeaderCell("INGRESO");

                if (nodos != null)
                {
                    foreach (System.Xml.XmlNode n in nodos)
                    {
                        string nombre = n["nombre"]?.InnerText;
                        string monto = n["monto"]?.InnerText;

                        table.AddCell(nombre);
                        table.AddCell("Q " + monto);
                    }
                }

                document.Add(table);
                document.Close();

                return File(ms.ToArray(), "application/pdf", "Ingresos.pdf");
            }
        }
        public async Task<IActionResult> EstadoCuentaTodos()
        {
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync("https://localhost:7196/api/estadoCuenta/todos");
                var xml = await response.Content.ReadAsStringAsync();

                var doc = new System.Xml.XmlDocument();
                doc.LoadXml(xml);

                var clientes = doc.SelectNodes("//cliente");

                List<dynamic> lista = new List<dynamic>();

                foreach (System.Xml.XmlNode c in clientes)
                {
                    lista.Add(new
                    {
                        NIT = c["NIT"]?.InnerText,
                        Nombre = c["nombre"]?.InnerText,
                        Saldo = c["saldoFavor"]?.InnerText
                    });
                }

                ViewBag.Clientes = lista;
            }

            return View();
        }
    }
}

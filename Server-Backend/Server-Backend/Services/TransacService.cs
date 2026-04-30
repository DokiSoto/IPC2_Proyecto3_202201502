using System.Linq;
using System.Xml;
using Server_Backend.Models;
namespace Server_Backend.Services
{
    public class TransacService
    {
        public RespuestaTransac Procesar(string xml)
        {
            RespuestaTransac respuesta = new RespuestaTransac();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            var facturas = doc.GetElementsByTagName("factura");

            foreach (XmlNode nodo in facturas)
            {
                string numero = nodo["numeroFactura"]?.InnerText;
                string nit = nodo["NITcliente"]?.InnerText;
                string fechaStr = nodo["fecha"]?.InnerText;
                string valorStr = nodo["valor"]?.InnerText;

                if(string.IsNullOrEmpty(numero) || string.IsNullOrEmpty(nit))
                {
                    respuesta.FacturasconError++;
                    continue;
                }

                var existente  = DataStore.Facturas
                    .Find(f => f.Numero == numero);

                if(existente != null)
                {
                    respuesta.FacturasDuplicadas++;
                    continue;
                }

                DateTime fecha = DateTime.Parse(fechaStr);
                double valor = double.Parse(valorStr);

                Factura nuevaFactura = new Factura
                {
                    Numero = numero,
                    NIT = nit,
                    Fecha = fecha,
                    Total = valor,
                    SaldoPendiente = valor
                };

                var cliente = DataStore.Clientes.FirstOrDefault(c => c.NIT == nit);

                if(cliente != null && cliente.SaldoFavor > 0)
                {
                    if (cliente.SaldoFavor >= valor)
                    {
                        cliente.SaldoFavor -= valor;
                        nuevaFactura.SaldoPendiente = 0;
                    }
                    else
                    {
                        nuevaFactura.SaldoPendiente -= cliente.SaldoFavor;
                        cliente.SaldoFavor = 0;
                    }
                }

                DataStore.Facturas.Add(nuevaFactura);   
                respuesta.NuevasFactiras++;

            }
            var pagos = doc.GetElementsByTagName("pago");

            foreach (XmlNode nodo in pagos)
            {
                string codigoStr = nodo["codigoBanco"]?.InnerText;
                string fechaStr = nodo["fecha"]?.InnerText;
                string nit = nodo["NITcliente"]?.InnerText;
                string valorStr = nodo["valor"]?.InnerText;

                if (string.IsNullOrEmpty(nit) || string.IsNullOrEmpty(valorStr))
                {
                    respuesta.PagosconError++;
                    continue;
                }

                int codigo = int.Parse(codigoStr);
                DateTime fecha = DateTime.Parse(fechaStr);
                double monto = double.Parse(valorStr);

                var clienteExiste = DataStore.Clientes.Any(c => c.NIT == nit);
                if (!clienteExiste)
                {
                    respuesta.PagosconError++;
                    continue;
                }

                var bancoExiste = DataStore.Bancos.Any(b => b.Codigo == codigo);
                if (!bancoExiste)
                {
                    respuesta.PagosconError++;
                    continue;
                }

                var pagoExistente = DataStore.Pagos.FirstOrDefault(p =>
                    p.NIT == nit &&
                    p.Fecha == fecha &&
                    p.Monto == monto &&
                    p.CodigoBanco == codigo
                );

                if(pagoExistente != null)
                {
                    respuesta.PagosDuplicados++;
                    continue;
                }

                DataStore.Pagos.Add(new Pago
                {
                    CodigoBanco = codigo,
                    Fecha = fecha,
                    NIT = nit,
                    Monto = monto
                });

                double restante = monto;

                var facturasCliente = DataStore.Facturas
                    .Where(f => f.NIT == nit && f.SaldoPendiente > 0)
                    .OrderBy(f => f.Fecha)
                    .ToList();

                foreach (var factura in facturasCliente)
                {
                    if (restante <= 0)
                        break;

                    if (restante >= factura.SaldoPendiente)
                    {
                        restante -= factura.SaldoPendiente;
                        factura.SaldoPendiente = 0;
                    }
                    else
                    {
                        factura.SaldoPendiente -= restante;
                        restante = 0;
                    }
                }

                if(restante > 0)
                {
                    var cliente = DataStore.Clientes
                        .First(c => c.NIT == nit);

                    cliente.SaldoFavor += restante;
                }


                respuesta.NuevosPagos++;
            }
            return respuesta;
        }
    }
}

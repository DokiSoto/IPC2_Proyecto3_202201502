using System.Xml;
using Server_Backend.Models;

namespace Server_Backend.Services
{
    public class ConfigService
    {
        public RespuestaConfig ProcesarXML(string xml)
        {
            RespuestaConfig respuesta = new RespuestaConfig();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            var clientes = doc.GetElementsByTagName("cliente");

            foreach (XmlNode nodo in clientes)
            {
                string nit = nodo["NIT"].InnerText;
                string nombre = nodo["nombre"].InnerText;

                var existente = DataStore.Clientes
                    .Find(c => c.NIT == nit);

                if (existente == null)
                {
                    DataStore.Clientes.Add(new Cliente
                    {
                        NIT = nit,
                        Nombre = nombre
                    });

                    respuesta.ClientesCreados++;
                }
                else
                {
                    existente.Nombre = nombre;
                    respuesta.ClientesActualizados++;
                }
            }

            var bancos = doc.GetElementsByTagName("banco");

            foreach (XmlNode nodo in bancos)
            {
                int codigo = int.Parse(nodo["codigo"].InnerText);
                string nombre = nodo["nombre"].InnerText;

                var existente = DataStore.Bancos
                    .Find(b => b.Codigo == codigo);

                if (existente == null)
                {
                    DataStore.Bancos.Add(new Banco
                    {
                        Codigo = codigo,
                        Nombre = nombre
                    });

                    respuesta.BancosCreados++;
                }
                else
                {
                    existente.Nombre = nombre;
                    respuesta.BancosActualizados++;
                }
            }

            FileService fileService = new FileService();
            fileService.GuardarClientes(DataStore.Clientes);
            fileService.GuardarBancos(DataStore.Bancos);

            return respuesta;
        }
    }
}
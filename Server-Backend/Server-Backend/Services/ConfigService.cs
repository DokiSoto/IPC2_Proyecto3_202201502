using Server_Backend.Models;
using System.Xml;
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
            var bancos = doc.GetElementsByTagName("banco");

            respuesta.ClientesCreados = clientes.Count;
            respuesta.BancosCreados = bancos.Count;

            return respuesta;
        }
    }
}

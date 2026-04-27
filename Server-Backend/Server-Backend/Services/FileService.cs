using System.ComponentModel;
using System.Xml;
using Server_Backend.Models;
namespace Server_Backend.Services
{
    public class FileService
    {
        private string rutaClientes = "Data/clientes.xml";
        private string rutaBancos = "Data/bancos.xml";

        public void GuardarClientes(List<Cliente> clientes)
        {
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

                cliente.AppendChild(nit);
                cliente.AppendChild(nombre);

                root.AppendChild(cliente);
            }

            doc.Save(rutaClientes);
        }

        public void GuardarBancos(List<Banco> bancos)
        {
            XmlDocument doc = new XmlDocument();
            XmlElement root = doc.CreateElement("bancos");
            doc.AppendChild(root);

            foreach (var b in bancos)
            {
                XmlElement banco = doc.CreateElement("banco");

                XmlElement codigo = doc.CreateElement("codigo");
                codigo.InnerText = b.Codigo.ToString();

                XmlElement nombre = doc.CreateElement("nombre");
                nombre.InnerText = b.Nombre;

                banco.AppendChild(codigo);
                banco.AppendChild(nombre);

                root.AppendChild(banco);
            }

            doc.Save(rutaBancos);

        }

        public List<Cliente> LeerClientes()
        {
            List<Cliente> lista = new List<Cliente>();

            if (!File.Exists(rutaClientes))
                return lista;

            XmlDocument doc = new XmlDocument();
            doc.Load(rutaClientes);

            var nodos = doc.GetElementsByTagName("cliente");

            foreach (XmlNode nodo in nodos)
            {
                lista.Add(new Cliente
                {
                    NIT = nodo["NIT"].InnerText,
                    Nombre = nodo["nombre"].InnerText
                });
            }

            return lista;


        }

        public List<Banco> LeerBancos()
        {
            List<Banco> lista = new List<Banco>();

            if (!File.Exists(rutaBancos))
                return lista;

            XmlDocument doc = new XmlDocument();
            doc.Load(rutaBancos);

            var nodos = doc.GetElementsByTagName("banco");

            foreach (XmlNode nodo in nodos)
            {
                lista.Add(new Banco
                {
                    Codigo = int.Parse(nodo["codigo"].InnerText),
                    Nombre = nodo["nombre"].InnerText
                });
            }

            return lista;
        }
    }
}
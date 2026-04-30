using Microsoft.AspNetCore.Mvc;
using Server_Backend.Models;
using System.IO;
namespace Server_Backend.Controllers
{
    
        [ApiController]
        [Route("api/reset")]
        public class ResetController : ControllerBase
        {
            [HttpPost]
            public IActionResult Reset()
            {
                DataStore.Clientes.Clear();
                DataStore.Bancos.Clear();
                DataStore.Facturas.Clear();
            DataStore.Pagos.Clear();

                string rutaClientes = "Data/clientes.xml";
                string rutaBancos = "Data/bancos.xml";

                if(System.IO.File.Exists(rutaClientes))
                    System.IO.File.Delete(rutaClientes);

                if (System.IO.File.Exists(rutaBancos))
                    System.IO.File.Delete(rutaBancos);

                return Content("<mensaje> Sistema Eliminado por completo</mensaje>", "application/xml");

            }
        }
    }


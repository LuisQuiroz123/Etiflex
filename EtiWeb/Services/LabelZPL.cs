using System.Text;
using WebApiData = EtiWeb.Data;

namespace EtiWeb.Services
{
    public class LabelZPL
    {
        // Opcional: logo en ZPL, puedes reemplazarlo con tu propio logo
        private const string LogoZpl = ""; // Ejemplo: "^FO10,10^GFA,...^FS"

        /// <summary>
        /// Genera ZPL para todas las etiquetas de un Request
        /// </summary>
        /// <param name="request">Objeto Request con sus archivos y datos</param>
        /// <returns>String con ZPL listo para imprimir</returns>
        public string GenerateZpl(WebApiData.Request request)
        {
            var zplBuilder = new StringBuilder();

            foreach (var file in request.Files)
            {
                for (int i = 0; i < file.TotalLabels; i++)
                {
                    zplBuilder.AppendLine("^XA"); // Inicio de etiqueta

                    // Logo (opcional)
                    if (!string.IsNullOrEmpty(LogoZpl))
                        zplBuilder.AppendLine(LogoZpl);

                    // Pedido y cliente
                    zplBuilder.AppendLine("^FO10,60^A0N,35,35^FDPedido:^FS");
                    zplBuilder.AppendLine($"^FO150,60^A0N,35,35^FD{request.RequestNumber}^FS");

                    zplBuilder.AppendLine("^FO10,100^A0N,30,30^FDCliente:^FS");
                    zplBuilder.AppendLine($"^FO150,100^A0N,30,30^FD{request.ClientData_ClientName}^FS");

                    // Dirección
                    string direccion = $"{request.ClientData_AddressLine1} {request.ClientData_AddressLine2} {request.ClientData_AddressLine3}";
                    zplBuilder.AppendLine("^FO10,140^A0N,25,25^FDDirección:^FS");
                    zplBuilder.AppendLine($"^FO150,140^A0N,25,25^FD{direccion}^FS");

                    // Tipo de entrega
                    zplBuilder.AppendLine("^FO10,180^A0N,25,25^FDTipo de Entrega:^FS");
                    zplBuilder.AppendLine($"^FO150,180^A0N,25,25^FD{request.DeliveryType}^FS");

                    // Archivo
                    zplBuilder.AppendLine("^FO10,220^A0N,25,25^FDArchivo:^FS");
                    zplBuilder.AppendLine($"^FO150,220^A0N,25,25^FD{file.Name}^FS");

                    // Notas (si existen)
                    if (!string.IsNullOrEmpty(request.Notes))
                    {
                        zplBuilder.AppendLine("^FO10,260^A0N,20,20^FDNotas:^FS");
                        zplBuilder.AppendLine($"^FO150,260^A0N,20,20^FD{request.Notes}^FS");
                    }

                    // Código de barras
                    zplBuilder.AppendLine($"^FO10,300^BY2^BCN,100,Y,N,N^FD{request.RequestNumber}^FS");

                    zplBuilder.AppendLine("^XZ"); // Fin de etiqueta
                }
            }

            return zplBuilder.ToString();
        }



        public string GenerateZplForSato(WebApiData.Request request)
        {
            // Ajusta el ZPL según el estándar Sato
            var zplBuilder = new StringBuilder();
            foreach (var file in request.Files)
            {
                for (int i = 0; i < file.TotalLabels; i++)
                {
                    zplBuilder.AppendLine("^XA");
                    // Aquí se podría ajustar fuente, posiciones o códigos especiales Sato
                    zplBuilder.AppendLine($"^FO10,10^FD{request.RequestNumber}^FS");
                    zplBuilder.AppendLine("^XZ");
                }
            }
            return zplBuilder.ToString();
        }


        public string GenerateZplForGodex(WebApiData.Request request)
        {
            // Ajusta el ZPL para Godex
            var zplBuilder = new StringBuilder();
            foreach (var file in request.Files)
            {
                for (int i = 0; i < file.TotalLabels; i++)
                {
                    zplBuilder.AppendLine("^XA");
                    zplBuilder.AppendLine($"^FO10,10^FD{request.RequestNumber}^FS");
                    zplBuilder.AppendLine("^XZ");
                }
            }
            return zplBuilder.ToString();
        }






        /// <summary>
        /// Opcional: enviar ZPL a impresora Zebra por IP y puerto 9100
        /// </summary>
        public void SendToPrinter(string zpl, string ip, int port = 9100)
        {
            using var client = new System.Net.Sockets.TcpClient(ip, port);
            using var stream = client.GetStream();
            var data = Encoding.UTF8.GetBytes(zpl);
            stream.Write(data, 0, data.Length);
        }
    }
}

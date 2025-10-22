using System.Text;

namespace EtiWeb.Services
{
    public class PrinterService
    {
        public async Task<bool> PrintAsync(string printerBrand, string zpl)
        {
            // Aquí tu lógica para enviar ZPL a la impresora según marca
            // Ejemplo: IP hardcodeada o desde configuración del usuario
            try
            {
                string ip = "192.168.1.100"; // ejemplo
                int port = 9100;

                using var client = new System.Net.Sockets.TcpClient(ip, port);
                using var stream = client.GetStream();
                var data = Encoding.UTF8.GetBytes(zpl);
                await stream.WriteAsync(data, 0, data.Length);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Participants
{
    public class ServerConnection
    {
        private readonly string host;
        private readonly int port;

        public ServerConnection(string host = "127.0.0.1", int port = 8080)
        {
            this.host = host;
            this.port = port;
        }

        public string SendRequest(string request)
        {
            try
            {
                using (TcpClient client = new TcpClient(host, port))
                using (NetworkStream stream = client.GetStream())
                {
                    byte[] data = Encoding.UTF8.GetBytes(request);
                    stream.Write(data, 0, data.Length);

                    byte[] buffer = new byte[1024];
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    return Encoding.UTF8.GetString(buffer, 0, bytesRead);
                }
            }
            catch (Exception ex)
            {
                return "FAIL|Không thể kết nối đến server: " + ex.Message;
            }
        }
    }
}

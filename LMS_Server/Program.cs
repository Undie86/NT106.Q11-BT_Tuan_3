using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS_Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "LMS Server";

            try
            {
                Server server = new Server();
                server.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khởi động server: " + ex.Message);
            }

            Console.WriteLine("Nhấn phím bất kỳ để thoát...");
            Console.ReadKey();
        }
    }
}

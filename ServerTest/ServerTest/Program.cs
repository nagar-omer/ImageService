using ImageService.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerTest
{
    class Program
    {
        static void Main(string[] args)
        {
            TCPServer Server = new TCPServer();
            Server.Listener();
            Server.ReadMessage();

        }
    }
}

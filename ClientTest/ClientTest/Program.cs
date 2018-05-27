using ImageServiceGui.Client;
using ImageServiceGui.ClientNameSpace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientTest
{
    class Program
    {
        static void Main(string[] args)
        {
            
            TCPClient client = new TCPClient();
            client.Connect();
            Message tryMessage = new Message(1, "check123");
            client.SendData(tryMessage);
        }
    }
}

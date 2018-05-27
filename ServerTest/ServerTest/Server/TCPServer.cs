
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Server
{
    class TCPServer
    {
        public IPEndPoint ep;
        public TcpListener listener;
        public NetworkStream stream;
        public BinaryReader reader;
        public BinaryWriter writer;
        public TcpClient client;

        public NetworkStream Stream { get => stream; set => stream = value; }
        public BinaryReader Reader { get => reader; set => reader = value; }
        public BinaryWriter Writer { get => writer; set => writer = value; }

        public TCPServer()
        {
            ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8000);
            listener = new TcpListener(ep);

        }
        public void Listener()
        {
            listener.Start();
            Console.WriteLine("Waiting for client connections...");
            client = listener.AcceptTcpClient();
            Console.WriteLine("Client connected");
            NetworkStream stream = client.GetStream();
            BinaryReader reader = new BinaryReader(stream);
            BinaryWriter writer = new BinaryWriter(stream);
            Console.WriteLine("Waiting for a message");
        }
        public string ReadMessage()
        {
            int type = this.reader.ReadInt32();
            int textLen = reader.ReadInt32();
            byte[] buffer = new byte[textLen];
            int bytesRead = Reader.Read(buffer, 0, textLen);
            string retStr = type + ";" + System.Text.Encoding.Default.GetString(buffer);
            return retStr;
        }
        //using (
        //{

        //int num = reader.ReadInt32();
        //        Console.WriteLine("Number accepted");
        //num *= 2;
        //writer.Write(num);
        //}
        public void CloseServer()
        {
            client.Close();
            listener.Stop();
        }
    }
}
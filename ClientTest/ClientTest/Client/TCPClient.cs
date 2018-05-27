using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using ImageServiceGui.Client;

namespace ImageServiceGui.ClientNameSpace
{
    class TCPClient
    {
        public IPEndPoint ep;
        public TcpClient client;
        NetworkStream stream;
        BinaryReader reader;
        BinaryWriter writer;
        public TCPClient()
        {
            this.ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8000);
            this.client = new TcpClient();
            
        }
        public void Connect() {
            client.Connect(ep);
            this.stream = client.GetStream();
            this.reader = new BinaryReader(stream);
            this.writer = new BinaryWriter(stream);
        }
        public void SendData(Message theMessage)
        {
            int messageTypeToSend = theMessage.MessageType;
            string messageTextToSend = theMessage.MessageString;

            byte[] bytesToSendText = ASCIIEncoding.ASCII.GetBytes(messageTextToSend);

            //---send the type---
            Console.WriteLine("Sending : " + messageTypeToSend);
            writer.Write(messageTypeToSend);
            //---send text length---
            Console.WriteLine("Sending : " + bytesToSendText.Length);
            writer.Write(bytesToSendText.Length);
            //---send the text---
            Console.WriteLine("Sending : " + messageTextToSend);
            writer.Write(bytesToSendText, 0, bytesToSendText.Length);

            //// Get result from server
            //int result = reader.ReadInt32();
            //Console.WriteLine("Result = {0}", result);
        }
        public Message ReadData()
        {
            int type = reader.ReadInt32();
            int textLen = reader.ReadInt32();
            byte[] buffer = new byte[textLen];
            int bytesRead = reader.Read(buffer, 0, textLen);
            string messageText = System.Text.Encoding.Default.GetString(buffer);
            return new Message(type, messageText);

        }
        public void CloseClient()
        {
            client.Close();
        }
    }
}

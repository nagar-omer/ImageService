using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageServiceGui.Client
{
    class Message
    {
        public int MessageType { set; get; }
        public string MessageString { set; get; }
        public Message(int _messageType, string _messageText)
        {
            this.MessageType = _messageType;
            this.MessageString = _messageText;
        }
    }
}

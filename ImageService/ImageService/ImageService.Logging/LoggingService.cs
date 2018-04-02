﻿
using ImageService.Logging.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Logging
{
    public class LoggingService : ILoggingService
    {
        public event EventHandler<MessageRecievedEventArgs> MessageRecieved;
        public void Log(string message, MessageTypeEnum type)
        {
            var temp = new MessageRecievedEventArgs();
            temp.Status = type;
            temp.Message = message;
            MessageRecieved(this, temp);
        }
    }
}

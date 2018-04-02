using ImageService.Controller;
using ImageService.Controller.Handlers;
using ImageService.Infrastructure.Enums;
using ImageService.Logging;
using ImageService.Logging.Modal;
using ImageService.Modal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Server
{
    public class ImageServer
    {
        #region Members
        private EventLog ServerLogger;
        private IImageController m_controller;
        private ILoggingService m_logging;
        #endregion

        #region Properties
        public event EventHandler<CommandRecievedEventArgs> CommandRecieved;          // The event that notifies about a new Command being recieved
        #endregion

        public ImageServer() {
            ServerLogger = new System.Diagnostics.EventLog();
            m_logging.MessageRecieved += this.LogHandler;
        }

        private void LogHandler(object sender, MessageRecievedEventArgs args) {
            switch (args.Status) {
                case MessageTypeEnum.INFO:
                    ServerLogger.WriteEntry(args.Message, EventLogEntryType.Information);
                    break;
                case MessageTypeEnum.WARNING:
                    ServerLogger.WriteEntry(args.Message, EventLogEntryType.Warning);
                    break;
                case MessageTypeEnum.FAIL:
                    ServerLogger.WriteEntry(args.Message, EventLogEntryType.FailureAudit);
                    break;
                default:
                    ServerLogger.WriteEntry(args.Message, EventLogEntryType.Information);
                    break;
            }

        }

    }
}

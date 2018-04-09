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
using System.Configuration;

namespace ImageService.Server
{
    // TODO:    IMPLEMENT APP CONFIG
    public class ImageServer
    {
        #region Members
        private EventLog ServerLogger;
        private ILoggingService m_logging;
        private IImageServiceModal m_service;
        private Dictionary<string, DirectoyHandler> m_handlers;
        #endregion

        #region Properties
        public event EventHandler<CommandRecievedEventArgs> CommandRecieved;          // The event that notifies about a new Command being recieved
        #endregion

        public void RecieveCommand(CommandRecievedEventArgs args) {
            m_logging.Log("server recieved new comand " + args.CommandID, MessageTypeEnum.INFO);
            CommandRecieved(this, args);
        }

        public void watch_dir(string path_dir_to_watch) {
            m_logging.Log("watch dir: " + path_dir_to_watch, MessageTypeEnum.INFO);
            // TODO:    init m_handler
            // we should be able to watch mor then one directory, so this should actualy be a list
            // acordingly we will need to have list of handlers

            // or maybe dictionary for them both...
            m_handlers.Add(path_dir_to_watch, new DirectoyHandler(m_service, m_logging));
            m_handlers[path_dir_to_watch].StartHandleDirectory(path_dir_to_watch);
            CommandRecieved += m_handlers[path_dir_to_watch].OnCommandRecieved;
        }

        // server constructor
        public ImageServer(string outDir ,ILoggingService logger, int thumbSize = 40) {
            m_handlers = new Dictionary<string, DirectoyHandler>();
            m_logging = logger;
            // service modal - responsible for managing output_dir for sorted images
            m_service = new ImageServiceModal(outDir, thumbSize, logger);


            // TODO:    set logger path
            ServerLogger = new System.Diagnostics.EventLog("Application");
            //ServerLogger.Source = ConfigurationManager.AppSettings["SourceName"];
            ServerLogger.Source = "Application";
            m_logging.MessageRecieved += this.LogHandler;
            m_logging.Log("out: " + outDir, MessageTypeEnum.INFO);
            m_logging.Log("server constructor finished", MessageTypeEnum.INFO);
        }

        // Log handler - activated trough m_logging.MessageRecieved
        // writing to log acording to the message type
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

using ImageService.Modal;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageService.Infrastructure;
using ImageService.Infrastructure.Enums;
using ImageService.Logging;
using ImageService.Logging.Modal;
using System.Text.RegularExpressions;
using System.Diagnostics;


namespace ImageService.Controller.Handlers
{
    enum COMMAND { NEW=1, RENAME, DELETE};
    // this class is responsible for the directory that is being watched 
    // - it contains a filewatcher that alerts it if chanches have been made to this directory
    // - its asosiated to the commands executed in the server
    // ** important - any changes to the output dir is made only by image_service_modal that is given to the constructor
    public class DirectoyHandler : IDirectoryHandler
    {
        #region Members
        private static string[] filters;
        private EventLog DirHanlerLogeer;
        private IImageController m_controller;              // The Image Processing Controller
        private ILoggingService m_logging;
        private FileSystemWatcher m_dirWatcher;             // The Watcher of the Dir
        private string m_path;                              // The Path of directory
        #endregion
        // TODO:    DirectoryClose not used yet
        public event EventHandler<DirectoryCloseEventArgs> DirectoryClose;              // The Event That Notifies that the Directory is being closed

        // constructor - á directory wont be watched untill a call is made to StartHandleDirectory
        public DirectoyHandler(IImageServiceModal modal, ILoggingService logger) {
            // the controller uses functions in the modal to execute methods
            m_controller = new ImageController(modal);
            BuildLogger(logger);
        }

        // start watching this directory
        public void StartHandleDirectory(string dirPath) {
            m_path = dirPath;
            FileWatcherBuild();
        }

        // handles commands from server 
        // no comands from server at this point of the ex
        public void OnCommandRecieved(object sender, CommandRecievedEventArgs e) {
            bool flag;
            // TODO:    execute command 
            // figure out what to send to the execute comand
            m_controller.ExecuteCommand(e.CommandID, e.Args, out flag);
        }

        // initiate logger
        private void BuildLogger(ILoggingService logger) {
            m_logging = logger;
            // TODO:    set logger path
            DirHanlerLogeer = new System.Diagnostics.EventLog("Application");
            DirHanlerLogeer.Source = "Application";
            m_logging.MessageRecieved += this.LogHandler;
        }

        // Log handler - activated trough m_logging.MessageRecieved
        // writing to log acording to the message type
        private void LogHandler(object sender, MessageRecievedEventArgs args) {
            switch (args.Status) {
                case MessageTypeEnum.INFO:
                    DirHanlerLogeer.WriteEntry(args.Message, EventLogEntryType.Information);
                    break;
                case MessageTypeEnum.WARNING:
                    DirHanlerLogeer.WriteEntry(args.Message, EventLogEntryType.Warning);
                    break;
                case MessageTypeEnum.FAIL:
                    DirHanlerLogeer.WriteEntry(args.Message, EventLogEntryType.FailureAudit);
                    break;
                default:
                    DirHanlerLogeer.WriteEntry(args.Message, EventLogEntryType.Information);
                    break;
            }

        }

        // building fileWatcher
        private void FileWatcherBuild() {
            m_dirWatcher = new FileSystemWatcher(m_path);
            // list of types to watch 
            m_dirWatcher.Filter = "*";
            filters = new string[] { ".jpg", ".png", ".gif", ".bmp" };
            m_dirWatcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            
            // ataching a function for each of the cases
            // the function from the right will be activated when a change of the matching type is accuring
            m_dirWatcher.Changed += HandlerChanged;
            m_dirWatcher.Deleted += HandlerDeleated;
            m_dirWatcher.Created += HandlerCreated;
            m_dirWatcher.Renamed += HandlerRenamed;

            // start watching!!
            m_dirWatcher.EnableRaisingEvents = true;
        }

        private void HandlerChanged(object sender, FileSystemEventArgs args) {
            if (filters.Contains(Path.GetExtension(args.FullPath))) {
                m_logging.Log("HandlerChanged", MessageTypeEnum.INFO);
            }
        }

        private void HandlerCreated(object sender, FileSystemEventArgs args) {
            // check type is matching to .jpg/.png.....
            if (filters.Contains(Path.GetExtension(args.FullPath))) {
                m_logging.Log("HandlerCreated", MessageTypeEnum.INFO);
                // indication var
                bool result;
                // args - full path of created file 
                string[] argsForCommand = new string[1];
                argsForCommand[0] = args.FullPath;
                // activate command {1: NewFileCommand}, with args of the file that was created
                m_controller.ExecuteCommand(1, argsForCommand, out result);
            }
        }

        private void HandlerDeleated(object sender, FileSystemEventArgs args) {
            if (filters.Contains(Path.GetExtension(args.FullPath))) {
                m_logging.Log("HandlerDeleated", MessageTypeEnum.INFO);
                // indication var
                bool result;
                // args - full path of created file 
                string[] argsForCommand = new string[1];
                argsForCommand[0] = args.FullPath;
                // activate command {1: NewFileCommand}, with args of the file that was created
                // TODO:    create Enum to replace 1
                m_controller.ExecuteCommand(3, argsForCommand, out result);
            }
        }

        private void HandlerRenamed(object sender, FileSystemEventArgs args) {
            if (filters.Contains(Path.GetExtension(args.FullPath))) {
                m_logging.Log("HandlerRenamed", MessageTypeEnum.INFO);
            }
        }
    }
}

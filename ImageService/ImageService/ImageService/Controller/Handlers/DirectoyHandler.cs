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

        public void StartHandleDirectory(string dirPath) {
            m_path = dirPath;
            FileWatcherBuild();
        }

        public void OnCommandRecieved(object sender, CommandRecievedEventArgs e) {
            bool flag;
            m_controller.ExecuteCommand(e.CommandID, e.Args, out flag);
        }

        public event EventHandler<DirectoryCloseEventArgs> DirectoryClose;              // The Event That Notifies that the Directory is being closed

        private void BuildLogger() {
            DirHanlerLogeer = new System.Diagnostics.EventLog();
            m_logging.MessageRecieved += this.LogHandler;
        }

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
        private void FileWatcherBuild() {
            filters = new string[] { ".jpg", ".png", ".gif", ".bmp" };
            var m_dirWatcher = new FileSystemWatcher(m_path);
            m_dirWatcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            m_dirWatcher.Filter = "*";

            m_dirWatcher.Changed += HandlerChanged;
            m_dirWatcher.Deleted += HandlerDeleated;
            m_dirWatcher.Created += HandlerCreated;
            m_dirWatcher.Renamed += HandlerRenamed;

            m_dirWatcher.EnableRaisingEvents = true;
        }

        private void HandlerChanged(object sender, FileSystemEventArgs args) {
            if (filters.Contains(Path.GetExtension(args.FullPath))) {
                // file changed logic
            }
        }

        private void HandlerCreated(object sender, FileSystemEventArgs args) {
            if (filters.Contains(Path.GetExtension(args.FullPath))) {
                // file created logic
            }
        }

        private void HandlerDeleated(object sender, FileSystemEventArgs args) {
            if (filters.Contains(Path.GetExtension(args.FullPath))) {
                // file deleted logic
            }
        }

        private void HandlerRenamed(object sender, FileSystemEventArgs args) {
            if (filters.Contains(Path.GetExtension(args.FullPath))) {
                // file renamed logic
            }
        }
    }
}

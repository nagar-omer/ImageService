using ImageService.Logging;
using ImageService.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace ImageService {
    public partial class Service1 : ServiceBase {
        private string outdir_path;

        public Service1() {
            InitializeComponent();
        }

        public void OnDebug() {
            OnStart(null);
        }
        protected override void OnStart(string[] args) {
            int thumbSize = Int32.Parse(ConfigurationManager.AppSettings["ThumbnailSize"]);
            outdir_path = ConfigurationManager.AppSettings["OutputDir"];
            var logger = new LoggingService();
            var server = new ImageServer(outdir_path, logger, thumbSize);

            int numFolders = Int32.Parse(ConfigurationManager.AppSettings["NumFoldersToWatch"]);
            for(int i = 1; i<= numFolders; i++)
                server.watch_dir(ConfigurationManager.AppSettings["Handler_" + i]);
            
        }

        protected override void OnStop() {
        }
    }
}

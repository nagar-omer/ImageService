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

            //take handler as string of path and split it to paths (as strings) array
            string phrase = ConfigurationManager.AppSettings["Handler"];
            string[] dirPaths = phrase.Split(';');
            for (int i = 0; i < numFolders; i++)
            {
                var a = dirPaths[i];
                server.watch_dir(dirPaths[i]);
            }
        }

        protected override void OnStop() {
            //DictonaryXml.DictToXml()
        }
    }
}

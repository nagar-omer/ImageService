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
            outdir_path = "output";
            var logger = new LoggingService();
            var server = new ImageServer(outdir_path, logger);
            server.watch_dir("to_watch");
        }

        protected override void OnStop() {
        }
    }
}

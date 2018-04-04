using ImageService.Commands;
using ImageService.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Commands {
    class DeleteFileCommand : ICommand {
        private IImageServiceModal m_modal;
        private string file_path;

        public DeleteFileCommand(IImageServiceModal modal) {
            m_modal = modal;            // Storing the Modal
        }

        public string Execute(string[] args, out bool result) {
            // args[0] will contain full path of file created in the original dir  
            return m_modal.DeleteFile(args[0], out result);
            // The String Will Return the New Path if result = true, and will return the error message
        }
    }
}

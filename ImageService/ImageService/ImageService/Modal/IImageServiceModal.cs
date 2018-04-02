using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Modal
{
    public interface IImageServiceModal
    {
        /// <summary>
        /// The Function Addes A file to the system
        /// </summary>
        /// <param name="path">The Path of the Image from the file</param>
        /// <returns>Indication if the Addition Was Successful</returns>
        void AddFile(string path, out bool result);
        void RenameFile(string path, string NewName, out bool result);
        void CreateFolder(string path, out bool result);
        void DeleteFile(string path, out bool result);
        void DeleteFolder(string path, out bool result);
    }
}

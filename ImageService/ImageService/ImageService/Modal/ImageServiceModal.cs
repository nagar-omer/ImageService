using ImageService.Infrastructure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;


// controls OutputDir
namespace ImageService.Modal
{
    public class ImageServiceModal : IImageServiceModal {
        #region Members
        private string m_OutputFolder;            // The Output Folder
        private int m_thumbnailSize;              // The Size Of The Thumbnail Size
        #endregion

        void AddFile(string path, out bool result) {

        }
        void RenameFile(string path, string NewName, out bool result) {

        }
        void CreateFolder(string path, out bool result) {

        }
        void DeleteFile(string path, out bool result) {

        }
        void DeleteFolder(string path, out bool result) {

        }

    }
}

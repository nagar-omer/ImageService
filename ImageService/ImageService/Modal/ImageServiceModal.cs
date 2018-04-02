using ImageService.Infrastructure;
using System;
using System.Collections.Generic;
using System.Drawing;
// TODO:    fix this error ( using Imaging )
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;


// controls OutputDir
namespace ImageService.Modal
{
    public class ImageServiceModal : IImageServiceModal {
        #region Members
        private string m_OutputFolder;            // The Output Folder
        private int m_thumbnailSize;              // The Size Of The Thumbnail Size
        #endregion

        public ImageServiceModal(string outDir, int thumbSize) {
            m_OutputFolder = outDir;
            m_thumbnailSize = thumbSize;
        }
        public string AddFile(string path, out bool result) {
            result = false;
            return "";
            // TODO:    implement AddFile
        }
        public string RenameFile(string path, string NewName, out bool result) {
            result = false;
            return "";
            // TODO:    implement RenameFile
        }
        public string DeleteFile(string path, out bool result) {
            result = false;
            return "";
            // TODO:    implement DeleteFile
        }
        private void CreateFolder(string path, out bool result) {
            result = false;
            // TODO:    implement CreateFolder
        }
        private void DeleteFolder(string path, out bool result) {
            result = false;
            // TODO:    implement DeleteFolder
        }

    }
}

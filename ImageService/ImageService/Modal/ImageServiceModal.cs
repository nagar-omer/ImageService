using ImageService.Infrastructure;
using ImageService.Logging;
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
        private string m_OutputFolder; // The Output Folder
        private string m_TumbFolder;
        private int m_thumbnailSize;
        private DateTime defaultTime;
        private ILoggingService m_loggin;
        private Regex r = new Regex(":");
        // The Size Of The Thumbnail Size
        #endregion

        // constructor 
        public ImageServiceModal(string outDir, int thumbSize, ILoggingService logger) {
            m_OutputFolder = outDir;
            m_TumbFolder = Path.Combine(outDir, "Thumbnail");
            defaultTime = new DateTime(2000, 1, 1);
            m_thumbnailSize = thumbSize;
            m_loggin = logger;
        }
        
        //retrieves the datetime WITHOUT loading the whole image
        private DateTime GetDateTakenFromImage(string path) {
            m_loggin.Log("get time - image service" + path, Logging.Modal.MessageTypeEnum.INFO);
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            using (var myImage = Image.FromStream(fs, false, false)) {
                PropertyItem propItem;
                // try getting taken date time
                try {
                    propItem = myImage.GetPropertyItem(36867);
                }
                catch (Exception e1) {
                    // try getting last modified
                    try {
                        propItem = myImage.GetPropertyItem(306);
                    }
                    // if non of the above exist return current time
                    catch(Exception e2) {
                        return defaultTime;
                    }
                }
                string dateTaken = r.Replace(Encoding.UTF8.GetString(propItem.Value), "-", 2);
                return DateTime.Parse(dateTaken);
            }
        }

        private void create_path(string year, string month) {
            // create output
            CreateFolder(m_OutputFolder);
            // create output/year
            CreateFolder(Path.Combine(m_OutputFolder, year));
            // create output/year/month
            CreateFolder(Path.Combine(m_OutputFolder, year, month));
            // create output/Thumbnail
            CreateFolder(m_TumbFolder);
            // create output/Thumbnail/year
            CreateFolder(Path.Combine(m_TumbFolder, year));
            // create output/Thumbnail/year/month
            CreateFolder(Path.Combine(m_TumbFolder, year, month));
        }

        public string AddFile(string path, out bool result) {
            m_loggin.Log("new file - image service" + path, Logging.Modal.MessageTypeEnum.INFO);
            try {
                var date = GetDateTakenFromImage(path);
                create_path(date.Year.ToString(), date.Month.ToString());
                // copy to sorted folders
                string dest_path = Path.Combine(m_OutputFolder, date.Year.ToString(), date.Month.ToString(), Path.GetFileName(path));
                File.Copy(path, dest_path, true);
                // create Tumbnail and save it to thmbnail dir
                string tumb_path = Path.Combine(m_TumbFolder, date.Year.ToString(), date.Month.ToString(), Path.GetFileName(path));
                Image image = Image.FromFile(path);
                Image thumb = image.GetThumbnailImage(m_thumbnailSize, m_thumbnailSize, () => false, IntPtr.Zero);
                thumb.Save(tumb_path);
                // return success
                result = true;
                return dest_path;
            }
            catch(Exception e) {
                // return fail
                result = false;
                return "";
            }
        }

        public string RenameFile(string path, string NewName, out bool result) {
            // TODO:    implement RenameFile
            result = true;
            try {
                File.Move("oldfilename", "newfilename");
            }
            catch(Exception e) {
                result = false;
            }
            try {
                File.Move("oldfilename", "newfilename");
            }
            catch(Exception e) {
                result = false;
            }
            return "";
        }

        // delete file from sorted and from thumbnails
        public string DeleteFile(string path, out bool result) {
            m_loggin.Log("delete file - image service" + path, Logging.Modal.MessageTypeEnum.INFO);
            var date = GetDateTakenFromImage(path);
            string sort_path = Path.Combine(m_OutputFolder, date.Year.ToString(), date.Month.ToString(), Path.GetFileName(path));
            string tumb_path = Path.Combine(m_TumbFolder, date.Year.ToString(), date.Month.ToString(), Path.GetFileName(path));
            result = true;
            // delete from sorted
            try {
                File.Delete(sort_path);
            }
            catch(Exception e) {
                result = false;
            }
            // delete from tumbnails
            try {
                File.Delete(tumb_path);
            }
            catch (Exception e) {
                result = false;
            }

            // delete empty folders
            DeleteFolder(Path.Combine(m_OutputFolder, date.Year.ToString(), date.Month.ToString()));
            DeleteFolder(Path.Combine(m_OutputFolder, date.Year.ToString()));
            DeleteFolder(Path.Combine(m_TumbFolder, date.Year.ToString(), date.Month.ToString()));
            DeleteFolder(Path.Combine(m_TumbFolder, date.Year.ToString()));
            return sort_path;
        }

        // create folder if it dosn't exist
        private void CreateFolder(string path) {
            if (!Directory.Exists(path)) {
                m_loggin.Log("create dir " + path, Logging.Modal.MessageTypeEnum.INFO);
                DirectoryInfo dir = Directory.CreateDirectory(path);
                dir.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
            }
        }

        // delete folder only if its Empty!!
        private void DeleteFolder(string path) {
            // if directory is empty
            if (!Directory.EnumerateFileSystemEntries(path).Any())
                Directory.Delete(path);
        }
    }
}

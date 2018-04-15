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
using System.Xml.Linq;


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
        Dictionary<string, FileOutInfo> watched;
        private string watche_file_name = "watched.xml";
        private Regex r = new Regex(":");
        // The Size Of The Thumbnail Size
        #endregion

        // constructor 
        public ImageServiceModal(string outDir, int thumbSize, ILoggingService logger) {
            m_OutputFolder = outDir;
            m_TumbFolder = Path.Combine(outDir, "Thumbnail");
            watched = new Dictionary<string, FileOutInfo>();
            defaultTime = new DateTime(2000, 1, 1);
            m_thumbnailSize = thumbSize;
            m_loggin = logger;
        }

        private void Copy(string source, string target) {
            using (var inputFile = new FileStream(
         source,
         FileMode.Open,
         FileAccess.Read,
         FileShare.ReadWrite)) {
                using (var outputFile = new FileStream(target, FileMode.Create)) {
                    var buffer = new byte[0x10000];
                    int bytes;

                    while ((bytes = inputFile.Read(buffer, 0, buffer.Length)) > 0) {
                        outputFile.Write(buffer, 0, bytes);
                    }
                }
            }
        }

        private bool IsFileLocked(string path) {
            FileStream stream = null;
            var file = new FileInfo(path);
            try {
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException) {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
            finally {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
        }

        private void wait_until_unlocked(string path) {
            for (int i = 0; i < 100; i++)
                if (IsFileLocked(path))
                    System.Threading.Thread.Sleep(1000);
        }

        //retrieves the datetime WITHOUT loading the whole image
        private DateTime GetDateTakenFromImage(string path) {
            m_loggin.Log("get time - image service" + path, Logging.Modal.MessageTypeEnum.INFO);
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var myImage = Image.FromStream(fs, false, false)) {
                PropertyItem propItem;
                // try getting taken date time
                try {
                    propItem = myImage.GetPropertyItem(36867);
                }
                catch (Exception e1) {
                    m_loggin.Log("GetDateTakenFromImage_1: " + e1.Message + path, Logging.Modal.MessageTypeEnum.INFO);
                    // try getting last modified
                    try {
                        propItem = myImage.GetPropertyItem(306);
                    }
                    // if non of the above exist return current time
                    catch(Exception e2) {
                        m_loggin.Log("GetDateTakenFromImage_2: " + e2.Message + path, Logging.Modal.MessageTypeEnum.INFO);
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

        private void addToWatched(string path, string file_name,  out string sort_path, out string thumb_path) {
            var date = GetDateTakenFromImage(path);
            create_path(date.Year.ToString(), date.Month.ToString());

            sort_path = Path.Combine(m_OutputFolder, date.Year.ToString(), date.Month.ToString(), file_name);

            if (File.Exists(sort_path)) {
                var has_name = false;
                int i = 0;
                while (!has_name) {
                    sort_path = Path.Combine(m_OutputFolder, date.Year.ToString(), date.Month.ToString(), i + "_" + file_name);
                    if (!File.Exists(sort_path)) {
                        has_name = true;
                        break;
                    }
                    i++;
                }
                
            }
              
            // create Tumbnail and save it to thmbnail dir
            thumb_path = Path.Combine(m_TumbFolder, date.Year.ToString(), date.Month.ToString(), Path.GetFileName(sort_path));
            watched.Add(path, new FileOutInfo());
            watched[path].Time = date;
            watched[path].Name = Path.GetFileName(sort_path);
        }

        public DateTime removeFromWatched(string path, out string sort_path, out string thumb_path) {
            var date = watched[path].Time;
            var file_name = watched[path].Name;
            sort_path = Path.Combine(m_OutputFolder, date.Year.ToString(), date.Month.ToString(), file_name);
            thumb_path = Path.Combine(m_TumbFolder, date.Year.ToString(), date.Month.ToString(), file_name);
            watched.Remove(path);
            return date;
        }
        public string AddFile(string path, string file_name, out bool result) {
            m_loggin.Log("new file - image service" + path, Logging.Modal.MessageTypeEnum.INFO);
            try {
                // wait for file to be unlocked
                wait_until_unlocked(path);
                string dest_path, tumb_path;
                addToWatched(path, file_name, out dest_path, out tumb_path);
                m_loggin.Log("copy from:" + path + " to: " + dest_path , Logging.Modal.MessageTypeEnum.INFO);

                //File.Copy(path, dest_path, false);
                Copy(path, dest_path); 

                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                using (var myImage = Image.FromStream(fs, false, false)) {
                    //Image image = Image.FromFile(path);
                    Image thumb = myImage.GetThumbnailImage(m_thumbnailSize, m_thumbnailSize, () => false, IntPtr.Zero);
                    thumb.Save(tumb_path);
                }
                // return success
                result = true;
                return dest_path;
            }
            catch(Exception e) {
                // return fail
                m_loggin.Log("AddFile: " + e.Message, Logging.Modal.MessageTypeEnum.INFO);
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
            string sort_path, tumb_path;
            var date = removeFromWatched(path, out sort_path, out tumb_path);
            result = true;
            // delete from sorted
            try {
                if (File.Exists(sort_path)) {
                    wait_until_unlocked(sort_path);
                    File.Delete(sort_path);
                }
            }
            catch(Exception e) {
                m_loggin.Log("deleteFile_1: " + e.Message + path, Logging.Modal.MessageTypeEnum.INFO);
                result = false;
            }
            // delete from tumbnails
            try {
                if (File.Exists(tumb_path)) {
                    wait_until_unlocked(tumb_path);
                    File.Delete(tumb_path);
                }
            }
            catch (Exception e) {
                m_loggin.Log("deleteFile_2: " + e.Message + path, Logging.Modal.MessageTypeEnum.INFO);
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
            if (!Directory.EnumerateFileSystemEntries(path).Any()) {
                Directory.Delete(path);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Modal {
    class FileOutInfo {
        public DateTime Time { get; set; }
        public string Name { get; set; }
        public String toStringMy() {
            string info = Name+";"+Time.ToString();
            return info;
        }
        static public FileOutInfo stringToFile(string info) {
            string[] infoArr = info.Split(';');
            DateTime time=DateTime.Parse(infoArr[1]);
            FileOutInfo fileInfo = new FileOutInfo();
            fileInfo.Time = time;
            fileInfo.Name = infoArr[0];
            return fileInfo;
        } 
    }
}
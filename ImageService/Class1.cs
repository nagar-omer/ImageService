using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageService.Modal;
using System.Xml.Linq;

namespace ImageService
{
    static class DictonaryXml
    {
        //converts Xml object to Dictonary of string,FileOutInfo
        static public Dictionary<string, FileOutInfo> XmlObjToDictonary(XElement el) {
            return DictonaryXml.DictonaryStringToFileOutInfoDict(DictonaryXml.XmlToDictionary(el));
        }
        //converts Dictonary of string,FileOutInfo to Xml object
        static public XElement DictonaryToXmlObj(Dictionary<string, FileOutInfo> myDictonary) {
            return DictonaryXml.DictonaryToXml(DictonaryXml.DictonaryFileOutInfoDictToString(myDictonary));
        }
        static private Dictionary<string, string> DictonaryFileOutInfoDictToString(Dictionary<string, FileOutInfo> myDictionary)
        {
            Dictionary < string, string> myDict=new Dictionary<string, string>();
            foreach (KeyValuePair<string, FileOutInfo> entry in myDictionary)
            {
                // do something with entry.Value or entry.Key
                myDict.Add(entry.Key, entry.Value.toStringMy());
            }
            return myDict;
        }
        static private Dictionary<string, FileOutInfo> DictonaryStringToFileOutInfoDict(Dictionary<string, string> myDictionary)
        {
            Dictionary<string, FileOutInfo> myDict = new Dictionary<string,FileOutInfo>();
            foreach (KeyValuePair<string,string> entry in myDictionary)
            {
                // do something with entry.Value or entry.Key
                myDict.Add(entry.Key, FileOutInfo.stringToFile(entry.Value));
            }
            return myDict;
        }
        static private Dictionary<string, string> XmlToDictionary
                                        (XElement rootElement)
        {
            /*Dictionary<string, string> dict = new Dictionary<string, string>();

            foreach (XElement elm in baseElm.Elements())
            {
                string dictKey = elm.Attribute(key).Value;
                string dictVal = elm.Attribute(value).Value;

                dict.Add(dictKey, dictVal);

            }

            return dict;*/
            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (var el in rootElement.Elements())
            {
                dict.Add(el.Name.LocalName, el.Value);
            }
            return dict;
        }
        static private XElement DictonaryToXml
                 (Dictionary<string, string> inputDict)
        {

            //XElement outElm = new XElement(elmName);

            //Dictionary<string, string>.KeyCollection keys = inputDict.Keys;

            //XElement inner = new XElement(valuesName);

            //foreach (string key in keys)
            //{
            //    inner.Add(new XAttribute("key", key));
            //    inner.Add(new XAttribute("value", inputDict[key]));
            //    outElm.Add(inner);
            //}
            //return outElm;
            XElement el = new XElement("root",
            inputDict.Select(kv => new XElement(kv.Key, kv.Value)));
            return el;

        }

    }

}

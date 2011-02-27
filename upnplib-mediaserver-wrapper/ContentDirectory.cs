using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using UPNPLib;
using System.Xml;
using System.IO;


namespace upnplib_mediaserver_wrapper
{
    public class ContentDirectory
    {
        private UPnPService cdService = null;
        private string objectID = null;
        private string title = null; 

        public ContentDirectory(UPnPService cd)
        {
            cdService = cd;
            objectID = "0"; // if the ID is not specified, make this object the root
            title = "Root";
        }

        public ContentDirectory(UPnPService cd, string objID, string objTitle)
        {
            cdService = cd;
            objectID = objID;
            title = objTitle;
        }

        public List<ContentDirectory> Children
        {
            get
            {
                string browseFlag = "BrowseDirectChildren"; // BrowseDirectChildren or BrowseMetadata as allowed values
                string filter = ""; 
                int startingIndex = 0;
                int requestedCount = 1000;
                string sortCriteria = "";

                object[] inArgs = new object[6];
                inArgs[0] = objectID;
                inArgs[1] = browseFlag;
                inArgs[2] = filter;
                inArgs[3] = startingIndex;
                inArgs[4] = requestedCount;
                inArgs[5] = sortCriteria;

                object outArgs = new object[4];
                cdService.InvokeAction("Browse", inArgs, ref outArgs);
                object[] resultobj = (object[])outArgs;

                string result;
                int numberReturned;
                int totalMatches;
                int updateID;

                result = (string) resultobj[0];
                numberReturned = (int)(UInt32) resultobj[1];
                totalMatches = (int)(UInt32) resultobj[2];
                updateID = (int)(UInt32) resultobj[3];

                List<ContentDirectory> children = subdirectoriesFromXml(result);
                children.AddRange(mediaItemsFromXml(result));

                return children;
            }
        }

        private List<ContentDirectory> subdirectoriesFromXml(string result)
        {
            string tagName = "container";
            return childrenFromXml(result, tagName);
        }

        private List<ContentDirectory> mediaItemsFromXml(string result)
        {
            string tagName = "item";
            return childrenFromXml(result, tagName);
        }

        private List<ContentDirectory> childrenFromXml(string result, string tagName)
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(new MemoryStream(new UTF8Encoding().GetBytes(result)));
            XmlNodeList containers = xml.GetElementsByTagName(tagName);
            List<ContentDirectory> children = new List<ContentDirectory>();
            foreach (XmlNode container in containers)
            {
                XmlDocument containerXml = new XmlDocument();
                containerXml.Load(new MemoryStream(new UTF8Encoding().GetBytes("<" + tagName + ">" + container.InnerXml + "</" + tagName + ">")));
                string objTitle = containerXml.GetElementsByTagName("dc:title")[0].InnerText;
                children.Add(new ContentDirectory(cdService, container.Attributes["id"].Value, objTitle));
            }
            return children;
        }
               

        public string SystemUpdateId
        {
            get
            {
                object systemUpdateId = new object();
                cdService.InvokeAction("GetSystemUpdateID", new object[0], ref systemUpdateId);
                return (string)SystemUpdateId;
            }
        }

        public string Title
        {
            get
            {
                return title;
            }
        }

    }
}

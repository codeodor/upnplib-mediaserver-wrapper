using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using UPNPLib;

namespace upnplib_mediaserver_wrapper
{
    public class MediaServer
    {
        public static List<MediaServer> All()
        {
            List<MediaServer> mediaServers = new List<MediaServer>();
            UPnPDeviceFinder deviceFinder = new UPnPDeviceFinder();
            UPnPDevices mediaServerDevices = deviceFinder.FindByType("urn:schemas-upnp-org:device:MediaServer:1", 0);
            foreach (UPnPDevice mediaServerDevice in mediaServerDevices)
            {
                mediaServers.Add(new MediaServer(mediaServerDevice, mediaServerDevice.FriendlyName));
            }

            return mediaServers;
        }

        private UPnPDevice device = null;
        private ContentDirectory contentDirectory = null;
        private ConnectionManager connectionManager = null;
        private string title = null;

        public MediaServer(UPnPDevice mediaServerDevice, string deviceTitle) 
        {
            device = mediaServerDevice;    
            title = deviceTitle;
        }

        public string Title
        {
            get
            {
                return title;
            }
        }

        public ContentDirectory ContentDirectory
        {
            get 
            {
                if(contentDirectory == null)
                    contentDirectory = new ContentDirectory(device.Services["urn:upnp-org:serviceId:ContentDirectory"]);
            
                return contentDirectory;
            }
        }

        public ConnectionManager ConnectionManager
        {
            get
            {
                if (connectionManager == null)
                    connectionManager = new ConnectionManager(device.Services["urn:upnp-org:serviceId:ConnectionManager"]);

                return connectionManager;
            }
        }
        
    }
}

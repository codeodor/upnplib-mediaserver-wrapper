using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UPNPLib;

namespace upnplib_mediaserver_wrapper
{
    public class ConnectionManager
    {
        private UPnPService cmService = null;
        public ConnectionManager(UPnPService cm)
        {
            cmService = cm;
        }
    }
}

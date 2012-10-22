using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Maps.MapControl.WPF;

namespace Project422_2.api
{
    public enum CanvasItems
    {
        ADDRESS_SEARCH,DEVICE_LOCATION
    };

    public enum NetworkStatus
    {
        ONLINE, OFFLINE, CHECKING
    };

    public enum AddNewDeviceType
    {
        PEER, SLAVE, EDIT
    };

    public class Constants
    {
        // Ping status
        public static int pingDelayDefault = 10000;
        public static int updateDelayDefault = 10000;

        // XMLfile ElementName
        public const string xmlDeviceList = "deviceList";
        public const string xmlDevice = "device";
        public const string xmlDeviceId = "deviceId";
        public const string xmlParentId = "parentId";
        public const string xmlDomain = "domain";
        public const string xmlIpaddress = "ipaddress";
        public const string xmldeviceName = "deviceName";
        public const string xmlLatitude = "latitude";
        public const string xmlLongitude = "longitude";
        public const string xmlDisabled = "diabled";
        public const string xmlCurrentStatus = "currentStatus";
        public const string xmlDefaultViewZoom = "defaultViewZoom";
        public const string xmlDefaultViewLatitude = "defaultViewLatitude";
        public const string xmlDefaultViewLongitude = "defaultViewLongitude";
        public const string xmlRoot = "root";

        public static string DEVICE_CONFIG_FILE = "C:\\deviceList.xml";

        // Default view
        public static double defaultZoom = 2;
        public static Location defaultLocation = new Location(0.0,0.0);
    }
}

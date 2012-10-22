using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Maps.MapControl.WPF;
using System.Net;

namespace Project422_2.api
{
    class Device
    {

        // To be retrieved and stored in XML
        public int DeviceId { get; set; }

        public int ParentID { get; set; }
       
        public IPAddress Ipaddress { get; set; }

        public string DomainName { get; set; }
       
        public string DeviceName { get; set; }

        public Microsoft.Maps.MapControl.WPF.Location MapCoOrdinate { get; set; }
        
        public bool Disabled { get; set; }

        public NetworkStatus CurrentStatus { get; set; }

        public DateTime lastUpdateTime { get; set; }

        // Static objects
        public static int deviceIDSeq=1;

        // Increments deviceID seq number
        private void validateIDSeq(int deviceID)
        {

            if (deviceIDSeq <= deviceID)
                deviceIDSeq = deviceID + 1;
        }

        // This constructor will be called while loading the data from a XML file
        public Device(int deviceID, int parentID, string domain, string ipaddress, String deviceName, double latitude, double longitude, bool disabled, NetworkStatus currentStatus)
        {
            this.DeviceId = deviceID == -1 ? deviceIDSeq : deviceID;
            this.ParentID = parentID;
            this.DeviceName = deviceName;
            this.MapCoOrdinate = new Location(latitude, longitude);
            this.Disabled = disabled;
            this.CurrentStatus = currentStatus;
            
            this.DomainName = domain ;
            IPAddress ip;
            if (IPAddress.TryParse(ipaddress, out ip))
            {
                this.Ipaddress = ip;
            }
            else
            {
                this.Ipaddress = null; 
            }

            validateIDSeq(deviceIDSeq);
        }

        // comparator function for this class
        public static int CompareDeviceId(Device  x, Device  y)
        {
            return x.DeviceId .CompareTo(y.DeviceId );
        }
    }

}

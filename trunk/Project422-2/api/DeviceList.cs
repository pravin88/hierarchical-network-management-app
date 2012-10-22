using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace Project422_2.api
{
    class DeviceList
    {
        private static List<Device> deviceList = new List<Device>();

        public static void initList()
        {
            deviceList = new List<Device>();
        }

        // deletes a device from the device list
        public static void deleteDevice(int deviceID)
        {
            deviceList.Remove(getDevice(deviceID));
        }

        // called from xml parser while loading the data from XML file
        public static void loadDeviceFromXML(int deviceID, int parentID, string domain, string ipaddress, String deviceName, double latitude, double longitude, bool disabled, NetworkStatus currentStatus)
        {
            Device devObj = new Device(deviceID, parentID, domain, ipaddress, deviceName, latitude, longitude, disabled, currentStatus );
            deviceList.Add(devObj);
        }

        // called from GUI for creating a new device object.
        public static int addDeviceFromGUI(int parentID, string domainOrIpaddress, String deviceName)
        {
            Device devObj = new Device(-1, parentID, domainOrIpaddress , domainOrIpaddress  , deviceName, -1, -1, false, NetworkStatus.OFFLINE );
            deviceList.Add(devObj);
            return devObj.DeviceId;
        }

        // get the device object from a device list
        public static Device getDevice(int deviceID)
        {
            foreach (Device devObj in deviceList)
            {
                if (devObj.DeviceId == deviceID)
                    return devObj;
            }
            return null;

        }

        // get the device object from a device list
        public static Device getDevice(IPAddress ipaddress)
        {
            foreach (Device devObj in deviceList)
            {
                if ( null != devObj.Ipaddress && devObj.Ipaddress.Address   == ipaddress.Address )
                    return devObj;
            }
            return null;

        }

        // this method will be called for editing the device Object in the list.
        public static void editDevice(Device editedDevice)
        {
            // deletes the existing device with obsolete data
            deviceList.Remove(getDevice(editedDevice.DeviceId));

            // add the edited device with the new data
            deviceList.Add(editedDevice);
        }

        // this method will be called for deleting the device Object in the list and subtrees.
        public static void deleteDeviceSubtree(int deviceId)
        {
            // delete all the subtree device
            foreach (Device devObj in deviceList)
            {
                if (devObj.ParentID == deviceId)
                    deviceList.Remove(getDevice(devObj.DeviceId));
            }

            // deletes the requested device
            deviceList.Remove(getDevice(deviceId));



        }

        // returns the device list 
        public static List<Device> getDeviceList()
        {
            // sort the list before returning
            deviceList.Sort(Device .CompareDeviceId );

            return deviceList;
        }
    }
}

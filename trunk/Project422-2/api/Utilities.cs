using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows.Controls;
using Google.Api.Maps.Service.Geocoding;
using Microsoft.Maps.MapControl.WPF;
using System.Windows;
using Google.Api.Maps.Service;

namespace Project422_2.api
{
    class Utilities
    {

        /// <summary>
        /// Reads the device configuration from an XML file and loads it to the memory
        /// </summary>
        /// <param name="filename"></param>
        public static void readConfigXMlFile(String filename)
        {
            try
            {

                DeviceList.initList();

                XmlDocument xml = new XmlDocument();

                xml.Load(filename);

                XmlNodeList defaultViewZoom = xml.GetElementsByTagName(Constants.xmlDefaultViewZoom);
                Constants.defaultZoom = Double.Parse(defaultViewZoom[0].InnerText);

                XmlNodeList defaultViewLatitude = xml.GetElementsByTagName(Constants.xmlDefaultViewLatitude);
                double defaultLatitude = Double.Parse(defaultViewLatitude[0].InnerText);

                XmlNodeList defaultViewLongitude = xml.GetElementsByTagName(Constants.xmlDefaultViewLongitude);
                double defaultLongitude = Double.Parse(defaultViewLongitude[0].InnerText);

                Constants.defaultLocation = new Location(defaultLatitude, defaultLongitude);

                // Read Antenna Config and load to the memory
                foreach (XmlNode device in xml.GetElementsByTagName(Constants.xmlDevice))
                {
                    int deviceId = 0, parentID = 0;
                    string domain = "", ipaddress = "", deviceName = "";
                    double latitude = 0, longitude = 0;
                    bool disabled = false;
                    NetworkStatus currentStatus = NetworkStatus.OFFLINE;

                    foreach (XmlNode deviceChildren in device.ChildNodes)
                    {
                        // add the node to the object
                        switch (deviceChildren.Name)
                        {
                            case Constants.xmlDeviceId:
                                deviceId = Int32.Parse(deviceChildren.InnerText);
                                break;
                            case Constants.xmlParentId:
                                parentID = Int32.Parse(deviceChildren.InnerText);
                                break;
                            case Constants.xmlDomain:
                                domain = deviceChildren.InnerText;
                                break;
                            case Constants.xmlIpaddress:
                                ipaddress = deviceChildren.InnerText;
                                break;
                            case Constants.xmldeviceName:
                                deviceName = deviceChildren.InnerText;
                                break;
                            case Constants.xmlLatitude:
                                latitude = Double.Parse(deviceChildren.InnerText);
                                break;
                            case Constants.xmlLongitude:
                                longitude = Double.Parse(deviceChildren.InnerText);
                                break;
                            case Constants.xmlDisabled:
                                disabled = Boolean.Parse(deviceChildren.InnerText);
                                break;
                            case Constants.xmlCurrentStatus:
                                currentStatus = (NetworkStatus)Enum.Parse(typeof(NetworkStatus), deviceChildren.InnerText);
                                break;
                        }// end switch
                    }// end one antenna

                    DeviceList.loadDeviceFromXML(deviceId, parentID, domain, ipaddress, deviceName, latitude, longitude, disabled, currentStatus);
                }// end all devices
            }
            catch (Exception e)
            {
                MessageBox.Show("Error loading file.");
                Logger.Error(e.Message);
            }
        }

        /// <summary>
        /// Saves the device in the memory to an XML file
        /// </summary>
        /// <param name="filename"></param>
        public static void saveConfigXMLFile(string filename)
        {
            try
            {
                XmlDocument xml = new XmlDocument();

                XmlElement xmlDeviceList = xml.CreateElement(Constants.xmlDeviceList);

                XmlElement root = xml.CreateElement(Constants.xmlRoot);


                // provide default view

                XmlElement defaultViewZoom = xml.CreateElement(Constants.xmlDefaultViewZoom);
                defaultViewZoom.InnerText = Constants.defaultZoom.ToString();
                root.AppendChild(defaultViewZoom);

                XmlElement defaultViewLatitude = xml.CreateElement(Constants.xmlDefaultViewLatitude);
                defaultViewLatitude.InnerText = Constants.defaultLocation.Latitude.ToString();
                root.AppendChild(defaultViewLatitude);

                XmlElement defaultViewLongitude = xml.CreateElement(Constants.xmlDefaultViewLongitude);
                defaultViewLongitude.InnerText = Constants.defaultLocation.Longitude.ToString();
                root.AppendChild(defaultViewLongitude);

                foreach (Device device in DeviceList.getDeviceList())
                {
                    XmlElement xmlDevice = xml.CreateElement(Constants.xmlDevice);

                    XmlElement deviceID = xml.CreateElement(Constants.xmlDeviceId);
                    deviceID.InnerText = device.DeviceId.ToString();
                    xmlDevice.AppendChild(deviceID);

                    XmlElement parentID = xml.CreateElement(Constants.xmlParentId);
                    parentID.InnerText = device.ParentID.ToString();
                    xmlDevice.AppendChild(parentID);

                    XmlElement deviceName = xml.CreateElement(Constants.xmldeviceName);
                    deviceName.InnerText = device.DeviceName;
                    xmlDevice.AppendChild(deviceName);

                    XmlElement ipAddress = xml.CreateElement(Constants.xmlIpaddress);
                    ipAddress.InnerText = device.Ipaddress.ToString();
                    xmlDevice.AppendChild(ipAddress);

                    XmlElement domainName = xml.CreateElement(Constants.xmlDomain);
                    domainName.InnerText = device.DomainName;
                    xmlDevice.AppendChild(domainName);

                    XmlElement disabled = xml.CreateElement(Constants.xmlDisabled);
                    disabled.InnerText = device.Disabled.ToString();
                    xmlDevice.AppendChild(disabled);

                    XmlElement latitude = xml.CreateElement(Constants.xmlLatitude);
                    latitude.InnerText = device.MapCoOrdinate.Latitude.ToString();
                    xmlDevice.AppendChild(latitude);

                    XmlElement longitude = xml.CreateElement(Constants.xmlLongitude);
                    longitude.InnerText = device.MapCoOrdinate.Longitude.ToString();
                    xmlDevice.AppendChild(longitude);

                    XmlElement currentStatus = xml.CreateElement(Constants.xmlCurrentStatus);
                    currentStatus.InnerText = device.CurrentStatus.ToString();
                    xmlDevice.AppendChild(currentStatus);

                    xmlDeviceList.AppendChild(xmlDevice);
                }

                root.AppendChild(xmlDeviceList);

                xml.AppendChild(root);
                // Save the document to a file and auto-indent the output.
                XmlTextWriter writer = new XmlTextWriter(filename, null);
                writer.Formatting = Formatting.Indented;
                xml.Save(writer);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving config file ");
                Logger.Error(ex.Message);
            }
        }

        /// <summary>
        /// Converts a Geocode response to a tree
        /// </summary>
        /// <param name="results"></param>
        /// <returns></returns>
        public static TreeViewItem[] ToTree(GeocodingResult[] results)
        {
            var nodes = new List<TreeViewItem>();

            foreach (var result in results)
            {
                var node = new TreeViewItem();
                node.Header = "(" + result.Types.FirstOrDefault() + ") " + result.FormattedAddress;
                node.Tag = new SelectedObject(result.Types.FirstOrDefault(), result.Geometry.Location);

                foreach (var component in result.Components)
                {
                    node.Items.Add(new TreeViewItem
                    {
                        Header = component.Types.FirstOrDefault() + ": " + component.LongName,
                        Tag = result.Geometry.Location
                    });
                }

                node.Items.Add(new TreeViewItem { Header = "Lat: " + result.Geometry.Location.Latitude });
                node.Items.Add(new TreeViewItem { Header = "Lng: " + result.Geometry.Location.Longitude });

                nodes.Add(node);
            }

            return nodes.ToArray();
        }

    }

    public class SelectedObject
    {
        public AddressType FirstOrDefault { get; set; }
        public GeographicPosition GeographicPosition { get; set;}
        public SelectedObject(AddressType firstOrDefault, GeographicPosition geographicPosition)
        {
            this.FirstOrDefault = firstOrDefault;
            this.GeographicPosition = geographicPosition;
        }
    }
}

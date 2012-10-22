using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Google.Api.Maps.Service.Geocoding;
using Google.Api.Maps.Service;
using Project422_2.api;
using Microsoft.Maps.MapControl.WPF;
using System.Xml;
using System.Net;
using System.IO;
using System.Timers;
using System.Threading;
using System.Net.NetworkInformation;
using System.Windows.Threading;
using System.Net.Sockets;

/*********************************************************************************************************/
//  Hierarchical Network Management Application
//  
//  Author: Pravin
//  Created On: 22/10/2012
//  Contact: pravin88.kumar@gmail.com
/*********************************************************************************************************/

namespace Project422_2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string BingMapsKey = "ApecU4Us8osvdBtm528_22fCL1S7Awq4AbX70QXzyBXdQymZR00k30J6to0BoCcP";
        System.Windows.Point pinPosition;
        private MapLayer imageLayer = new MapLayer();
        public System.Timers.Timer pingTimer = new System.Timers.Timer(Constants .pingDelayDefault);
        System.Timers.Timer updateTimer = new System.Timers.Timer(Constants.updateDelayDefault);

        Thread updateStatusBarThread;
        private TreeViewWithIcons selectedLeftPane;

        /// <summary>
        /// Constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            // hide the canvas over the map
            hidePopupCanvas();            

            // initialize map with a layer which will contain pushpins
            locationMap.Children.Add(imageLayer);

            updateStatusBarThread = new Thread(updateStatusBar);
            updateStatusBarThread.Start();

            // Initialize timer
            pingTimer.Elapsed += new System.Timers.ElapsedEventHandler(pingTimerEvent);
            updateTimer.Elapsed += new System.Timers.ElapsedEventHandler(updateTimerEvent);
            
            pingTimer.Start();
        }

        private void updateStatusBar()
        {
            while (true)
            {
                this.Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
                {

                    if (pingTimer.Enabled)
                    {
                        statusBarText.Text = "Auto refreshing every "+ pingTimer.Interval + " milli secs";
                    }
                    else
                    {
                        statusBarText.Text = "Auto refresh disabled";
                    }

                }));

                Thread.Sleep(1000);
            }
        }

        #region pingtimer
        /// <summary>
        /// This method will be called peridiocally by the timer.
        /// This will send an async ping request for each ip address and
        /// updates the leftPaneDeviceTree and pushpin map layer
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void pingTimerEvent(object source, ElapsedEventArgs e)
        {
            pingRequestAllDevices();

        }


        public void pingRequestAllDevices()
        {
            // send asynchronous ping to each device
            List<Device> deviseList = new List<Device>(DeviceList.getDeviceList());
            foreach (Device device in deviseList)
            {
                if (device.DomainName != null)
                {
                    try
                    {
                        IPAddress[] ipadd = Dns.GetHostAddresses(device.DomainName);
                        if (ipadd != null)
                        {
                            device.Ipaddress = ipadd[0];
                        }
                    }
                    catch (ArgumentException ae)
                    {
                        // do nothing
                    }
                    catch (SocketException se)
                    {
                        // do nothing
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex.ToString());
                    }
                }
                if (device.Ipaddress != null)
                {
                    asyncPingRequest(device.Ipaddress);
                }
            }

            // Initialize timer for updating 

            updateTimer.Start();
        }

        private void updateTimerEvent(object source, ElapsedEventArgs e)
        {
            this.Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
            {
                updateTree(leftPaneDeviceTree);
                updatePushpinMap();
            }));

            updateTimer.Stop();
        }

        /// <summary>
        /// This method will send a async ping request to the given ipaddress
        /// </summary>
        /// <param name="ipaddress"></param>
        private void asyncPingRequest(IPAddress  ipaddress)
        {
            AutoResetEvent waiter = new AutoResetEvent(false);

            Ping pingSender = new Ping();

            // When the PingCompleted event is raised,
            // the PingCompletedCallback method is called.
            pingSender.PingCompleted += new PingCompletedEventHandler(PingCompletedCallback);

            // Create a buffer of 32 bytes of data to be transmitted.
            string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
            byte[] buffer = Encoding.ASCII.GetBytes(data);

            // Wait 12 seconds for a reply.
            int timeout = 12000;

            // Set options for transmission:
            // The data can go through 64 gateways or routers
            // before it is destroyed, and the data packet
            // cannot be fragmented.
            PingOptions options = new PingOptions(64, true);

            // Send the ping asynchronously.
            // Use the waiter as the user token.
            // When the callback completes, it can wake up this thread.
            pingSender.SendAsync(ipaddress , timeout, buffer, options, waiter);

            // Prevent this example application from ending.
            // A real application should do something useful
            // when possible.
            //waiter.WaitOne();
        }

        /// <summary>
        /// This method receives the response for each async ping request sent
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void PingCompletedCallback(object sender, PingCompletedEventArgs e)
        {
            // If the operation was canceled, display a message to the user.
            if (e.Cancelled)
            {
                // Let the main thread resume. 
                // UserToken is the AutoResetEvent object that the main thread 
                // is waiting for.
                ((AutoResetEvent)e.UserState).Set();
            }

            // If an error occurred, display the exception to the user.
            if (e.Error != null)
            {
                Console.WriteLine(e.Error.ToString());

                // Let the main thread resume. 
                ((AutoResetEvent)e.UserState).Set();
            }

            PingReply reply = e.Reply;

            UpdateDeviceCurrentStatus(reply);
            //i .CheckHostName ()
            // Let the main thread resume.
            ((AutoResetEvent)e.UserState).Set();
        }

        /// <summary>
        /// Updates the current status for the corresponding device from which response is received
        /// </summary>
        /// <param name="reply"></param>
        public static void UpdateDeviceCurrentStatus(PingReply reply)
        {
            if (reply != null && reply.Address.ToString() != "")
            {
                Device device = DeviceList.getDevice(reply.Address);

                if (device != null)
                {
                    device.CurrentStatus = (reply.Status == IPStatus.Success) ? NetworkStatus.ONLINE : NetworkStatus.OFFLINE;
                    device.lastUpdateTime = DateTime.Now;
                    DeviceList.editDevice(device);
                }
            }
        }
        #endregion

        #region GUIEvents

        private void RefreshSettings_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            RefreshSettings refreshSettings = new RefreshSettings(this);
            refreshSettings.Show();
        }

        private void Exit_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Load_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog  dlg = new Microsoft.Win32.OpenFileDialog ();
            dlg.FileName = "config"; // Default file name
            dlg.DefaultExt = ".xml"; // Default file extension
            dlg.Filter = "xml file (.xml)|*.xml"; // Filter files by extension

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open xml document and read to internal memory
                Utilities.readConfigXMlFile(dlg.FileName);

                // update GUI
                updateTree(leftPaneDeviceTree);
                updatePushpinMap();

                locationMap.SetView(Constants.defaultLocation,Constants.defaultZoom);
            }
        }
        
        private void Save_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Configure save file dialog box
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "config"; // Default file name
            dlg.DefaultExt = ".xml"; // Default file extension
            dlg.Filter = "xml file (.xml)|*.xml"; // Filter files by extension

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                // Save docinternal memory to xml file
                Utilities.saveConfigXMLFile(dlg.FileName);
            }
        }

        private void AddPeerDevice_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            int deviceId = (selectedLeftPane == null) ?-1: int.Parse(selectedLeftPane.Tag.ToString());

            AddNewDevice addNewDevice = new AddNewDevice(this, AddNewDeviceType.PEER, deviceId);
            addNewDevice.Show();
        }

        private void AddSlaveDevice_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            AddNewDevice addNewDevice = new AddNewDevice(this, AddNewDeviceType.SLAVE, int.Parse(selectedLeftPane.Tag.ToString()));
            addNewDevice.Show();
        }

        /// <summary>
        /// Sends a geocode request for the address specified in the text bar
        /// On successfull response, displays address suggestions over the canvas
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addressSearchBtn_Click(object sender, RoutedEventArgs e)
        {



        }

        /// <summary>
        /// zooms the map to the address selected in the popup canvas
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addressResultTree_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                hidePopupCanvas();
                SelectedObject selectedObject = ((SelectedObject)((TreeViewItem)addressResultTree.SelectedItem).Tag);
                GeographicPosition location = selectedObject.GeographicPosition;
                AddressType firstOrDefault = selectedObject.FirstOrDefault;
                Location loc = new Microsoft.Maps.MapControl.WPF.Location(Double.Parse(location.Latitude.ToString()), Double.Parse(location.Longitude.ToString()));
                locationMap.SetView(loc, getZoom(firstOrDefault));

            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
        }

        private double getZoom(AddressType firstOrDefault)
        {
            switch(firstOrDefault)
            {
                case AddressType.Country:
                    return 4;
                default:
                    return 7;
            }

        }

        /// <summary>
        /// gets the location where the user clicked in the map
        /// displays the device tree from which user could assign the selected point
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void locationMap_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            pinPosition = e.GetPosition(this.locationMap);
            viewDeviceLocationTree();
            updateTree(chooseDeviceLocationTree);
        }

        /// <summary>
        /// gets the device selected in the canvas and updates the location to the pin location selected with right click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chooseDeviceLocationTree_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            try
            {
                hidePopupCanvas();

                // Disables the default mouse double-click action.
                e.Handled = true;

                //Convert the mouse coordinates to a locatoin on the map
                Location pinLocation = locationMap.ViewportPointToLocation(pinPosition);

                TreeViewItem selectedDevice = (TreeViewItem)chooseDeviceLocationTree.SelectedItem;
                Device editedDevice = DeviceList.getDevice(Int32.Parse(selectedDevice.Tag.ToString()));
                editedDevice.MapCoOrdinate = new Location(pinLocation.Latitude, pinLocation.Longitude);
                DeviceList.editDevice(editedDevice);

                updatePushpinMap();

            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }

        }

        /// <summary>
        /// zooms the map to the selected device in the tree
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ViewLocation_MenuItem_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                int deviceID = Int32.Parse(((TreeViewItem)leftPaneDeviceTree.SelectedItem).Tag.ToString());
                Device device = DeviceList.getDevice(deviceID);

                if(device .MapCoOrdinate .Latitude != -1 && device .MapCoOrdinate .Longitude != -1)
                    locationMap.SetView(new Location(device.MapCoOrdinate.Latitude, device.MapCoOrdinate.Longitude), 5);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }

        }

        private void locationMap_ViewChangeOnFrame(object sender, MapEventArgs e)
        {
            mapZoomControl.Value = locationMap.ZoomLevel;
        }

        private void mapZoomControl_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            locationMap.ZoomLevel = mapZoomControl.Value;
        }

        private void leftPaneDeviceTree_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (null == (selectedLeftPane = (TreeViewWithIcons)leftPaneDeviceTree.SelectedItem))
            {
                addSlaveMenuItem.IsEnabled = false;
                deleteDeviceMenuItem.IsEnabled = false;
            }
            else
            {
                addSlaveMenuItem.IsEnabled = true;
                deleteDeviceMenuItem.IsEnabled = true;
            }
        }

        private void DeleteDevice_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (selectedLeftPane != null)
            {
                DeviceList.deleteDeviceSubtree(int.Parse(selectedLeftPane.Tag.ToString()));

                updateTree(leftPaneDeviceTree);
                updatePushpinMap();
            }
        }

        private void SetDefaultView_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Constants.defaultZoom = locationMap.ZoomLevel;
            Constants.defaultLocation = locationMap.Center;
        }

        private void EditDevice_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (null != selectedLeftPane)
            {
                AddNewDevice editDevice = new AddNewDevice(this, AddNewDeviceType.EDIT, int.Parse(selectedLeftPane.Tag.ToString()));
                editDevice.Show();
            }

        }

        private void addressResultCanvasClose_MouseDown(object sender, MouseButtonEventArgs e)
        {
            hidePopupCanvas();
        }

        #endregion
                
        #region GUIUpdate
        private void updateTree(TreeView treeView)
        {
                // clear the tree
            treeView.Items.Clear();

            List<Device> deviceList = new List<Device >( DeviceList.getDeviceList());

            foreach (Device device in deviceList )
            {
                // if the status hasn't been updated for twice as long as the ping timer, then set the status to offline
                if (device.lastUpdateTime.AddMilliseconds(Constants.pingDelayDefault *2) < DateTime.Now && device.CurrentStatus == NetworkStatus.ONLINE)
                {
                    device.CurrentStatus = NetworkStatus.OFFLINE;
                    DeviceList.editDevice(device);
                }

                if (device.ParentID > 0)
                {
                    if (SelectTreeViewItem(treeView.Items, device.ParentID.ToString()) != null)
                    {
                        ((TreeViewItem)treeView.SelectedItem).Items.Add(createTreeViewItem(device.DeviceId.ToString(), device.DeviceName, device.CurrentStatus));
                    }
                }
                else
                {
                    treeView.Items.Add(createTreeViewItem(device.DeviceId.ToString(), device.DeviceName, device.CurrentStatus));
                }
            }

        }

        private void updatePushpinMap()
        {
            imageLayer.Children.Clear();
            List<Device> deviceList = new List<Device> ( DeviceList.getDeviceList());
            foreach (Device device in deviceList )
            {
                if((device .MapCoOrdinate .Latitude != -1) && (device .MapCoOrdinate .Longitude != -1))
                    addPushpinToMap(new Location(device .MapCoOrdinate.Latitude  ,device .MapCoOrdinate.Longitude  ),device.CurrentStatus, device.DeviceName  );
            }
        }

        public void addPeerDevice(String ipAddress, String deviceName)
        {
            try
            {
                if ((selectedLeftPane == null) || (SelectTreeViewItem(leftPaneDeviceTree.Items, selectedLeftPane.Tag.ToString())).Parent.DependencyObjectType.Name == "TreeView")
                {
                    int deviceID = DeviceList.addDeviceFromGUI(0, ipAddress, deviceName);
                    leftPaneDeviceTree.Items.Add(createTreeViewItem(deviceID.ToString(), deviceName, NetworkStatus.OFFLINE));
                }
                else
                {
                    TreeViewItem selectedItem = (TreeViewItem)leftPaneDeviceTree.SelectedItem;
                    TreeViewItem parent = (TreeViewItem)selectedItem.Parent;
                    int parentID = Int32.Parse(parent.Tag.ToString());
                    int deviceID = DeviceList.addDeviceFromGUI(parentID, ipAddress, deviceName);
                    ((TreeViewItem)((TreeViewItem)leftPaneDeviceTree.SelectedItem).Parent).Items.Add(createTreeViewItem(deviceID.ToString(), deviceName, NetworkStatus.OFFLINE));
                }
            }
            catch (Exception e)
            {
                Logger.Error(e.ToString());
            }
        }

        public void addSlaveDevice(String ipAddress, String deviceName)
        {
            try
            {
                if (selectedLeftPane != null)
                {
                    SelectTreeViewItem(leftPaneDeviceTree.Items, selectedLeftPane.Tag.ToString());
                    int parentID = Int32.Parse(((TreeViewItem)leftPaneDeviceTree.SelectedItem).Tag.ToString());
                    int deviceID = DeviceList.addDeviceFromGUI(parentID, ipAddress, deviceName);
                    ((TreeViewItem)leftPaneDeviceTree.SelectedItem).Items.Add(createTreeViewItem(deviceID.ToString(), deviceName, NetworkStatus.OFFLINE));
                }

            }
            catch (Exception e)
            {
                Logger.Error(e.ToString());
            }

        }

        public void editDevice(String ipAddress, String deviceName, int deviceId)
        {
            try
            {
                Device editedDevice = DeviceList.getDevice(deviceId);

                editedDevice.DeviceName = deviceName;

                editedDevice.DomainName = ipAddress;
                IPAddress ip;
                if (IPAddress.TryParse(ipAddress, out ip))
                {
                    editedDevice.Ipaddress = ip;
                }
                else
                {
                    editedDevice.Ipaddress = null;
                }
                DeviceList.editDevice(editedDevice);

            }
            catch (Exception e)
            {
                Logger.Error(e.ToString());
            }

        }

        /// <summary>
        /// Hides the canvas shown over the map
        /// </summary>
        private void hidePopupCanvas()
        {
            addressResultCanvas.Visibility = Visibility.Hidden;
            addressResultTree.Visibility = Visibility.Hidden;
            chooseDeviceLocationTree.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Shows a canvas over the map with address result returned from the geocode response
        /// </summary>
        private void viewAddressResult()
        {
            addressResultCanvas.Visibility = Visibility.Visible;
            addressResultTree.Visibility = Visibility.Visible;
            chooseDeviceLocationTree.Visibility = Visibility.Hidden;

        }

        /// <summary>
        /// Shows a canvas with device tree for assigning the selected location in the map
        /// </summary>
        private void viewDeviceLocationTree()
        {
            addressResultCanvas.Visibility = Visibility.Visible;
            chooseDeviceLocationTree.Visibility = Visibility.Visible;
            addressResultTree.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Adds a pin to the specified location in the map with specified status
        /// </summary>
        /// <param name="location"></param>
        /// <param name="status"></param>
        private void addPushpinToMap(Location location, NetworkStatus status, string title)
        {

            Image image = new Image();
            //image.Height = 150;
            //Define the URI location of the image
            BitmapImage myBitmapImage = new BitmapImage();
            myBitmapImage.BeginInit();

            if (status == NetworkStatus.ONLINE)
                myBitmapImage.UriSource = new Uri(Directory.GetCurrentDirectory() + "\\image\\pin_green.png");
            else
                myBitmapImage.UriSource = new Uri(Directory.GetCurrentDirectory() + "\\image\\pin_red.png");

            // To save significant application memory, set the DecodePixelWidth or  
            // DecodePixelHeight of the BitmapImage value of the image source to the desired 
            // height or width of the rendered image. If you don't do this, the application will 
            // cache the image as though it were rendered as its normal size rather then just 
            // the size that is displayed.
            // Note: In order to preserve aspect ratio, set DecodePixelWidth
            // or DecodePixelHeight but not both.
            //Define the image display properties
            //myBitmapImage.DecodePixelHeight = 150;
            myBitmapImage.EndInit();
            image.Source = myBitmapImage;
            image.Opacity = 1.0;
            image.Stretch = System.Windows.Media.Stretch.None;
            ToolTip tooltip = new System.Windows.Controls.ToolTip();
            tooltip.FontSize = 14;
            tooltip.Content = title;
            image.ToolTip = tooltip;


            //Add the image to the defined map layer
            imageLayer.AddChild(image, location, PositionOrigin.BottomLeft);

        }



        #endregion

        #region helper
        /// <summary>
        /// sends a geocode request and returns the response
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        private GeocodingResponse geocodeRequest(string address)
        {
            var request = new GeocodingRequest();
            request.Address = address;
            request.Sensor = "false";
            return GeocodingService.GetResponse(request);

            //XmlDocument a = Geocode(addressSearchText.Text);
            //MessageBox.Show(a.ToString());

        }

        /// <summary>
        /// Selects the tree view item.
        /// </summary>
        /// <param name="Collection">The collection.</param>
        /// <param name="Value">The value.</param>
        /// <returns></returns>
        private TreeViewWithIcons SelectTreeViewItem(ItemCollection Collection, String Value)
        {
            if (Collection == null) return null;
            foreach (TreeViewWithIcons Item in Collection)
            {
                /// Find in current
                if (Item.Tag.Equals(Value))
                {
                    Item.IsSelected = true;
                    return Item;
                }
                /// Find in Childs
                if (Item.Items != null)
                {
                    TreeViewWithIcons childItem = this.SelectTreeViewItem(Item.Items, Value);
                    if (childItem != null)
                    {
                        Item.IsExpanded = true;
                        return childItem;
                    }
                }
            }
            return null;
        }

        private TreeViewWithIcons createTreeViewItem(String id, String name, NetworkStatus status)
        {
            TreeViewWithIcons item = new TreeViewWithIcons();
            item.Tag = id;
            item.HeaderText = name;
            item.Icon = CreateImage(getImageForStatus(status));
            return item;
        }

        private string getImageForStatus(NetworkStatus status)
        {
            switch (status)
            {
                case NetworkStatus.ONLINE:
                    return Directory.GetCurrentDirectory() + "\\image\\greendot.jpg";
                case NetworkStatus.OFFLINE:
                    return Directory.GetCurrentDirectory() + "\\image\\reddot.png";
                default:
                    return Directory.GetCurrentDirectory() + "\\image\\greydot.jpg";
            }

        }

        public BitmapImage CreateImage(string path)
        {
            BitmapImage myBitmapImage = new BitmapImage();
            myBitmapImage.BeginInit();
            myBitmapImage.UriSource = new Uri(path);
            myBitmapImage.EndInit();
            return myBitmapImage;
        }
        
        #endregion

        private void addressSearchText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var response = geocodeRequest(addressSearchText.Text);

                if (response.Status == ServiceResponseStatus.Ok || response.Status == ServiceResponseStatus.ZeroResults)
                {
                    // update the GUI tree
                    viewAddressResult();
                    addressResultTree.ItemsSource = Utilities.ToTree(response.Results);
                }

            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var response = geocodeRequest(addressSearchText.Text);

            if (response.Status == ServiceResponseStatus.Ok || response.Status == ServiceResponseStatus.ZeroResults)
            {
                // update the GUI tree
                viewAddressResult();
                addressResultTree.ItemsSource = Utilities.ToTree(response.Results);
            }


        }






/*
        public XmlDocument Geocode(string addressQuery)
        {
            //Create REST Services geocode request using Locations API
            string geocodeRequest = "http://dev.virtualearth.net/REST/v1/Locations/" + addressQuery + "?o=xml&key=" + BingMapsKey;


            //Make the request and get the response
            XmlDocument geocodeResponse = GetXmlResponse(geocodeRequest);

            return (geocodeResponse);
        }
        

        // Submit a REST Services or Spatial Data Services request and return the response
        private XmlDocument GetXmlResponse(string requestUrl)
        {
            System.Diagnostics.Trace.WriteLine("Request URL (XML): " + requestUrl);
            HttpWebRequest request = WebRequest.Create(requestUrl) as HttpWebRequest;
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                if (response.StatusCode != HttpStatusCode.OK)
                    throw new Exception(String.Format("Server error (HTTP {0}: {1}).",
                    response.StatusCode,
                    response.StatusDescription));
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(response.GetResponseStream());
                return xmlDoc;
            }
        }
        */
  

    }

    /// <summary>
    /// This Class Provides the TreeView with extended functionalities like,
    /// Adding the HeaderText feature to Node, Setting the icon for TreeViewNode.
    /// </summary>

    public class TreeViewWithIcons : TreeViewItem
    {
        #region Global variables
        ImageSource iconSource;
        TextBlock textBlock;
        Image icon;
        #endregion Global variables

        #region Constructors and Destructors
        public TreeViewWithIcons()
        {
            StackPanel stack = new StackPanel();
            stack.Orientation = Orientation.Horizontal;
            Header = stack;
            //Uncomment this code If you want to add an Image after the Node-HeaderText
            //textBlock = new TextBlock();
            //textBlock.VerticalAlignment = VerticalAlignment.Center;
            //stack.Children.Add(textBlock);
            icon = new Image();
            icon.VerticalAlignment = VerticalAlignment.Center;
            icon.Margin = new Thickness(0, 0, 4, 0);
            icon.Source = iconSource;
            stack.Children.Add(icon);
            //Add the HeaderText After Adding the icon
            textBlock = new TextBlock();
            textBlock.VerticalAlignment = VerticalAlignment.Center;
            stack.Children.Add(textBlock);
        }
        #endregion Constructors and Destructors
        #region Properties
        /// <summary>
        /// Gets/Sets the Selected Image for a TreeViewNode
        /// </summary>
        public ImageSource Icon
        {
            set
            {
                iconSource = value;
                icon.Source = iconSource;
            }
            get
            {
                return iconSource;
            }
        }
        #endregion Properties
        #region Event Handlers
        /// <summary>
        /// Event Handler on UnSelected Event
        /// </summary>
        /// <param name="args">Eventargs</param>
        protected override void OnUnselected(RoutedEventArgs args)
        {
            base.OnUnselected(args);
            icon.Source = iconSource;
        }
        /// <summary>
        /// Event Handler on Selected Event 
        /// </summary>
        /// <param name="args">Eventargs</param>
        protected override void OnSelected(RoutedEventArgs args)
        {
            base.OnSelected(args);
            icon.Source = iconSource;
        }

        /// <summary>
        /// Gets/Sets the HeaderText of TreeViewWithIcons
        /// </summary>
        public string HeaderText
        {
            set
            {
                textBlock.Text = value;
            }
            get
            {
                return textBlock.Text;
            }
        }
        #endregion Event Handlers
    }
}

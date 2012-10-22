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
using System.Windows.Shapes;
using Project422_2.api;

namespace Project422_2
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class AddNewDevice : Window
    {
        MainWindow handle;
        AddNewDeviceType deviceType;
        Device device;

        public AddNewDevice(MainWindow handle, AddNewDeviceType deviceType, int deviceId)
        {
            InitializeComponent();

            this.handle = handle;
            this.deviceType = deviceType;

            string name = "Root";
            device = DeviceList.getDevice(deviceId);

            if (device != null)
                name = device.DeviceName;

            switch (deviceType)
            {
                case AddNewDeviceType.PEER:
                    device = DeviceList.getDevice(deviceId);
                    addDeviceButton.Content = "Add Device";
                    instructionBox.Content = "Add new Peer device to " + name;
                    break;
                case AddNewDeviceType.SLAVE:
                    device = DeviceList.getDevice(deviceId);
                    addDeviceButton.Content = "Add Device";
                    instructionBox.Content = "Add new Peer slave to " + name;
                    break;
                case AddNewDeviceType.EDIT:
                    device = DeviceList.getDevice(deviceId);
                    addDeviceButton.Content = "Edit Device";
                    instructionBox.Content = "Edit device "+ name ;

                    ipAddress.Text = device.DomainName;
                    deviceName.Text = device.DeviceName;
                    break;
            }

        }


        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// sends the callback to the main window with the data entered in this window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addDeviceButton_Click(object sender, RoutedEventArgs e)
        {
            if (deviceType == AddNewDeviceType.PEER)
            {
                handle.addPeerDevice(ipAddress.Text, deviceName.Text);
            }
            else if (deviceType == AddNewDeviceType.SLAVE)
            {
                handle.addSlaveDevice(ipAddress.Text, deviceName.Text);
            }
            else if (deviceType == AddNewDeviceType.EDIT)
            {
                handle.editDevice(ipAddress.Text, deviceName.Text, device.DeviceId);
            }

            this.Close();
        }
    }
}

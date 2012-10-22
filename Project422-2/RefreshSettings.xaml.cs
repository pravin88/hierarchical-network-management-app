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

namespace Project422_2
{

    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class RefreshSettings : Window
    {

        MainWindow handle;

        public RefreshSettings(MainWindow handle)
        {
            this.handle = handle;

            InitializeComponent();

            updateGui();
        }

        private void updateGui()
        {
            autoRefreshInterval.Text = handle.pingTimer.Interval.ToString();

            if (handle.pingTimer.Enabled)
                autoUpdateStatus.Content = "Stop Auto Refresh";
            else
                autoUpdateStatus.Content = "Start Auto Refresh";
        }

        private void manualUpdateStatus_Click(object sender, RoutedEventArgs e)
        {
            handle.pingRequestAllDevices();
        }

        private void autoUpdateStatus_Click(object sender, RoutedEventArgs e)
        {
            if (handle.pingTimer.Enabled)
            {
                handle.pingTimer.Stop();
            }
            else
            {
                handle.pingTimer.Interval = int.Parse(autoRefreshInterval.Text);
                handle.pingTimer.Start();
            }

            updateGui();
        }
    }
}

using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp2
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        byte[] buffer = new byte[1024];
        Socket socket;

        private void Communicate(object sender, RoutedEventArgs e)
        {
            try
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(ipAddress.Text, Convert.ToInt32(Portnum.Text));
                this.Dispatcher.Invoke(new Action(() => clientStatus.Text += DateTime.Now.ToString("MM-dd HH:mm:ss")));
                this.Dispatcher.Invoke(new Action(() => clientStatus.Text += "client:connected success!\n"));
                socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(MSGreceive), socket);
            }
            catch
            {
                this.Dispatcher.Invoke(new Action(() => clientStatus.Text += DateTime.Now.ToString("MM-dd HH:mm:ss")));
                this.Dispatcher.Invoke(new Action(() => clientStatus.Text += "client ERROR\n"));
            }
        }

        public void MSGreceive(IAsyncResult IAR)
        {
            try
            {
                var socket = IAR.AsyncState as Socket;
                var length = socket.EndReceive(IAR);
                var msg = Encoding.ASCII.GetString(buffer, 0, length);
                
                this.Dispatcher.Invoke(new Action(() => clientStatus.Text += DateTime.Now.ToString("MM-dd HH:mm:ss")));
                this.Dispatcher.Invoke(new Action(() => clientStatus.Text += "server:" + msg + "\n"));
                socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(MSGreceive), socket);
            }
            catch(Exception ex)
            {
                this.Dispatcher.Invoke(new Action(() => clientStatus.Text += DateTime.Now.ToString("MM-dd HH:mm:ss")));
                this.Dispatcher.Invoke(new Action(() => clientStatus.Text += (ex.Message + "\n")));
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MSGsend(object sender, RoutedEventArgs e)
        {
            var msg = text.Text;
            var outputBuffer = Encoding.UTF8.GetBytes(msg);
            socket.BeginSend(outputBuffer, 0, outputBuffer.Length, SocketFlags.None, null, null);
        }
    }
}

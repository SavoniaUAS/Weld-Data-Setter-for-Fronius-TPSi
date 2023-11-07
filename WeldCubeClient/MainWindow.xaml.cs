using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
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

namespace WeldDataSetter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string MessageTemplate = "3={0};4={1};";
        private const string SuccessTemplate = "Latest message: Item No: {0}, Serial No: {1}, at {2}";
        private const string ErrorTemplate = "Error sending message. Timestamp: {0}";

        private string logFile;
        private string prefsFile;

        public MainWindow()
        {
            InitializeComponent();

            InitLogging();

            InitDefaultValues();
        }

        private void InitDefaultValues()
        {
            prefsFile = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName), "defaults.pref");
            if (!File.Exists(prefsFile))
            {
                File.Create(prefsFile);
            }
            else
            {
                using StreamReader reader = new StreamReader(prefsFile);
                string content = reader.ReadToEnd();
                if (!string.IsNullOrEmpty(content))
                {
                    var prefs = Newtonsoft.Json.JsonConvert.DeserializeObject<Prefs>(content);
                    this.tbIp.Text = prefs.DefaultIp;
                    this.tbPort.Text = prefs.DefaultPort;
                }
            }
        }

        private void InitLogging()
        {
            string sDirectory = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "WeldCubeClient");

            if (!Directory.Exists(sDirectory))
            {
                Directory.CreateDirectory(sDirectory);
            }

            string fileName = $"log-{DateTimeOffset.Now.Ticks}.txt";
            logFile = System.IO.Path.Combine(sDirectory, fileName);
            using FileStream stream = File.Create(logFile);
            using StreamWriter writer = new StreamWriter(stream);
            writer.WriteInfo("Application started");
        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            //empty and collapse message fields
            this.blockSuccess.Text = "";
            this.stackSuccess.Visibility = Visibility.Collapsed;
            this.blockError.Text = "";
            this.stackError.Visibility = Visibility.Collapsed;
            this.labelSaved.Visibility = Visibility.Hidden;

            //read and check values
            string ip = this.tbIp.Text;
            string portText = this.tbPort.Text;
            string itemNo = this.tbItemNo.Text;
            string serialNo = this.tbSerialNo.Text;
            Int32 port;
            if (string.IsNullOrEmpty(ip) || string.IsNullOrEmpty(portText) || string.IsNullOrEmpty(itemNo) || string.IsNullOrEmpty(serialNo))
            {
                this.blockError.Text = "Fill in the missing values!";
                this.stackError.Visibility = Visibility.Visible;
            }
            else if (!Int32.TryParse(portText, out port))
            {
                this.blockError.Text = "The port must be a number!";
                this.stackError.Visibility = Visibility.Visible;
            }
            else
            {
                //create message
                string message = string.Format(MessageTemplate, itemNo, serialNo);

                //call send method
                bool success = SendTcpMessage(port, ip, message);

                //show appropriate message to user
                if (success)
                {
                    this.blockSuccess.Text = string.Format(SuccessTemplate, itemNo, serialNo, DateTimeOffset.Now.ToString());
                    this.stackSuccess.Visibility = Visibility.Visible;
                }
                else
                {
                    this.blockError.Text = string.Format(ErrorTemplate, DateTimeOffset.Now.ToString());
                    this.stackError.Visibility = Visibility.Visible;
                }
            }
        }

        private bool SendTcpMessage(Int32 port, string server, string message)
        {
            using StreamWriter writer = new StreamWriter(logFile, true);
            writer.WriteInfo("Sending TCP message");

            try
            {
                // Create a TcpClient
                TcpClient client = new TcpClient(server, port);
                //write connection to log
                writer.WriteInfo($"Connected to server {server}, port {port}");

                // Translate the passed message into ASCII and store it as a Byte array.
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);

                // Get a client stream for reading and writing.
                NetworkStream stream = client.GetStream();

                // Send the message to the connected TcpServer.
                stream.Write(data, 0, data.Length);
                //write to log
                writer.WriteInfo($"Sent message to server. Message: {message}");

                Console.WriteLine("Sent: {0}", message);

                // Receive the TcpServer.response.

                // Buffer to store the response bytes.
                data = new Byte[256];

                // String to store the response ASCII representation.
                String responseData = String.Empty;

                // Read the first batch of the TcpServer response bytes.
                //Int32 bytes = stream.Read(data, 0, data.Length);
                //responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);

                ////write response to log
                //writer.WriteInfo($"Received message from server. Message: \r\n {responseData}");

                // Close everything.
                stream.Close();
                client.Close();
                //write to log
                writer.WriteInfo("Closed connection to the server");

                return true;
            }
            catch (ArgumentNullException e)
            {
                //write error to log
                writer.WriteError(e);

                return false;
            }
            catch (SocketException e)
            {
                //write error to log
                writer.WriteError(e);

                return false;
            }
        }

        private void btnSetDefaults_Click(object sender, RoutedEventArgs e)
        {
            //empty and collapse message fields
            this.blockSuccess.Text = "";
            this.stackSuccess.Visibility = Visibility.Collapsed;
            this.blockError.Text = "";
            this.stackError.Visibility = Visibility.Collapsed;
            this.labelSaved.Visibility = Visibility.Hidden;

            using StreamWriter logwriter = new StreamWriter(logFile, true);
            logwriter.WriteInfo("Setting preferences");
            try
            {
                using StreamWriter writer = new StreamWriter(prefsFile);
                var prefs = new Prefs()
                {
                    DefaultIp = this.tbIp.Text,
                    DefaultPort = this.tbPort.Text
                };
                string content = Newtonsoft.Json.JsonConvert.SerializeObject(prefs);
                writer.Write(content);
                this.labelSaved.Visibility = Visibility.Visible;
                logwriter.WriteInfo("Preferences set");
            }
            catch (Exception ex)
            {
                logwriter.WriteError(ex);
                this.blockError.Text = string.Format("Error saving preferences. Timestamp: {0}", DateTimeOffset.Now.ToString());
                this.stackError.Visibility = Visibility.Visible;
            }
        }
    }
}

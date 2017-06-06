using System;
using System.Configuration;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.Net.Sockets;

namespace ZPLPrinterProject
{
    public partial class ZPLPrinterForm : Form
    {
        private List<string> temporaryFiles = new List<string>();

        public ZPLPrinterForm(string[] args)
        {
            InitializeComponent();

            if (args.Length > 0)
            {
                OpenFile(args[0]);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Load app settings
            var config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);

            widthTextbox.Text = config.AppSettings.Settings["width"].Value;
            heightTextbox.Text = config.AppSettings.Settings["height"].Value;
            unitsCombobox.SelectedItem = config.AppSettings.Settings["units"].Value;
        }

        private void ZPLPrinterForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Delete temporary files
            foreach (var filePath in temporaryFiles)
            {
                File.Delete(filePath);
            }

            // Save app settings
            var config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);

            config.AppSettings.Settings["width"].Value = widthTextbox.Text;
            config.AppSettings.Settings["height"].Value = heightTextbox.Text;
            config.AppSettings.Settings["units"].Value = unitsCombobox.SelectedItem.ToString();

            config.Save(ConfigurationSaveMode.Modified);
        }

        public void OpenFile(string path)
        {
            sourceTextBox.Text = File.ReadAllText(path);
            //previewButton_Click(null, null);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = "zpl files (*.zpl)|*.zpl";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    OpenFile(openFileDialog.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }

        private void previewButton_Click(object sender, EventArgs e)
        {
            previewButton.Enabled = false;

            // to inches
            double width = Double.Parse(widthTextbox.Text);
            double height = Double.Parse(heightTextbox.Text);

            if (unitsCombobox.SelectedItem.Equals("cm"))
            {
                width = width * 0.393701;
                height = height * 0.393701;
            }

            byte[] zplSourceBytes = Encoding.UTF8.GetBytes(sourceTextBox.Text);

            var request = (HttpWebRequest)WebRequest.Create("http://api.labelary.com/v1/printers/8dpmm/labels/" + width + "x" + height + "/");
            request.Method = "POST";
            request.Accept = "application/pdf";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = zplSourceBytes.Length;

            var requestStream = request.GetRequestStream();
            requestStream.Write(zplSourceBytes, 0, zplSourceBytes.Length);
            requestStream.Close();

            // Temp file
            string tempFileName = "temp_" + Guid.NewGuid().ToString() + ".pdf";

            temporaryFiles.Add(tempFileName);

            try
            {
                var response = (HttpWebResponse)request.GetResponse();
                var responseStream = response.GetResponseStream();
                var fileStream = File.Create(tempFileName);
                responseStream.CopyTo(fileStream);
                responseStream.Close();
                fileStream.Close();

                previewButton.Enabled = true;

                labelWebBrowser.Url = new Uri(String.Format("file:///{0}/" + tempFileName, Directory.GetCurrentDirectory()));
            }
            catch (WebException exception)
            {
                Console.WriteLine("Error: {0}", exception.Status);
            }
        }

        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void printButton_Click(object sender, EventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            printDialog.PrinterSettings = new PrinterSettings();

            if (DialogResult.OK == printDialog.ShowDialog(this))
            {
                RawPrinterHelper.SendStringToPrinter(printDialog.PrinterSettings.PrinterName, sourceTextBox.Text);
            }

            /*Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientSocket.NoDelay = true;

            IPAddress ip = IPAddress.Parse("192.168.1.22");
            IPEndPoint ipep = new IPEndPoint(ip, 9100);
            clientSocket.Connect(ipep);

            byte[] fileBytes = Encoding.ASCII.GetBytes(sourceTextBox.Text);

            clientSocket.Send(fileBytes);
            clientSocket.Close();*/
        }

    }
}

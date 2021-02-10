using SimpleTcp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace TCPServer
{
    public partial class FormServer : Form
    {
        public FormServer()
        {
            InitializeComponent();
        }

        SimpleTcpServer server;

        #region Buttons
        private void btnStart_Click(object sender, EventArgs e)
        {
            server.Start();
            txtInfo.Text += $"Starting....{Environment.NewLine}";
            btnStart.Enabled = false;
            btnSend.Enabled = true;
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (server.IsListening)
            {
                if (!string.IsNullOrEmpty(txtMessage.Text) && lstClientIP.SelectedItem != null)
                {
                    for (int i = 0; i < lstClientIP.SelectedItems.Count; i++)
                    {
                        server.Send(lstClientIP.SelectedItems[i].ToString(),DateTime.Now.ToString("HH:mm:ss")+" "+ "Server: " + txtMessage.Text);
                        txtInfo.Text += $"Server: {txtMessage.Text}{Environment.NewLine}";
                    }
                    txtMessage.Text = string.Empty;
                }
            }
        }
        #endregion

        private void Form1_Load(object sender, EventArgs e)
        {
            btnSend.Enabled = false;
            server = new SimpleTcpServer(txtIP.Text);
            server.Events.ClientConnected += Events_ClientConnected;
            server.Events.ClientDisconnected += Events_ClientDisconnected;
            server.Events.DataReceived += Events_DataReceived;
        }

        #region Connection Functions
        private void Events_DataReceived(object sender, DataReceivedEventArgs e)
        {
            string cMes = null;
            this.Invoke((MethodInvoker)delegate
            {
                cMes = "";
                txtInfo.Text += $"{e.IpPort}: {Encoding.UTF8.GetString(e.Data)}{Environment.NewLine}";
                cMes += $"{e.IpPort}: {Encoding.UTF8.GetString(e.Data)}";
                File.WriteAllText("~'\'Messages.txt", cMes);
               // lblMsg.Text = DateTime.Now.ToString();

            });
            if (server.IsListening)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    for (int i = 0; i < lstClientIP.SelectedItems.Count; i++)
                    {
                        server.Send(lstClientIP.SelectedItems[i].ToString(), cMes);
                        
                        // txtInfo.Text += $"{lstClientIP.SelectedItems[0].ToString()}: {lblMsg.Text}{Environment.NewLine}";
                    }
               //     server.Send(lstClientIP.SelectedItems[0].ToString(), lblMsg.Text);
                //    txtInfo.Text += $"{lstClientIP.SelectedItems[1].ToString()}: {lblMsg.Text}{Environment.NewLine}";
                  //  lblMsg.Text = string.Empty;
                });
            }
        }

        private void Events_ClientDisconnected(object sender, ClientDisconnectedEventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                txtInfo.Text += $"{e.IpPort} disconnected.{Environment.NewLine}";
                lstClientIP.Items.Remove(e.IpPort);
            });
        }

        private void Events_ClientConnected(object sender, ClientConnectedEventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                txtInfo.Text += $"{e.IpPort} connected.{Environment.NewLine}";
                lstClientIP.Items.Add(e.IpPort);
            });
        }
        #endregion        
    }
}

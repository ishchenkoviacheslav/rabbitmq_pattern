using Sharing.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Serializator;
namespace Client_WinForms
{
    public partial class Form1 : Form
    {
        ClientsAPI.ClientsAPI api;
        public Form1()
        {
            InitializeComponent();
            api = new ClientsAPI.ClientsAPI();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            OnlineStatus();
        }
       
        private void OnlineStatus()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        //change to offline
                        if (DateTime.UtcNow.Subtract(api.LastUpDate) > new TimeSpan(0, 0, 5))
                        {
                            this.Invoke((Action)delegate
                            {
                                status.Text = "offline";
                                status.ForeColor = Color.Red;
                            });

                            //try to ping if offline - may be was bad connection and this peer was deleted from peer-list on server(because peer was older than 5 sec)
                        }
                        else//change to online
                        {
                            this.Invoke((Action)delegate
                            {
                                status.Text = "online";
                                status.ForeColor = Color.Green;
                            });
                        }

                    }
                    catch (Exception)
                    {
                        this.Invoke((Action)delegate
                        {
                            status.Text = "error";
                            status.ForeColor = Color.OrangeRed;
                        });
                    }
                    Thread.Sleep(1000);
                }
            });
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            api.Dispose();
        }
    }
}

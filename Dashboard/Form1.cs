using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace Dashboard
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnServer_Click(object sender, EventArgs e)
        {
            try
            {
                // Từ thư mục Dashboard/bin/Debug → bước lên 3 cấp để đến thư mục solution
                string baseDir = Path.Combine(Application.StartupPath, @"..\..\..");
                string serverPath = Path.Combine(baseDir, @"LMS_Server\bin\Debug\LMS_Server.exe");

                Process.Start(Path.GetFullPath(serverPath));
                //MessageBox.Show("Server started successfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Cant start the server: " + ex.Message);
            }

        }

        private void btnClient_Click(object sender, EventArgs e)
        {
            try
            {
                string baseDir = Path.Combine(Application.StartupPath, @"..\..\..");
                string clientPath = Path.Combine(baseDir, @"Participants\bin\Debug\Participants.exe");

                Process.Start(Path.GetFullPath(clientPath));
                //MessageBox.Show("Client started successfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Cant start the client: " + ex.Message);
            }
        }
    }
}

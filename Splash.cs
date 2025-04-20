using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Urban
{
    public partial class Splash : Form
    {
        private Timer timer;

        public Splash()
        {
            InitializeComponent();
            progressBar1.Minimum = 0;
            progressBar1.Maximum = 100;
            progressBar1.Value = 0;
            progressBar1.Step = 7;

        }

        private void Splash_Load(object sender, EventArgs e)
        {
            timer = new Timer();
            timer.Interval = 100; // Adjust this for loading speed
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (progressBar1.Value < progressBar1.Maximum)
            {
                progressBar1.PerformStep();
            }
            else
            {
                timer.Stop();

                // After progress completes, show the Login form once
                Login login = new Login();
                login.Show();

                // Then close the splash screen
                this.Hide(); // or this.Close(); if this isn't your main form
            }
        }
    }
}

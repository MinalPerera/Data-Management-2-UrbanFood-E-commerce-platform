using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Urban
{
    public partial class SuppDash : Form
    {
        public SuppDash()
        {
            InitializeComponent();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Products customerForm = new Products();
            customerForm.Show();
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SupProfile customerForm = new SupProfile();
            customerForm.Show();
            this.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Sales customerForm = new Sales();
            customerForm.Show();
            this.Hide();
        }

        private void SuppDash_Load(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Hide();
            AdminLogin mainForm = new AdminLogin();
            mainForm.Show();
        }
    }
}

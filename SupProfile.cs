using Oracle.ManagedDataAccess.Client;
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
    public partial class SupProfile : Form
    {
        private string connectionString = "User Id=ADMIN;Password=Mm123412341234;" +
                 "Data Source=(DESCRIPTION=" +
                 "(ADDRESS=(PROTOCOL=tcps)(PORT=1522)(HOST=adb.ap-hyderabad-1.oraclecloud.com))" +
                 "(CONNECT_DATA=(SERVICE_NAME=geb558a3e2b27cd_mydb_medium.adb.oraclecloud.com))" +
                 "(SECURITY=(SSL_SERVER_DN_MATCH=yes)(MY_WALLET_DIRECTORY=C:\\Users\\Minal Perera\\Downloads\\Wallet_mydb)))";

        public SupProfile()
        {
            InitializeComponent();
        }

        private void SupProfile_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    conn.Open();

                    OracleCommand cmd = new OracleCommand("save_supp", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("p_nic", OracleDbType.Varchar2).Value = textBox1.Text;
                    cmd.Parameters.Add("p_first_name", OracleDbType.Varchar2).Value = textBox2.Text;
                    cmd.Parameters.Add("p_last_name", OracleDbType.Varchar2).Value = textBox3.Text;
                    cmd.Parameters.Add("p_contact", OracleDbType.Varchar2).Value = textBox4.Text;
                    cmd.Parameters.Add("p_email", OracleDbType.Varchar2).Value = textBox5.Text;
                    cmd.Parameters.Add("p_address", OracleDbType.Varchar2).Value = textBox6.Text;

                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Supplier information saved successfully!");
                    ClearForm();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

        }

        private void ClearForm()
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            textBox5.Clear();
            textBox6.Clear();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Hide();
            SuppDash mainForm = new SuppDash();
            mainForm.Show();
        }
    }
}

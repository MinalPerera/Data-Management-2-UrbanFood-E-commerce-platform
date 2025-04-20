using MongoDB.Driver.Core.Configuration;
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
    public partial class Products : Form
    {
        private string connectionString = "User Id=ADMIN;Password=Mm123412341234;" +
                                 "Data Source=(DESCRIPTION=" +
                                 "(ADDRESS=(PROTOCOL=tcps)(PORT=1522)(HOST=adb.ap-hyderabad-1.oraclecloud.com))" +
                                 "(CONNECT_DATA=(SERVICE_NAME=geb558a3e2b27cd_mydb_medium.adb.oraclecloud.com))" +
                                 "(SECURITY=(SSL_SERVER_DN_MATCH=yes)(MY_WALLET_DIRECTORY=C:\\Users\\Minal Perera\\Downloads\\Wallet_mydb)))";

        public Products()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                // Create connection to Oracle
                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    conn.Open();

                    // Create command to call our PL/SQL procedure
                    OracleCommand cmd = new OracleCommand("add_product", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Add parameters from the form
                    cmd.Parameters.Add("p_name", OracleDbType.Varchar2).Value = textBox1.Text;
                    cmd.Parameters.Add("p_description", OracleDbType.Varchar2).Value = textBox2.Text;
                    cmd.Parameters.Add("p_price", OracleDbType.Decimal).Value = decimal.Parse(textBox3.Text);
                    cmd.Parameters.Add("p_weight", OracleDbType.Decimal).Value = decimal.Parse(textBox4.Text);

                    // Execute the procedure
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Product added successfully!");

                    // Clear the form
                    textBox1.Clear();
                    textBox2.Clear();
                    textBox3.Clear();
                    textBox4.Clear();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
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

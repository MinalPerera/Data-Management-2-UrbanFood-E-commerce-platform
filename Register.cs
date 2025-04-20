using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Urban
{
    public partial class Register: Form
    {
        private string connectionString = "User Id=ADMIN;Password=Mm123412341234;" +
                                   "Data Source=(DESCRIPTION=" +
                                   "(ADDRESS=(PROTOCOL=tcps)(PORT=1522)(HOST=adb.ap-hyderabad-1.oraclecloud.com))" +
                                   "(CONNECT_DATA=(SERVICE_NAME=geb558a3e2b27cd_mydb_medium.adb.oraclecloud.com))" +
                                   "(SECURITY=(SSL_SERVER_DN_MATCH=yes)(MY_WALLET_DIRECTORY=C:\\Users\\Minal Perera\\Downloads\\Wallet_mydb)))";
        public Register()
        {
            InitializeComponent();
            txtPassword.PasswordChar = '*';
        }

        private bool ValidateInputs()
        {
            // Email validation using regex
            string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            if (string.IsNullOrWhiteSpace(txtEmail.Text) || !Regex.IsMatch(txtEmail.Text, emailPattern))
            {
                MessageBox.Show("Please enter a valid email address.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmail.Focus();
                return false;
            }

            // Password validation - at least 8 characters with numbers and letters
            if (string.IsNullOrWhiteSpace(txtPassword.Text) || txtPassword.Text.Length < 8 ||
                !Regex.IsMatch(txtPassword.Text, @"^(?=.*[A-Za-z])(?=.*\d).+$"))
            {
                MessageBox.Show("Password must be at least 8 characters and contain both letters and numbers.",
                    "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPassword.Focus();
                return false;
            }

            return true;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (!ValidateInputs())
                return;

            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text;

            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    // Call the PL/SQL stored procedure instead of direct SQL
                    using (OracleCommand cmd = new OracleCommand("BEGIN REGISTER_USER(:p_email, :p_password, :p_result, :p_message); END;", conn))
                    {
                        // Input parameters
                        cmd.Parameters.Add("p_email", OracleDbType.Varchar2).Value = email;
                        cmd.Parameters.Add("p_password", OracleDbType.Varchar2).Value = password;

                        // Output parameters
                        cmd.Parameters.Add("p_result", OracleDbType.Int32).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add("p_message", OracleDbType.Varchar2, 200).Direction = ParameterDirection.Output;

                        cmd.ExecuteNonQuery();

                        // Get results from the procedure
                        int result = Convert.ToInt32(cmd.Parameters["p_result"].Value.ToString());
                        string message = cmd.Parameters["p_message"].Value.ToString();

                        if (result == 1)
                        {
                            MessageBox.Show(message, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            Login customerForm = new Login();
                            customerForm.Show();
                            this.Hide();
                        }
                        else
                        {
                            MessageBox.Show(message, "Registration Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            
        }

        private void Register_Load(object sender, EventArgs e)
        {
            txtEmail.Focus();
            txtPassword.PasswordChar = '*';
        }

        private void checkBoxShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            txtPassword.PasswordChar = checkBoxShowPassword.Checked ? '\0' : '*';
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Hide();
            Login mainForm = new Login();
            mainForm.Show();
        }
    }
}

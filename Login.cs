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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Urban
{
    public partial class Login: Form
    {

        private string connectionString = "User Id=ADMIN;Password=Mm123412341234;" +
                                  "Data Source=(DESCRIPTION=" +
                                  "(ADDRESS=(PROTOCOL=tcps)(PORT=1522)(HOST=adb.ap-hyderabad-1.oraclecloud.com))" +
                                  "(CONNECT_DATA=(SERVICE_NAME=geb558a3e2b27cd_mydb_medium.adb.oraclecloud.com))" +
                                  "(SECURITY=(SSL_SERVER_DN_MATCH=yes)(MY_WALLET_DIRECTORY=C:\\Users\\Minal Perera\\Downloads\\Wallet_mydb)))";

        public Login()
        {
            InitializeComponent();
            txtPassword.PasswordChar = '*';
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // validation
            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Please enter your email.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmail.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Please enter your password.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPassword.Focus();
                return;
            }

            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text.Trim();

            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    // PL/SQL for validation
                    string plsql = @"
                    DECLARE
                        v_count NUMBER := 0;
                        v_result VARCHAR2(100);
                    BEGIN
                        -- Validate credentials
                        SELECT COUNT(*) INTO v_count 
                        FROM Users 
                        WHERE Email = :email 
                        AND Password = :password;
                        
                        IF v_count > 0 THEN
                            :result := 'SUCCESS';
                        ELSE
                            :result := 'FAILED';
                        END IF;
                    EXCEPTION
                        WHEN OTHERS THEN
                            :result := 'ERROR';
                    END;";

                    using (OracleCommand cmd = new OracleCommand(plsql, conn))
                    {
                        // Add parameters
                        cmd.CommandType = CommandType.Text;
                        cmd.BindByName = true;

                        cmd.Parameters.Add("email", OracleDbType.Varchar2).Value = email;
                        cmd.Parameters.Add("password", OracleDbType.Varchar2).Value = password;

                        OracleParameter resultParam = new OracleParameter("result", OracleDbType.Varchar2, 100);
                        resultParam.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(resultParam);

                        cmd.ExecuteNonQuery();

                        string result = cmd.Parameters["result"].Value.ToString();

                        if (result == "SUCCESS")
                        {
                            MessageBox.Show("Login Successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);


                            ProductDash NMarket = new ProductDash();
                            NMarket.Show();
                            this.Hide();
                        }
                        else
                        {
                            MessageBox.Show("Invalid email or password!", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Database Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void txtPassword_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void txtEmail_TextChanged(object sender, EventArgs e)
        {

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

            Register customerForm = new Register();
            customerForm.Show();
            this.Hide();
        }

        private void Login_Load(object sender, EventArgs e)
        {
            txtPassword.PasswordChar = '*';
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            txtPassword.PasswordChar = checkBoxShowPassword.Checked ? '\0' : '*';
        }



        private void pictureBox1_Click(object sender, EventArgs e)
        {
            AdminLogin NMarket = new AdminLogin();
            NMarket.Show();
            this.Hide();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

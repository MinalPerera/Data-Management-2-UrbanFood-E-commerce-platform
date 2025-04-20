using System;
using System.Data;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;

namespace Urban
{
    public partial class CardPay : Form
    {
        private string connectionString = "User Id=ADMIN;Password=Mm123412341234;" +
                         "Data Source=(DESCRIPTION=" +
                         "(ADDRESS=(PROTOCOL=tcps)(PORT=1522)(HOST=adb.ap-hyderabad-1.oraclecloud.com))" +
                         "(CONNECT_DATA=(SERVICE_NAME=geb558a3e2b27cd_mydb_medium.adb.oraclecloud.com))" +
                         "(SECURITY=(SSL_SERVER_DN_MATCH=yes)(MY_WALLET_DIRECTORY=C:/Users/Minal Perera/Downloads/Wallet_mydb)))";

        public CardPay()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Validations
            if (string.IsNullOrWhiteSpace(textBox1.Text) ||
                string.IsNullOrWhiteSpace(textBox2.Text) ||
                string.IsNullOrWhiteSpace(textBox3.Text) ||
                string.IsNullOrWhiteSpace(textBox4.Text))
            {
                MessageBox.Show("Please fill all the necessary information!", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Credit Card Number Validation (numeric and length check)
            if (!System.Text.RegularExpressions.Regex.IsMatch(textBox1.Text, @"^\d{16}$"))
            {
                MessageBox.Show("Credit card number must be 16 digits!", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Expiration Date Validation (MM/YY format)
            if (!System.Text.RegularExpressions.Regex.IsMatch(textBox3.Text, @"^(0[1-9]|1[0-2])\/([0-9]{2})$"))
            {
                MessageBox.Show("Expiration date must be in MM/YY format!", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // CVV Validation (3-4 digits)
            if (!System.Text.RegularExpressions.Regex.IsMatch(textBox4.Text, @"^\d{3,4}$"))
            {
                MessageBox.Show("CVV must be 3 or 4 digits!", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Call the stored procedure instead of direct SQL insert
                using (OracleConnection myConnection = new OracleConnection(connectionString))
                {
                    myConnection.Open();
                    using (OracleCommand myCommand = new OracleCommand("ADD_PAYMENT_DETAILS", myConnection))
                    {
                        myCommand.CommandType = CommandType.StoredProcedure;

                        // Input parameters
                        myCommand.Parameters.Add("p_name", OracleDbType.Varchar2).Value = textBox2.Text;
                        myCommand.Parameters.Add("p_card_number", OracleDbType.Varchar2).Value = textBox1.Text;
                        myCommand.Parameters.Add("p_expiration", OracleDbType.Varchar2).Value = textBox3.Text;
                        myCommand.Parameters.Add("p_cvv", OracleDbType.Varchar2).Value = textBox4.Text;

                        // Output parameter for status
                        OracleParameter statusParam = new OracleParameter("p_status", OracleDbType.Varchar2, 100);
                        statusParam.Direction = ParameterDirection.Output;
                        myCommand.Parameters.Add(statusParam);

                        myCommand.ExecuteNonQuery();

                        // Check the status returned by the stored procedure
                        string status = statusParam.Value.ToString();
                        if (status == "SUCCESS")
                        {
                            MessageBox.Show("Credit card added successfully!", "Success",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Open Tracking form and close current form
                            Tracking customerForm = new Tracking();
                            customerForm.Show();
                            this.Hide();
                        }
                        else
                        {
                            MessageBox.Show("Error: " + status, "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "System Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CardPay_Load(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}

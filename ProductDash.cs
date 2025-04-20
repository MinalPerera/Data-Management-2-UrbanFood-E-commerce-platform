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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace Urban
{
    public partial class ProductDash : Form
    {

        private string connectionString = "User Id=ADMIN;Password=Mm123412341234;" +
                         "Data Source=(DESCRIPTION=" +
                         "(ADDRESS=(PROTOCOL=tcps)(PORT=1522)(HOST=adb.ap-hyderabad-1.oraclecloud.com))" +
                         "(CONNECT_DATA=(SERVICE_NAME=geb558a3e2b27cd_mydb_medium.adb.oraclecloud.com))" +
                         "(SECURITY=(SSL_SERVER_DN_MATCH=yes)(MY_WALLET_DIRECTORY=C:\\Users\\Minal Perera\\Downloads\\Wallet_mydb)))";

        private decimal riceprice = 210, cornprice = 130, carrotprice = 250, potatoprice = 180;
        private const int MaxQuantity = 100;

        // Event handlers for numeric up-down controls
        private void numericUpDownrice_ValueChanged(object sender, EventArgs e) => CalculateSubtotal();
        private void numericUpDowncorn_ValueChanged(object sender, EventArgs e) => CalculateSubtotal();
        private void numericUpDowncarrot_ValueChanged(object sender, EventArgs e) => CalculateSubtotal();
        private void numericUpDownpotato_ValueChanged(object sender, EventArgs e) => CalculateSubtotal();



        public ProductDash()
        {
            InitializeComponent();

            // Set maximum values for the numeric up down controls
            numericUpDownrice.Maximum = MaxQuantity;
            numericUpDowncorn.Maximum = MaxQuantity;
            numericUpDowncarrot.Maximum = MaxQuantity;
            numericUpDownpotato.Maximum = MaxQuantity;
        }

        public void CalculateSubtotal()
        {
            decimal total = (numericUpDownrice.Value * riceprice) +
                           (numericUpDowncorn.Value * cornprice) +
                           (numericUpDowncarrot.Value * carrotprice) +
                           (numericUpDownpotato.Value * potatoprice);

            txtsubtotal.Text = total.ToString("Rs.0.00");

            // Enable or disable the Add to Cart button based on total
            btnaddtocart.Enabled = total > 0;
        }

        private void ProductDash_Load(object sender, EventArgs e)
        {

        }

        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }

        private void lblr_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void lblca_Click(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void lblcarrot_Click(object sender, EventArgs e)
        {

        }

        private void lblRice_Click(object sender, EventArgs e)
        {

        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void lblc_Click(object sender, EventArgs e)
        {

        }

        private void lblcorn_Click(object sender, EventArgs e)
        {

        }

        private void groupBox4_Enter(object sender, EventArgs e)
        {

        }

        private void txtsubtotal_TextChanged(object sender, EventArgs e)
        {

        }

        private void lblpotato_Click(object sender, EventArgs e)
        {

        }

        private void lblp_Click(object sender, EventArgs e)
        {

        }

        private void lblsubtotal_Click(object sender, EventArgs e)
        {

        }

        private bool ValidateQuantities()
        {
            errorProvider1.Clear();
            bool isValid = true;

            // Client-side validation for rice quantity
            if (numericUpDownrice.Value > MaxQuantity)
            {
                errorProvider1.SetError(numericUpDownrice, $"Maximum quantity is {MaxQuantity}");
                isValid = false;
            }

            // Client-side validation for corn quantity
            if (numericUpDowncorn.Value > MaxQuantity)
            {
                errorProvider1.SetError(numericUpDowncorn, $"Maximum quantity is {MaxQuantity}");
                isValid = false;
            }

            // Client-side validation for carrot quantity
            if (numericUpDowncarrot.Value > MaxQuantity)
            {
                errorProvider1.SetError(numericUpDowncarrot, $"Maximum quantity is {MaxQuantity}");
                isValid = false;
            }

            // Client-side validation for potato quantity
            if (numericUpDownpotato.Value > MaxQuantity)
            {
                errorProvider1.SetError(numericUpDownpotato, $"Maximum quantity is {MaxQuantity}");
                isValid = false;
            }

            return isValid;
        }

        private void btnaddtocart_Click(object sender, EventArgs e)
        {
            // Client-side validation
            if (!ValidateQuantities())
            {
                MessageBox.Show("Please correct the quantity errors.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                bool anyItemsAdded = false;
                string errorMessages = string.Empty;

                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    conn.Open();

                    // Add rice to cart if quantity > 0
                    if (numericUpDownrice.Value > 0)
                    {
                        var result = AddToCartUsingProcedure(conn, "Rice", (int)numericUpDownrice.Value,
                            numericUpDownrice.Value * riceprice);

                        if (result.Success)
                            anyItemsAdded = true;
                        else
                            errorMessages += $"Rice: {result.Message}\n";
                    }

                    // Add corn to cart if quantity > 0
                    if (numericUpDowncorn.Value > 0)
                    {
                        var result = AddToCartUsingProcedure(conn, "Corn", (int)numericUpDowncorn.Value,
                            numericUpDowncorn.Value * cornprice);

                        if (result.Success)
                            anyItemsAdded = true;
                        else
                            errorMessages += $"Corn: {result.Message}\n";
                    }

                    // Add carrot to cart if quantity > 0
                    if (numericUpDowncarrot.Value > 0)
                    {
                        var result = AddToCartUsingProcedure(conn, "Carrot", (int)numericUpDowncarrot.Value,
                            numericUpDowncarrot.Value * carrotprice);

                        if (result.Success)
                            anyItemsAdded = true;
                        else
                            errorMessages += $"Carrot: {result.Message}\n";
                    }

                    // Add potato to cart if quantity > 0
                    if (numericUpDownpotato.Value > 0)
                    {
                        var result = AddToCartUsingProcedure(conn, "Potato", (int)numericUpDownpotato.Value,
                            numericUpDownpotato.Value * potatoprice);

                        if (result.Success)
                            anyItemsAdded = true;
                        else
                            errorMessages += $"Potato: {result.Message}\n";
                    }
                }

                if (anyItemsAdded)
                {
                    if (string.IsNullOrEmpty(errorMessages))
                        MessageBox.Show("Items added to cart successfully!", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                        MessageBox.Show($"Some items were added to cart.\n\nErrors:\n{errorMessages}",
                            "Partial Success", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    this.Hide();
                    Checkout cart = new Checkout();
                    cart.ShowDialog();
                    this.Close();
                }
                else if (!string.IsNullOrEmpty(errorMessages))
                {
                    MessageBox.Show($"Failed to add items to cart:\n{errorMessages}",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("No items were selected to add to the cart.",
                        "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        private (bool Success, string Message) AddToCartUsingProcedure(OracleConnection conn, string name, int quantity, decimal price)
        {
            using (OracleCommand cmd = conn.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "add_to_cart";

                // Input parameters
                cmd.Parameters.Add("p_name", OracleDbType.Varchar2).Value = name;
                cmd.Parameters.Add("p_quantity", OracleDbType.Int32).Value = quantity;
                cmd.Parameters.Add("p_price", OracleDbType.Decimal).Value = price;

                // Output parameters
                OracleParameter resultParam = new OracleParameter("p_result", OracleDbType.Int32);
                resultParam.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(resultParam);

                OracleParameter messageParam = new OracleParameter("p_message", OracleDbType.Varchar2, 1000);
                messageParam.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(messageParam);

                cmd.ExecuteNonQuery();

                int result = Convert.ToInt32(resultParam.Value.ToString());
                string message = messageParam.Value.ToString();

                return (result == 0, message);
            }
        }

    }
}

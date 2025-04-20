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
    public partial class Checkout : Form
    {
        private string connectionString = "User Id = ADMIN; Password=Mm123412341234;" +
                        "Data Source=(DESCRIPTION=" +
                        "(ADDRESS=(PROTOCOL=tcps)(PORT=1522)(HOST=adb.ap-hyderabad-1.oraclecloud.com))" +
                        "(CONNECT_DATA=(SERVICE_NAME=geb558a3e2b27cd_mydb_medium.adb.oraclecloud.com))" +
                        "(SECURITY=(SSL_SERVER_DN_MATCH=yes)(MY_WALLET_DIRECTORY=C:\\Users\\Minal Perera\\Downloads\\Wallet_mydb)))";

        public Checkout()
        {
            InitializeComponent();
        }

        private void LoadLastCartRow()
        {
            try
            {
                string query = "SELECT * FROM (SELECT * FROM Cart1 ORDER BY ID DESC) WHERE ROWNUM = 1";
                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    conn.Open();
                    OracleDataAdapter da = new OracleDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        dataGridView1.DataSource = dt;
                        button2.Enabled = true; // Enable proceed button
                    }
                    else
                    {
                        MessageBox.Show("No items in cart!", "Empty Cart", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        button2.Enabled = false; // Disable proceed button
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading cart: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Checkout_Load(object sender, EventArgs e)
        {
            LoadLastCartRow();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count <= 0)
            {
                MessageBox.Show("Please select a row to delete!", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                int cartID = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["ID"].Value);

                // Confirm deletion
                DialogResult result = MessageBox.Show("Are you sure you want to delete this item?",
                    "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    // Call the PL/SQL procedure to delete the cart item
                    using (OracleConnection conn = new OracleConnection(connectionString))
                    {
                        conn.Open();
                        using (OracleCommand cmd = new OracleCommand("BEGIN DELETE_CART_ITEM(:p_cart_id, :p_status); END;", conn))
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.Add("p_cart_id", OracleDbType.Int32).Value = cartID;
                            cmd.Parameters.Add("p_status", OracleDbType.Varchar2, 100).Direction = ParameterDirection.Output;

                            cmd.ExecuteNonQuery();

                            string status = cmd.Parameters["p_status"].Value.ToString();
                            MessageBox.Show(status, "Cart Operation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    LoadLastCartRow(); // Refresh the grid
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting cart item: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            CusDetails customerForm = new CusDetails();
            customerForm.Show();
            this.Hide();
        }
    }
}

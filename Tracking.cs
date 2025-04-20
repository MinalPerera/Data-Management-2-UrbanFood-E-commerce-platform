using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Urban
{
    public partial class Tracking : Form
    {
        private string connectionString = "User Id=ADMIN;Password=Mm123412341234;" +
                                    "Data Source=(DESCRIPTION=" +
                                    "(ADDRESS=(PROTOCOL=tcps)(PORT=1522)(HOST=adb.ap-hyderabad-1.oraclecloud.com))" +
                                    "(CONNECT_DATA=(SERVICE_NAME=geb558a3e2b27cd_mydb_medium.adb.oraclecloud.com))" +
                                    "(SECURITY=(SSL_SERVER_DN_MATCH=yes)(MY_WALLET_DIRECTORY=C:\\Users\\Minal Perera\\Downloads\\Wallet_mydb)))";
        public Tracking()
        {
            InitializeComponent();

        }


        private void LoadOrder()
        {
            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    // Create command object for stored procedure
                    OracleCommand cmd = new OracleCommand("get_latest_order", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Define output parameters
                    cmd.Parameters.Add("p_order_id", OracleDbType.Int32).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("p_tracking_number", OracleDbType.Varchar2, 50).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("p_status", OracleDbType.Varchar2, 50).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("p_order_date", OracleDbType.Date).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("p_estimated_delivery", OracleDbType.Date).Direction = ParameterDirection.Output;

                    // Execute the stored procedure
                    cmd.ExecuteNonQuery();

                    // Check if order was found
                    if (cmd.Parameters["p_order_id"].Value != DBNull.Value)
                    {
                        lblOrderID.Text = cmd.Parameters["p_order_id"].Value.ToString();
                        lblTrackingNumber.Text = cmd.Parameters["p_tracking_number"].Value.ToString();

                        // Fix the date conversion issue
                        OracleDate orderDate = (OracleDate)cmd.Parameters["p_order_date"].Value;
                        OracleDate estimatedDelivery = (OracleDate)cmd.Parameters["p_estimated_delivery"].Value;

                        lblOrderDate.Text = orderDate.Value.ToString("yyyy-MM-dd");
                        lblEstimatedDelivery.Text = estimatedDelivery.Value.ToString("yyyy-MM-dd");

                        // Update order status in UI
                        UpdateOrderStatus(cmd.Parameters["p_status"].Value.ToString());
                    }
                    else
                    {
                        MessageBox.Show("No orders found!");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void LoadCartItems()
        {
            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    // Create command object for stored procedure
                    OracleCommand cmd = new OracleCommand("get_latest_order_items", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Define output cursor parameter
                    OracleParameter cursorParam = new OracleParameter("p_cursor", OracleDbType.RefCursor);
                    cursorParam.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(cursorParam);

                    // Execute and get reader
                    OracleDataReader reader = cmd.ExecuteReader();

                    listView1.Items.Clear();

                    while (reader.Read())
                    {
                        ListViewItem item = new ListViewItem(reader["ProductName"].ToString());
                        item.SubItems.Add(reader["Quantity"].ToString());
                        item.SubItems.Add("LKR " + reader["Price"].ToString());

                        listView1.Items.Add(item);
                    }

                    if (listView1.Items.Count == 0)
                    {
                        ListViewItem noItems = new ListViewItem("No items in cart.");
                        noItems.ForeColor = Color.Gray;
                        listView1.Items.Add(noItems);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading cart items: " + ex.Message);
                }
            }
        }


        private void UpdateOrderStatus(string status)
        {
            switch (status)
            {
                case "Ordered":
                    progressBar1.Value = 25;
                    lblStatus.Text = "Ordered ✅";
                    break;
                case "In Transit":
                    progressBar1.Value = 50;
                    lblStatus.Text = "In Transit ✅";
                    break;
                case "Out for Delivery":
                    progressBar1.Value = 75;
                    lblStatus.Text = "Out for Delivery ✅";
                    break;
                case "Delivered":
                    progressBar1.Value = 100;
                    lblStatus.Text = "Delivered ✅";
                    break;
                default:
                    lblStatus.Text = "Unknown Status";
                    progressBar1.Value = 0;
                    break;
            }

            
        }

        private void progressBar1_Click(object sender, EventArgs e)
        {
            // Optional: Add extra functionality if needed
        }

        private void Tracking_Load(object sender, EventArgs e)
        {
            // Configure ListView
            listView1.View = View.Details;
            listView1.FullRowSelect = true;
            listView1.GridLines = true;

            // Add columns
            listView1.Columns.Clear();
            listView1.Columns.Add("Product Name", 200);
            listView1.Columns.Add("Quantity", 80);
            listView1.Columns.Add("Price", 80);

            LoadOrder();
            LoadCartItems();

        }

        private void button1_Click(object sender, EventArgs e)
        {

            Review customerForm = new Review();
            customerForm.Show();
            this.Hide();
        }
    }
}

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
    public partial class Sales : Form
    {
        private string connectionString = "User Id=ADMIN;Password=Mm123412341234;" +
                  "Data Source=(DESCRIPTION=" +
                  "(ADDRESS=(PROTOCOL=tcps)(PORT=1522)(HOST=adb.ap-hyderabad-1.oraclecloud.com))" +
                  "(CONNECT_DATA=(SERVICE_NAME=geb558a3e2b27cd_mydb_medium.adb.oraclecloud.com))" +
                  "(SECURITY=(SSL_SERVER_DN_MATCH=yes)(MY_WALLET_DIRECTORY=C:\\Users\\Minal Perera\\Downloads\\Wallet_mydb)))";

        public Sales()
        {
            InitializeComponent();
            SetupPLSQLProcedures();
        }

        private void SetupPLSQLProcedures()
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    conn.Open();

                    // Create procedure to get all cart items
                    string createGetAllCartItemsProc = @"
                    CREATE OR REPLACE PROCEDURE get_all_cart_items(p_cursor OUT SYS_REFCURSOR) 
                    AS
                    BEGIN
                        OPEN p_cursor FOR
                        SELECT * FROM cart1;
                    END;";

                    // Create procedure to get high demand product
                    string createGetHighDemandProc = @"
                    CREATE OR REPLACE PROCEDURE get_high_demand_product(p_product_name OUT VARCHAR2) 
                    AS
                    BEGIN
                        SELECT NAME INTO p_product_name
                        FROM (
                            SELECT NAME, SUM(QUANTITY) as TOTAL_QUANTITY
                            FROM cart1
                            GROUP BY NAME
                            ORDER BY TOTAL_QUANTITY DESC
                        )
                        WHERE ROWNUM = 1;
                    EXCEPTION
                        WHEN NO_DATA_FOUND THEN
                            p_product_name := 'No data found';
                    END;";

                    // Execute the procedure creation statements
                    using (OracleCommand cmd = new OracleCommand(createGetAllCartItemsProc, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    using (OracleCommand cmd = new OracleCommand(createGetHighDemandProc, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error setting up PL/SQL procedures: " + ex.Message);
            }
        }

        private void LoadAllCartRows()
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    conn.Open();

                    using (OracleCommand cmd = new OracleCommand("get_all_cart_items", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Add the output parameter for the cursor
                        OracleParameter cursorParam = new OracleParameter("p_cursor", OracleDbType.RefCursor);
                        cursorParam.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(cursorParam);

                        // Execute the command and fill the data adapter
                        OracleDataAdapter da = new OracleDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        // Check if any data was retrieved
                        if (dt.Rows.Count > 0)
                        {
                            dataGridView1.DataSource = dt;
                            // Make sure all columns are visible
                            dataGridView1.AutoResizeColumns();
                        }
                        else
                        {
                            MessageBox.Show("No data found in cart1 table.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data: " + ex.Message);
            }
        }

        private void LoadHighDemandProduct()
        {
            try
            {
                string query = @"
            SELECT NAME, SUM(QUANTITY) as TOTAL_QUANTITY
            FROM cart1
            GROUP BY NAME
            ORDER BY TOTAL_QUANTITY DESC
            FETCH FIRST 1 ROW ONLY";

                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    conn.Open();
                    OracleCommand cmd = new OracleCommand(query, conn);
                    object result = cmd.ExecuteScalar();

                    if (result != null)
                    {
                        // Assuming textBoxHighDemand is the name of your textbox
                        textBoxHighDemand.Text = result.ToString();
                    }
                    else
                    {
                        textBoxHighDemand.Text = "No data found";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error finding high demand product: " + ex.Message);
            }
        }

        private void Sales_Load(object sender, EventArgs e)
        {
            LoadAllCartRows(); // Move your method call here
            LoadHighDemandProduct();
        }

        private void button1_Click(object sender, EventArgs e)
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Urban
{
    public partial class Review : Form
    {
        private IMongoCollection<BsonDocument> _reviewCollection;


        public Review()
        {
            InitializeComponent();

            // Initialize MongoDB connection - simplified
            try
            {
                var settings = MongoClientSettings.FromConnectionString("mongodb://localhost:27017");
                settings.ServerSelectionTimeout = TimeSpan.FromSeconds(5);
                var client = new MongoClient(settings);
                var database = client.GetDatabase("UrbanDB");
                _reviewCollection = database.GetCollection<BsonDocument>("Reviews");

                // Create simple indexes
                CreateIndexes();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Database connection error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CreateIndexes()
        {
            try
            {
                // Create basic indexes for common queries
                var productNameIndex = Builders<BsonDocument>.IndexKeys.Ascending("ProductName");
                var ratingIndex = Builders<BsonDocument>.IndexKeys.Descending("Rating");
                var dateIndex = Builders<BsonDocument>.IndexKeys.Descending("Date");

                _reviewCollection.Indexes.CreateOne(new CreateIndexModel<BsonDocument>(productNameIndex));
                _reviewCollection.Indexes.CreateOne(new CreateIndexModel<BsonDocument>(ratingIndex));
                _reviewCollection.Indexes.CreateOne(new CreateIndexModel<BsonDocument>(dateIndex));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating indexes: {ex.Message}");
            }
        }

        // Insert Review into MongoDB
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                // Disable button to prevent multiple submissions
                button1.Enabled = false;

                string customerName = txtCustomerName.Text.Trim();
                string feedback = txtComment.Text.Trim();
                string productName = cmbProductName.SelectedItem?.ToString();
                int rating = (int)numRating.Value;

                if (string.IsNullOrEmpty(customerName) || string.IsNullOrEmpty(feedback) || string.IsNullOrEmpty(productName))
                {
                    MessageBox.Show("Please enter all required fields.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Create document with proper schema
                var document = new BsonDocument
                {
                    { "ProductName", productName },
                    { "CustomerName", customerName },
                    { "Feedback", feedback },
                    { "Rating", rating },
                    { "Date", DateTime.UtcNow }
                };

                // Insert the document into MongoDB - This was missing
                _reviewCollection.InsertOne(document);

                MessageBox.Show("Review submitted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Clear input fields
                txtCustomerName.Clear();
                txtComment.Clear();
                numRating.Value = 1;
                cmbProductName.SelectedIndex = 0;

                // Refresh the grid to show the new review
                LoadReviews();
            }
            catch (MongoException ex)
            {
                MessageBox.Show($"Database error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Re-enable the button
                button1.Enabled = true;
            }
        }

        private void Review_Load(object sender, EventArgs e)
        {
            LoadReviews();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridViewReviews.SelectedRows.Count == 0)
            {
                MessageBox.Show("Select a review to update.");
                return;
            }

            try
            {
                var row = dataGridViewReviews.SelectedRows[0];
                var id = row.Cells["Id"].Value.ToString();

                var filter = Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(id));
                var update = Builders<BsonDocument>.Update
                    .Set("ProductName", cmbProductName.Text)
                    .Set("CustomerName", txtCustomerName.Text)
                    .Set("Rating", (int)numRating.Value)
                    .Set("Feedback", txtComment.Text)
                    .Set("Date", DateTime.UtcNow);

                _reviewCollection.UpdateOne(filter, update);

                MessageBox.Show("Review updated successfully.");
                LoadReviews(); // Reload grid
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating review: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridViewReviews.SelectedRows.Count == 0)
            {
                MessageBox.Show("Select a review to delete.");
                return;
            }

            try
            {
                var row = dataGridViewReviews.SelectedRows[0];
                var id = row.Cells["Id"].Value.ToString();

                var filter = Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(id));
                _reviewCollection.DeleteOne(filter);

                MessageBox.Show("Review deleted successfully.");
                LoadReviews(); // Refresh grid
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting review: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridViewReviews_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewReviews.SelectedRows.Count > 0)
            {
                var row = dataGridViewReviews.SelectedRows[0];
                cmbProductName.Text = row.Cells["Product"].Value.ToString();
                txtCustomerName.Text = row.Cells["Customer"].Value.ToString();
                numRating.Value = Convert.ToDecimal(row.Cells["Rating"].Value);
                txtComment.Text = row.Cells["Comment"].Value.ToString();
            }
        }

        private void LoadReviews()
        {
            try
            {
                var reviews = _reviewCollection.Find(new BsonDocument()).ToList();

                dataGridViewReviews.DataSource = reviews.Select(r => new
                {
                    Id = r.GetValue("_id", BsonNull.Value).ToString(),
                    Product = r.Contains("ProductName") ? r["ProductName"].AsString : "N/A",
                    Customer = r.Contains("CustomerName") ? r["CustomerName"].AsString : "N/A",
                    Rating = r.Contains("Rating") ? r["Rating"].ToInt32() : 0,
                    Comment = r.Contains("Feedback") ? r["Feedback"].AsString : "",
                    Date = r.Contains("Date") ? r["Date"].ToUniversalTime().ToString("yyyy-MM-dd") : "N/A"
                }).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading reviews: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridViewReviews_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }
    }

}

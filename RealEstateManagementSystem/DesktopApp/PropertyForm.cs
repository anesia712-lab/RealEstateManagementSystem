using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace DesktopApp
{
    public class PropertyForm : Form
    {
        private TextBox txtAddress;
        private TextBox txtPrice; 
        private NumericUpDown numBedrooms;
        private NumericUpDown numBathrooms;
        private NumericUpDown numSquareFeet;
        private Button btnSave;
        private Button btnClear;
        private Button btnExit;
        private DataGridView dgvProperties;
        private string connStr = "Server=localhost,1433;Initial Catalog=RealEstateDB;User Id=sa;Password=RealEstate_local_1;Encrypt=True;TrustServerCertificate=True;";

        public PropertyForm()
        {
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.Text = "Manage Properties";
            this.Size = new Size(600, 500);
            this.StartPosition = FormStartPosition.CenterParent;

            Label lblAddress = new Label { Text = "&Address:", Location = new Point(20, 20), AutoSize = true };
            txtAddress = new TextBox { Location = new Point(120, 20), Width = 400 };

            Label lblPrice = new Label { Text = "&Price ($):", Location = new Point(20, 60), AutoSize = true };
            txtPrice = new TextBox { Location = new Point(120, 60), Width = 150 };

            Label lblBedrooms = new Label { Text = "&Bedrooms:", Location = new Point(20, 100), AutoSize = true };
            numBedrooms = new NumericUpDown { Location = new Point(120, 100), Width = 80, Minimum = 1, Maximum = 50 };

            Label lblBathrooms = new Label { Text = "B&athrooms:", Location = new Point(220, 100), AutoSize = true };
            numBathrooms = new NumericUpDown { Location = new Point(300, 100), Width = 80, Minimum = 1, Maximum = 50 };

            Label lblSquareFeet = new Label { Text = "Square &Feet:", Location = new Point(20, 140), AutoSize = true };
            numSquareFeet = new NumericUpDown { Location = new Point(120, 140), Width = 100, Minimum = 100, Maximum = 50000 };

            btnSave = new Button { Text = "&Save", Location = new Point(120, 180) };
            btnClear = new Button { Text = "&Clear", Location = new Point(210, 180) };
            btnExit = new Button { Text = "E&xit", Location = new Point(300, 180) };

            dgvProperties = new DataGridView { Location = new Point(20, 230), Width = 540, Height = 200, ReadOnly = true, AllowUserToAddRows = false };

            btnSave.Click += BtnSave_Click;
            btnClear.Click += BtnClear_Click;
            btnExit.Click += (s, e) => this.Close();

            this.Controls.Add(lblAddress); this.Controls.Add(txtAddress);
            this.Controls.Add(lblPrice); this.Controls.Add(txtPrice);
            this.Controls.Add(lblBedrooms); this.Controls.Add(numBedrooms);
            this.Controls.Add(lblBathrooms); this.Controls.Add(numBathrooms);
            this.Controls.Add(lblSquareFeet); this.Controls.Add(numSquareFeet);
            this.Controls.Add(btnSave); this.Controls.Add(btnClear); this.Controls.Add(btnExit);
            this.Controls.Add(dgvProperties);
        }

        private void LoadData()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    SqlDataAdapter da = new SqlDataAdapter("SELECT Id, Address, Price, Bedrooms, Bathrooms, SquareFeet FROM Properties", conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvProperties.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading data: " + ex.Message);
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtAddress.Text) || !decimal.TryParse(txtPrice.Text, out decimal price) || price < 1000)
            {
                MessageBox.Show("Please provide a valid address and a price of at least $1000.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtAddress.Focus(); 
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    string query = "INSERT INTO Properties (Address, Price, Bedrooms, Bathrooms, SquareFeet) VALUES (@Address, @Price, @Bed, @Bath, @SqFt)";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Address", txtAddress.Text);
                        cmd.Parameters.AddWithValue("@Price", price);
                        cmd.Parameters.AddWithValue("@Bed", numBedrooms.Value);
                        cmd.Parameters.AddWithValue("@Bath", numBathrooms.Value);
                        cmd.Parameters.AddWithValue("@SqFt", numSquareFeet.Value);
                        
                        cmd.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("Property saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
                BtnClear_Click(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving data. Ensure Database is running.\n" + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                BtnClear_Click(null, null); 
            }
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            txtAddress.Clear();
            txtPrice.Clear();
            numBedrooms.Value = 1;
            numBathrooms.Value = 1;
            numSquareFeet.Value = 100;
            txtAddress.Focus();
        }
    }
}

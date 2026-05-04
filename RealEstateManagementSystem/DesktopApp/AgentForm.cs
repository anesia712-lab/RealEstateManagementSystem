using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace DesktopApp
{
    public class AgentForm : Form
    {
        private TextBox txtFullName;
        private MaskedTextBox mtxtPhone; // Masked textbox
        private TextBox txtEmail;
        private TextBox txtAddress; // Multiline
        private DateTimePicker dtpJoinDate; // DatePicker
        private Button btnSave;
        private Button btnClear;
        private Button btnExit;
        private DataGridView dgvAgents;
        private string connStr = "Data Source=KHRISEAN;Initial Catalog=RealEstateDB;Integrated Security=True;TrustServerCertificate=True;";

        public AgentForm()
        {
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.Text = "Manage Agents";
            this.Size = new Size(600, 500);
            this.StartPosition = FormStartPosition.CenterParent;

            // Form controls with Access Keys (&)
            Label lblName = new Label { Text = "&Full Name:", Location = new Point(20, 20), AutoSize = true };
            txtFullName = new TextBox { Location = new Point(120, 20), Width = 150 };

            Label lblPhone = new Label { Text = "&Phone:", Location = new Point(20, 50), AutoSize = true };
            mtxtPhone = new MaskedTextBox { Location = new Point(120, 50), Width = 150, Mask = "000-000-0000" };

            Label lblEmail = new Label { Text = "&Email:", Location = new Point(20, 80), AutoSize = true };
            txtEmail = new TextBox { Location = new Point(120, 80), Width = 150 };

            Label lblJoinDate = new Label { Text = "&Join Date:", Location = new Point(20, 110), AutoSize = true };
            dtpJoinDate = new DateTimePicker { Location = new Point(120, 110), Width = 150, Format = DateTimePickerFormat.Short };

            Label lblAddress = new Label { Text = "&Address:", Location = new Point(300, 20), AutoSize = true };
            txtAddress = new TextBox { Location = new Point(380, 20), Width = 180, Height = 100, Multiline = true, ScrollBars = ScrollBars.Vertical };

            btnSave = new Button { Text = "&Save", Location = new Point(120, 150) };
            btnClear = new Button { Text = "&Clear", Location = new Point(210, 150) };
            btnExit = new Button { Text = "E&xit", Location = new Point(300, 150) };

            dgvAgents = new DataGridView { Location = new Point(20, 200), Width = 540, Height = 230, ReadOnly = true, AllowUserToAddRows = false };

            btnSave.Click += BtnSave_Click;
            btnClear.Click += BtnClear_Click;
            btnExit.Click += (s, e) => this.Close();

            this.Controls.Add(lblName); this.Controls.Add(txtFullName);
            this.Controls.Add(lblPhone); this.Controls.Add(mtxtPhone);
            this.Controls.Add(lblEmail); this.Controls.Add(txtEmail);
            this.Controls.Add(lblJoinDate); this.Controls.Add(dtpJoinDate);
            this.Controls.Add(lblAddress); this.Controls.Add(txtAddress);
            this.Controls.Add(btnSave); this.Controls.Add(btnClear); this.Controls.Add(btnExit);
            this.Controls.Add(dgvAgents);
        }

        private void LoadData()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    SqlDataAdapter da = new SqlDataAdapter("SELECT Id, FullName, Email, PhoneNumber FROM Agents", conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvAgents.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                // Ignoring error in UI if DB is not set up perfectly for desktop
                Console.WriteLine("Error loading data: " + ex.Message);
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(txtFullName.Text) || !mtxtPhone.MaskCompleted || string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Please fill all required fields properly.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtFullName.Focus(); // Setting focus back
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    string query = "INSERT INTO Agents (FullName, Email, PhoneNumber, LicenseNumber) VALUES (@Name, @Email, @Phone, @License)";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Name", txtFullName.Text);
                        cmd.Parameters.AddWithValue("@Email", txtEmail.Text);
                        cmd.Parameters.AddWithValue("@Phone", mtxtPhone.Text);
                        cmd.Parameters.AddWithValue("@License", "LIC-" + new Random().Next(1000, 9999));
                        
                        cmd.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("Agent saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
                BtnClear_Click(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving data. Ensure Database is running.\n" + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                BtnClear_Click(null, null); // Clear controls on error
            }
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            txtFullName.Clear();
            mtxtPhone.Clear();
            txtEmail.Clear();
            txtAddress.Clear();
            dtpJoinDate.Value = DateTime.Now;
            txtFullName.Focus();
        }
    }
}

using System;
using System.Drawing;
using System.Windows.Forms;

namespace DesktopApp
{
    public class MainForm : Form
    {
        private string currentUser;
        private MenuStrip menuStrip;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel statusLabel;
        private NotifyIcon notifyIcon;

        // Track single instances
        private AgentForm agentFormInstance;
        private ClientForm clientFormInstance;
        private PropertyForm propertyFormInstance;

        public MainForm(string username)
        {
            currentUser = username;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Real Estate Management System - " + currentUser;
            this.Size = new Size(800, 600);
            this.IsMdiContainer = true; // MDI requirement
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;

            // MenuStrip
            menuStrip = new MenuStrip();
            ToolStripMenuItem fileMenu = new ToolStripMenuItem("&File");
            ToolStripMenuItem exitMenuItem = new ToolStripMenuItem("E&xit", null, (s, e) => Application.Exit());
            ToolStripMenuItem logoutMenuItem = new ToolStripMenuItem("&Logout", null, (s, e) => {
                Application.Restart();
                Environment.Exit(0);
            });
            fileMenu.DropDownItems.Add(logoutMenuItem);
            fileMenu.DropDownItems.Add(exitMenuItem);

            ToolStripMenuItem manageMenu = new ToolStripMenuItem("&Manage");
            ToolStripMenuItem agentsMenuItem = new ToolStripMenuItem("&Agents", null, OpenAgentForm);
            ToolStripMenuItem clientsMenuItem = new ToolStripMenuItem("&Clients", null, OpenClientForm);
            ToolStripMenuItem propertiesMenuItem = new ToolStripMenuItem("&Properties", null, OpenPropertyForm);
            manageMenu.DropDownItems.Add(agentsMenuItem);
            manageMenu.DropDownItems.Add(clientsMenuItem);
            manageMenu.DropDownItems.Add(propertiesMenuItem);

            menuStrip.Items.Add(fileMenu);
            menuStrip.Items.Add(manageMenu);
            this.MainMenuStrip = menuStrip;
            this.Controls.Add(menuStrip);

            // StatusStrip
            statusStrip = new StatusStrip();
            statusLabel = new ToolStripStatusLabel("Logged in as: " + currentUser);
            statusStrip.Items.Add(statusLabel);
            this.Controls.Add(statusStrip);

            // NotifyIcon
            notifyIcon = new NotifyIcon();
            notifyIcon.Icon = SystemIcons.Information;
            notifyIcon.Visible = true;
            notifyIcon.Text = "Real Estate System Running";
            notifyIcon.BalloonTipTitle = "Welcome";
            notifyIcon.BalloonTipText = "System successfully started!";
            notifyIcon.ShowBalloonTip(3000);
        }

        private void OpenAgentForm(object sender, EventArgs e)
        {
            if (agentFormInstance == null || agentFormInstance.IsDisposed)
            {
                agentFormInstance = new AgentForm();
                agentFormInstance.MdiParent = this;
                agentFormInstance.Show();
            }
            else
            {
                agentFormInstance.WindowState = FormWindowState.Normal;
                agentFormInstance.BringToFront();
            }
        }

        private void OpenClientForm(object sender, EventArgs e)
        {
            if (clientFormInstance == null || clientFormInstance.IsDisposed)
            {
                clientFormInstance = new ClientForm();
                clientFormInstance.MdiParent = this;
                clientFormInstance.Show();
            }
            else
            {
                clientFormInstance.WindowState = FormWindowState.Normal;
                clientFormInstance.BringToFront();
            }
        }

        private void OpenPropertyForm(object sender, EventArgs e)
        {
            if (propertyFormInstance == null || propertyFormInstance.IsDisposed)
            {
                propertyFormInstance = new PropertyForm();
                propertyFormInstance.MdiParent = this;
                propertyFormInstance.Show();
            }
            else
            {
                propertyFormInstance.WindowState = FormWindowState.Normal;
                propertyFormInstance.BringToFront();
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (notifyIcon != null)
            {
                notifyIcon.Visible = false;
                notifyIcon.Dispose();
            }
            base.OnFormClosing(e);
        }
    }
}

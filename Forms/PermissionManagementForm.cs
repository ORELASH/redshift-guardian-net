using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using RedshiftGuardianNET.Models;
using RedshiftGuardianNET.Services;

namespace RedshiftGuardianNET.Forms
{
    /// <summary>
    /// Permission Management Form - GRANT/REVOKE operations, User and Group management
    /// </summary>
    public class PermissionManagementForm : Form
    {
        private Cluster _cluster;
        private RedshiftOdbcService _odbcService;
        private PermissionManagementService _permService;

        // Main controls
        private TabControl tabControl;
        private Panel topPanel;
        private Label lblCluster;
        private CheckBox chkReadOnlyMode;
        private Button btnClose;

        // Grant Permissions Tab
        private TabPage tabGrant;
        private ComboBox cmbGrantType;
        private TextBox txtGrantSchema;
        private TextBox txtGrantTable;
        private TextBox txtGrantUser;
        private CheckedListBox clbGrantPermissions;
        private CheckBox chkWithGrantOption;
        private Button btnGrant;
        private TextBox txtGrantResult;

        // Revoke Permissions Tab
        private TabPage tabRevoke;
        private ComboBox cmbRevokeType;
        private TextBox txtRevokeSchema;
        private TextBox txtRevokeTable;
        private TextBox txtRevokeUser;
        private CheckedListBox clbRevokePermissions;
        private Button btnRevoke;
        private TextBox txtRevokeResult;

        // User Management Tab
        private TabPage tabUsers;
        private GroupBox grpCreateUser;
        private TextBox txtNewUsername;
        private TextBox txtNewPassword;
        private NumericUpDown numConnectionLimit;
        private DateTimePicker dtpValidUntil;
        private CheckBox chkValidUntilEnabled;
        private Button btnCreateUser;
        private GroupBox grpAlterUser;
        private TextBox txtAlterUsername;
        private TextBox txtAlterPassword;
        private Button btnAlterPassword;
        private GroupBox grpDropUser;
        private TextBox txtDropUsername;
        private Button btnDropUser;
        private TextBox txtUserResult;

        // Group Management Tab
        private TabPage tabGroups;
        private GroupBox grpCreateGroup;
        private TextBox txtNewGroupName;
        private Button btnCreateGroup;
        private GroupBox grpManageMembers;
        private TextBox txtMemberGroupName;
        private TextBox txtMemberUsername;
        private Button btnAddMember;
        private Button btnRemoveMember;
        private GroupBox grpDropGroup;
        private TextBox txtDropGroupName;
        private Button btnDropGroup;
        private TextBox txtGroupResult;

        public PermissionManagementForm(Cluster cluster)
        {
            if (cluster == null)
                throw new ArgumentNullException("cluster");

            _cluster = cluster;
            InitializeComponent();
            InitializeServices();
        }

        private void InitializeComponent()
        {
            this.Text = "Permission Management";
            this.Size = new Size(900, 700);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Top panel
            topPanel = new Panel();
            topPanel.Dock = DockStyle.Top;
            topPanel.Height = 60;
            topPanel.BackColor = SystemColors.Control;

            lblCluster = new Label();
            lblCluster.Location = new Point(15, 15);
            lblCluster.Size = new Size(500, 20);
            lblCluster.Font = new Font(lblCluster.Font, FontStyle.Bold);
            topPanel.Controls.Add(lblCluster);

            chkReadOnlyMode = new CheckBox();
            chkReadOnlyMode.Text = "Read-Only Mode (Safe)";
            chkReadOnlyMode.Location = new Point(15, 35);
            chkReadOnlyMode.Size = new Size(200, 20);
            chkReadOnlyMode.Checked = true;
            chkReadOnlyMode.CheckedChanged += ChkReadOnlyMode_CheckedChanged;
            topPanel.Controls.Add(chkReadOnlyMode);

            btnClose = new Button();
            btnClose.Text = "Close";
            btnClose.Location = new Point(780, 15);
            btnClose.Size = new Size(100, 30);
            btnClose.Click += (s, e) => this.Close();
            topPanel.Controls.Add(btnClose);

            this.Controls.Add(topPanel);

            // Tab control
            tabControl = new TabControl();
            tabControl.Location = new Point(10, 70);
            tabControl.Size = new Size(870, 580);

            // Create tabs
            CreateGrantTab();
            CreateRevokeTab();
            CreateUserTab();
            CreateGroupTab();

            this.Controls.Add(tabControl);
        }

        private void CreateGrantTab()
        {
            tabGrant = new TabPage("Grant Permissions");

            Label lblType = new Label();
            lblType.Text = "Grant Type:";
            lblType.Location = new Point(20, 20);
            lblType.Size = new Size(100, 20);
            tabGrant.Controls.Add(lblType);

            cmbGrantType = new ComboBox();
            cmbGrantType.Location = new Point(130, 18);
            cmbGrantType.Size = new Size(200, 25);
            cmbGrantType.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbGrantType.Items.AddRange(new object[] {
                "Table Permission",
                "Schema Permission",
                "All Tables in Schema"
            });
            cmbGrantType.SelectedIndex = 0;
            cmbGrantType.SelectedIndexChanged += CmbGrantType_SelectedIndexChanged;
            tabGrant.Controls.Add(cmbGrantType);

            Label lblSchema = new Label();
            lblSchema.Text = "Schema:";
            lblSchema.Location = new Point(20, 55);
            lblSchema.Size = new Size(100, 20);
            tabGrant.Controls.Add(lblSchema);

            txtGrantSchema = new TextBox();
            txtGrantSchema.Location = new Point(130, 53);
            txtGrantSchema.Size = new Size(200, 25);
            txtGrantSchema.Text = "public";
            tabGrant.Controls.Add(txtGrantSchema);

            Label lblTable = new Label();
            lblTable.Text = "Table:";
            lblTable.Location = new Point(20, 90);
            lblTable.Size = new Size(100, 20);
            tabGrant.Controls.Add(lblTable);

            txtGrantTable = new TextBox();
            txtGrantTable.Location = new Point(130, 88);
            txtGrantTable.Size = new Size(200, 25);
            tabGrant.Controls.Add(txtGrantTable);

            Label lblUser = new Label();
            lblUser.Text = "User/Group:";
            lblUser.Location = new Point(20, 125);
            lblUser.Size = new Size(100, 20);
            tabGrant.Controls.Add(lblUser);

            txtGrantUser = new TextBox();
            txtGrantUser.Location = new Point(130, 123);
            txtGrantUser.Size = new Size(200, 25);
            tabGrant.Controls.Add(txtGrantUser);

            Label lblPermissions = new Label();
            lblPermissions.Text = "Permissions:";
            lblPermissions.Location = new Point(20, 160);
            lblPermissions.Size = new Size(100, 20);
            tabGrant.Controls.Add(lblPermissions);

            clbGrantPermissions = new CheckedListBox();
            clbGrantPermissions.Location = new Point(130, 160);
            clbGrantPermissions.Size = new Size(200, 120);
            clbGrantPermissions.Items.AddRange(new object[] {
                "SELECT", "INSERT", "UPDATE", "DELETE",
                "REFERENCES", "DROP", "ALTER", "ALL"
            });
            tabGrant.Controls.Add(clbGrantPermissions);

            chkWithGrantOption = new CheckBox();
            chkWithGrantOption.Text = "WITH GRANT OPTION";
            chkWithGrantOption.Location = new Point(130, 290);
            chkWithGrantOption.Size = new Size(200, 20);
            tabGrant.Controls.Add(chkWithGrantOption);

            btnGrant = new Button();
            btnGrant.Text = "GRANT Permission";
            btnGrant.Location = new Point(130, 320);
            btnGrant.Size = new Size(200, 35);
            btnGrant.BackColor = Color.FromArgb(76, 175, 80);
            btnGrant.ForeColor = Color.White;
            btnGrant.FlatStyle = FlatStyle.Flat;
            btnGrant.Click += BtnGrant_Click;
            tabGrant.Controls.Add(btnGrant);

            Label lblResult = new Label();
            lblResult.Text = "Result:";
            lblResult.Location = new Point(380, 20);
            lblResult.Size = new Size(100, 20);
            tabGrant.Controls.Add(lblResult);

            txtGrantResult = new TextBox();
            txtGrantResult.Location = new Point(380, 45);
            txtGrantResult.Size = new Size(450, 310);
            txtGrantResult.Multiline = true;
            txtGrantResult.ScrollBars = ScrollBars.Vertical;
            txtGrantResult.ReadOnly = true;
            txtGrantResult.Font = new Font("Consolas", 9);
            tabGrant.Controls.Add(txtGrantResult);

            tabControl.TabPages.Add(tabGrant);
        }

        private void CreateRevokeTab()
        {
            tabRevoke = new TabPage("Revoke Permissions");

            Label lblType = new Label();
            lblType.Text = "Revoke Type:";
            lblType.Location = new Point(20, 20);
            lblType.Size = new Size(100, 20);
            tabRevoke.Controls.Add(lblType);

            cmbRevokeType = new ComboBox();
            cmbRevokeType.Location = new Point(130, 18);
            cmbRevokeType.Size = new Size(200, 25);
            cmbRevokeType.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbRevokeType.Items.AddRange(new object[] {
                "Table Permission",
                "Schema Permission",
                "All Permissions"
            });
            cmbRevokeType.SelectedIndex = 0;
            cmbRevokeType.SelectedIndexChanged += CmbRevokeType_SelectedIndexChanged;
            tabRevoke.Controls.Add(cmbRevokeType);

            Label lblSchema = new Label();
            lblSchema.Text = "Schema:";
            lblSchema.Location = new Point(20, 55);
            lblSchema.Size = new Size(100, 20);
            tabRevoke.Controls.Add(lblSchema);

            txtRevokeSchema = new TextBox();
            txtRevokeSchema.Location = new Point(130, 53);
            txtRevokeSchema.Size = new Size(200, 25);
            txtRevokeSchema.Text = "public";
            tabRevoke.Controls.Add(txtRevokeSchema);

            Label lblTable = new Label();
            lblTable.Text = "Table:";
            lblTable.Location = new Point(20, 90);
            lblTable.Size = new Size(100, 20);
            tabRevoke.Controls.Add(lblTable);

            txtRevokeTable = new TextBox();
            txtRevokeTable.Location = new Point(130, 88);
            txtRevokeTable.Size = new Size(200, 25);
            tabRevoke.Controls.Add(txtRevokeTable);

            Label lblUser = new Label();
            lblUser.Text = "User/Group:";
            lblUser.Location = new Point(20, 125);
            lblUser.Size = new Size(100, 20);
            tabRevoke.Controls.Add(lblUser);

            txtRevokeUser = new TextBox();
            txtRevokeUser.Location = new Point(130, 123);
            txtRevokeUser.Size = new Size(200, 25);
            tabRevoke.Controls.Add(txtRevokeUser);

            Label lblPermissions = new Label();
            lblPermissions.Text = "Permissions:";
            lblPermissions.Location = new Point(20, 160);
            lblPermissions.Size = new Size(100, 20);
            tabRevoke.Controls.Add(lblPermissions);

            clbRevokePermissions = new CheckedListBox();
            clbRevokePermissions.Location = new Point(130, 160);
            clbRevokePermissions.Size = new Size(200, 120);
            clbRevokePermissions.Items.AddRange(new object[] {
                "SELECT", "INSERT", "UPDATE", "DELETE",
                "REFERENCES", "DROP", "ALTER", "ALL"
            });
            tabRevoke.Controls.Add(clbRevokePermissions);

            btnRevoke = new Button();
            btnRevoke.Text = "REVOKE Permission";
            btnRevoke.Location = new Point(130, 290);
            btnRevoke.Size = new Size(200, 35);
            btnRevoke.BackColor = Color.FromArgb(244, 67, 54);
            btnRevoke.ForeColor = Color.White;
            btnRevoke.FlatStyle = FlatStyle.Flat;
            btnRevoke.Click += BtnRevoke_Click;
            tabRevoke.Controls.Add(btnRevoke);

            Label lblResult = new Label();
            lblResult.Text = "Result:";
            lblResult.Location = new Point(380, 20);
            lblResult.Size = new Size(100, 20);
            tabRevoke.Controls.Add(lblResult);

            txtRevokeResult = new TextBox();
            txtRevokeResult.Location = new Point(380, 45);
            txtRevokeResult.Size = new Size(450, 310);
            txtRevokeResult.Multiline = true;
            txtRevokeResult.ScrollBars = ScrollBars.Vertical;
            txtRevokeResult.ReadOnly = true;
            txtRevokeResult.Font = new Font("Consolas", 9);
            tabRevoke.Controls.Add(txtRevokeResult);

            tabControl.TabPages.Add(tabRevoke);
        }

        private void CreateUserTab()
        {
            tabUsers = new TabPage("User Management");

            // Create User Group
            grpCreateUser = new GroupBox();
            grpCreateUser.Text = "Create User";
            grpCreateUser.Location = new Point(15, 15);
            grpCreateUser.Size = new Size(400, 150);

            Label lblUsername = new Label();
            lblUsername.Text = "Username:";
            lblUsername.Location = new Point(15, 25);
            lblUsername.Size = new Size(100, 20);
            grpCreateUser.Controls.Add(lblUsername);

            txtNewUsername = new TextBox();
            txtNewUsername.Location = new Point(120, 23);
            txtNewUsername.Size = new Size(250, 25);
            grpCreateUser.Controls.Add(txtNewUsername);

            Label lblPassword = new Label();
            lblPassword.Text = "Password:";
            lblPassword.Location = new Point(15, 55);
            lblPassword.Size = new Size(100, 20);
            grpCreateUser.Controls.Add(lblPassword);

            txtNewPassword = new TextBox();
            txtNewPassword.Location = new Point(120, 53);
            txtNewPassword.Size = new Size(250, 25);
            txtNewPassword.PasswordChar = '*';
            grpCreateUser.Controls.Add(txtNewPassword);

            Label lblConnLimit = new Label();
            lblConnLimit.Text = "Conn Limit:";
            lblConnLimit.Location = new Point(15, 85);
            lblConnLimit.Size = new Size(100, 20);
            grpCreateUser.Controls.Add(lblConnLimit);

            numConnectionLimit = new NumericUpDown();
            numConnectionLimit.Location = new Point(120, 83);
            numConnectionLimit.Size = new Size(80, 25);
            numConnectionLimit.Minimum = -1;
            numConnectionLimit.Maximum = 1000;
            numConnectionLimit.Value = -1;
            grpCreateUser.Controls.Add(numConnectionLimit);

            chkValidUntilEnabled = new CheckBox();
            chkValidUntilEnabled.Text = "Valid Until:";
            chkValidUntilEnabled.Location = new Point(210, 85);
            chkValidUntilEnabled.Size = new Size(90, 20);
            grpCreateUser.Controls.Add(chkValidUntilEnabled);

            dtpValidUntil = new DateTimePicker();
            dtpValidUntil.Location = new Point(300, 83);
            dtpValidUntil.Size = new Size(90, 25);
            dtpValidUntil.Format = DateTimePickerFormat.Short;
            dtpValidUntil.Enabled = false;
            grpCreateUser.Controls.Add(dtpValidUntil);

            chkValidUntilEnabled.CheckedChanged += (s, e) =>
                dtpValidUntil.Enabled = chkValidUntilEnabled.Checked;

            btnCreateUser = new Button();
            btnCreateUser.Text = "Create User";
            btnCreateUser.Location = new Point(120, 115);
            btnCreateUser.Size = new Size(120, 25);
            btnCreateUser.Click += BtnCreateUser_Click;
            grpCreateUser.Controls.Add(btnCreateUser);

            tabUsers.Controls.Add(grpCreateUser);

            // Alter User Group
            grpAlterUser = new GroupBox();
            grpAlterUser.Text = "Alter User Password";
            grpAlterUser.Location = new Point(15, 175);
            grpAlterUser.Size = new Size(400, 100);

            Label lblAlterUser = new Label();
            lblAlterUser.Text = "Username:";
            lblAlterUser.Location = new Point(15, 25);
            lblAlterUser.Size = new Size(100, 20);
            grpAlterUser.Controls.Add(lblAlterUser);

            txtAlterUsername = new TextBox();
            txtAlterUsername.Location = new Point(120, 23);
            txtAlterUsername.Size = new Size(250, 25);
            grpAlterUser.Controls.Add(txtAlterUsername);

            Label lblNewPassword = new Label();
            lblNewPassword.Text = "New Password:";
            lblNewPassword.Location = new Point(15, 55);
            lblNewPassword.Size = new Size(100, 20);
            grpAlterUser.Controls.Add(lblNewPassword);

            txtAlterPassword = new TextBox();
            txtAlterPassword.Location = new Point(120, 53);
            txtAlterPassword.Size = new Size(250, 25);
            txtAlterPassword.PasswordChar = '*';
            grpAlterUser.Controls.Add(txtAlterPassword);

            btnAlterPassword = new Button();
            btnAlterPassword.Text = "Change Password";
            btnAlterPassword.Location = new Point(250, 68);
            btnAlterPassword.Size = new Size(120, 25);
            btnAlterPassword.Click += BtnAlterPassword_Click;
            grpAlterUser.Controls.Add(btnAlterPassword);

            tabUsers.Controls.Add(grpAlterUser);

            // Drop User Group
            grpDropUser = new GroupBox();
            grpDropUser.Text = "Drop User";
            grpDropUser.Location = new Point(15, 285);
            grpDropUser.Size = new Size(400, 80);

            Label lblDropUser = new Label();
            lblDropUser.Text = "Username:";
            lblDropUser.Location = new Point(15, 25);
            lblDropUser.Size = new Size(100, 20);
            grpDropUser.Controls.Add(lblDropUser);

            txtDropUsername = new TextBox();
            txtDropUsername.Location = new Point(120, 23);
            txtDropUsername.Size = new Size(250, 25);
            grpDropUser.Controls.Add(txtDropUsername);

            btnDropUser = new Button();
            btnDropUser.Text = "Drop User";
            btnDropUser.Location = new Point(250, 48);
            btnDropUser.Size = new Size(120, 25);
            btnDropUser.BackColor = Color.FromArgb(244, 67, 54);
            btnDropUser.ForeColor = Color.White;
            btnDropUser.Click += BtnDropUser_Click;
            grpDropUser.Controls.Add(btnDropUser);

            tabUsers.Controls.Add(grpDropUser);

            // Result
            Label lblUserResult = new Label();
            lblUserResult.Text = "Result:";
            lblUserResult.Location = new Point(430, 15);
            lblUserResult.Size = new Size(100, 20);
            tabUsers.Controls.Add(lblUserResult);

            txtUserResult = new TextBox();
            txtUserResult.Location = new Point(430, 40);
            txtUserResult.Size = new Size(410, 325);
            txtUserResult.Multiline = true;
            txtUserResult.ScrollBars = ScrollBars.Vertical;
            txtUserResult.ReadOnly = true;
            txtUserResult.Font = new Font("Consolas", 9);
            tabUsers.Controls.Add(txtUserResult);

            tabControl.TabPages.Add(tabUsers);
        }

        private void CreateGroupTab()
        {
            tabGroups = new TabPage("Group Management");

            // Create Group
            grpCreateGroup = new GroupBox();
            grpCreateGroup.Text = "Create Group";
            grpCreateGroup.Location = new Point(15, 15);
            grpCreateGroup.Size = new Size(400, 80);

            Label lblGroupName = new Label();
            lblGroupName.Text = "Group Name:";
            lblGroupName.Location = new Point(15, 25);
            lblGroupName.Size = new Size(100, 20);
            grpCreateGroup.Controls.Add(lblGroupName);

            txtNewGroupName = new TextBox();
            txtNewGroupName.Location = new Point(120, 23);
            txtNewGroupName.Size = new Size(250, 25);
            grpCreateGroup.Controls.Add(txtNewGroupName);

            btnCreateGroup = new Button();
            btnCreateGroup.Text = "Create Group";
            btnCreateGroup.Location = new Point(250, 48);
            btnCreateGroup.Size = new Size(120, 25);
            btnCreateGroup.Click += BtnCreateGroup_Click;
            grpCreateGroup.Controls.Add(btnCreateGroup);

            tabGroups.Controls.Add(grpCreateGroup);

            // Manage Members
            grpManageMembers = new GroupBox();
            grpManageMembers.Text = "Manage Group Members";
            grpManageMembers.Location = new Point(15, 105);
            grpManageMembers.Size = new Size(400, 150);

            Label lblMemberGroup = new Label();
            lblMemberGroup.Text = "Group Name:";
            lblMemberGroup.Location = new Point(15, 25);
            lblMemberGroup.Size = new Size(100, 20);
            grpManageMembers.Controls.Add(lblMemberGroup);

            txtMemberGroupName = new TextBox();
            txtMemberGroupName.Location = new Point(120, 23);
            txtMemberGroupName.Size = new Size(250, 25);
            grpManageMembers.Controls.Add(txtMemberGroupName);

            Label lblMemberUser = new Label();
            lblMemberUser.Text = "Username:";
            lblMemberUser.Location = new Point(15, 55);
            lblMemberUser.Size = new Size(100, 20);
            grpManageMembers.Controls.Add(lblMemberUser);

            txtMemberUsername = new TextBox();
            txtMemberUsername.Location = new Point(120, 53);
            txtMemberUsername.Size = new Size(250, 25);
            grpManageMembers.Controls.Add(txtMemberUsername);

            btnAddMember = new Button();
            btnAddMember.Text = "Add Member";
            btnAddMember.Location = new Point(120, 90);
            btnAddMember.Size = new Size(120, 30);
            btnAddMember.BackColor = Color.FromArgb(76, 175, 80);
            btnAddMember.ForeColor = Color.White;
            btnAddMember.Click += BtnAddMember_Click;
            grpManageMembers.Controls.Add(btnAddMember);

            btnRemoveMember = new Button();
            btnRemoveMember.Text = "Remove Member";
            btnRemoveMember.Location = new Point(250, 90);
            btnRemoveMember.Size = new Size(120, 30);
            btnRemoveMember.BackColor = Color.FromArgb(255, 152, 0);
            btnRemoveMember.ForeColor = Color.White;
            btnRemoveMember.Click += BtnRemoveMember_Click;
            grpManageMembers.Controls.Add(btnRemoveMember);

            tabGroups.Controls.Add(grpManageMembers);

            // Drop Group
            grpDropGroup = new GroupBox();
            grpDropGroup.Text = "Drop Group";
            grpDropGroup.Location = new Point(15, 265);
            grpDropGroup.Size = new Size(400, 80);

            Label lblDropGroup = new Label();
            lblDropGroup.Text = "Group Name:";
            lblDropGroup.Location = new Point(15, 25);
            lblDropGroup.Size = new Size(100, 20);
            grpDropGroup.Controls.Add(lblDropGroup);

            txtDropGroupName = new TextBox();
            txtDropGroupName.Location = new Point(120, 23);
            txtDropGroupName.Size = new Size(250, 25);
            grpDropGroup.Controls.Add(txtDropGroupName);

            btnDropGroup = new Button();
            btnDropGroup.Text = "Drop Group";
            btnDropGroup.Location = new Point(250, 48);
            btnDropGroup.Size = new Size(120, 25);
            btnDropGroup.BackColor = Color.FromArgb(244, 67, 54);
            btnDropGroup.ForeColor = Color.White;
            btnDropGroup.Click += BtnDropGroup_Click;
            grpDropGroup.Controls.Add(btnDropGroup);

            tabGroups.Controls.Add(grpDropGroup);

            // Result
            Label lblGroupResult = new Label();
            lblGroupResult.Text = "Result:";
            lblGroupResult.Location = new Point(430, 15);
            lblGroupResult.Size = new Size(100, 20);
            tabGroups.Controls.Add(lblGroupResult);

            txtGroupResult = new TextBox();
            txtGroupResult.Location = new Point(430, 40);
            txtGroupResult.Size = new Size(410, 305);
            txtGroupResult.Multiline = true;
            txtGroupResult.ScrollBars = ScrollBars.Vertical;
            txtGroupResult.ReadOnly = true;
            txtGroupResult.Font = new Font("Consolas", 9);
            tabGroups.Controls.Add(txtGroupResult);

            tabControl.TabPages.Add(tabGroups);
        }

        private void InitializeServices()
        {
            lblCluster.Text = "Cluster: " + _cluster.Name + " (" + _cluster.Host + ")";

            try
            {
                _odbcService = new RedshiftOdbcService();
                _permService = new PermissionManagementService(_odbcService);
                _permService.ReadOnlyMode = chkReadOnlyMode.Checked;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Failed to initialize services:\n\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void ChkReadOnlyMode_CheckedChanged(object sender, EventArgs e)
        {
            if (_permService != null)
            {
                _permService.ReadOnlyMode = chkReadOnlyMode.Checked;
            }

            if (chkReadOnlyMode.Checked)
            {
                chkReadOnlyMode.BackColor = Color.FromArgb(76, 175, 80);
                chkReadOnlyMode.ForeColor = Color.White;
            }
            else
            {
                chkReadOnlyMode.BackColor = Color.FromArgb(244, 67, 54);
                chkReadOnlyMode.ForeColor = Color.White;

                DialogResult confirm = MessageBox.Show(
                    "WARNING: You are disabling Read-Only Mode.\n\n" +
                    "Changes will be executed on the Redshift cluster!\n\n" +
                    "Are you sure you want to proceed?",
                    "Disable Read-Only Mode",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (confirm != DialogResult.Yes)
                {
                    chkReadOnlyMode.Checked = true;
                }
            }
        }

        private void CmbGrantType_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtGrantTable.Enabled = (cmbGrantType.SelectedIndex == 0); // Table only
        }

        private void CmbRevokeType_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtRevokeTable.Enabled = (cmbRevokeType.SelectedIndex == 0); // Table only
        }

        private void BtnGrant_Click(object sender, EventArgs e)
        {
            try
            {
                if (_odbcService == null || _permService == null)
                {
                    MessageBox.Show("Services not initialized", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Validation
                if (string.IsNullOrWhiteSpace(txtGrantSchema.Text))
                {
                    MessageBox.Show("Schema is required", "Validation",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtGrantUser.Text))
                {
                    MessageBox.Show("User/Group is required", "Validation",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (clbGrantPermissions.CheckedItems.Count == 0)
                {
                    MessageBox.Show("Select at least one permission", "Validation",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Get selected permissions
                List<string> permissions = new List<string>();
                foreach (object item in clbGrantPermissions.CheckedItems)
                {
                    permissions.Add(item.ToString());
                }

                // Connect
                _odbcService.Connect(_cluster);

                PermissionOperationResult result = null;

                switch (cmbGrantType.SelectedIndex)
                {
                    case 0: // Table
                        if (string.IsNullOrWhiteSpace(txtGrantTable.Text))
                        {
                            MessageBox.Show("Table is required", "Validation",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        result = _permService.GrantTablePermission(
                            txtGrantSchema.Text.Trim(),
                            txtGrantTable.Text.Trim(),
                            txtGrantUser.Text.Trim(),
                            permissions.ToArray(),
                            chkWithGrantOption.Checked);
                        break;

                    case 1: // Schema
                        result = _permService.GrantSchemaPermission(
                            txtGrantSchema.Text.Trim(),
                            txtGrantUser.Text.Trim(),
                            permissions.ToArray());
                        break;

                    case 2: // All Tables in Schema
                        result = _permService.GrantAllTablesInSchema(
                            txtGrantSchema.Text.Trim(),
                            txtGrantUser.Text.Trim(),
                            permissions.ToArray());
                        break;
                }

                DisplayResult(result, txtGrantResult);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (_odbcService != null)
                    _odbcService.Disconnect();
            }
        }

        private void BtnRevoke_Click(object sender, EventArgs e)
        {
            try
            {
                if (_odbcService == null || _permService == null)
                {
                    MessageBox.Show("Services not initialized", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Validation
                if (string.IsNullOrWhiteSpace(txtRevokeUser.Text))
                {
                    MessageBox.Show("User/Group is required", "Validation",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Connect
                _odbcService.Connect(_cluster);

                PermissionOperationResult result = null;

                switch (cmbRevokeType.SelectedIndex)
                {
                    case 0: // Table
                        if (string.IsNullOrWhiteSpace(txtRevokeSchema.Text) ||
                            string.IsNullOrWhiteSpace(txtRevokeTable.Text))
                        {
                            MessageBox.Show("Schema and Table are required", "Validation",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        if (clbRevokePermissions.CheckedItems.Count == 0)
                        {
                            MessageBox.Show("Select at least one permission", "Validation",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        List<string> permissions = new List<string>();
                        foreach (object item in clbRevokePermissions.CheckedItems)
                        {
                            permissions.Add(item.ToString());
                        }

                        result = _permService.RevokeTablePermission(
                            txtRevokeSchema.Text.Trim(),
                            txtRevokeTable.Text.Trim(),
                            txtRevokeUser.Text.Trim(),
                            permissions.ToArray());
                        break;

                    case 1: // Schema
                        if (string.IsNullOrWhiteSpace(txtRevokeSchema.Text))
                        {
                            MessageBox.Show("Schema is required", "Validation",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        result = _permService.RevokeSchemaPermission(
                            txtRevokeSchema.Text.Trim(),
                            txtRevokeUser.Text.Trim());
                        break;

                    case 2: // All Permissions
                        result = _permService.RevokeAllPermissions(
                            txtRevokeUser.Text.Trim());
                        break;
                }

                DisplayResult(result, txtRevokeResult);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (_odbcService != null)
                    _odbcService.Disconnect();
            }
        }

        private void BtnCreateUser_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtNewUsername.Text))
                {
                    MessageBox.Show("Username is required", "Validation",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtNewPassword.Text))
                {
                    MessageBox.Show("Password is required", "Validation",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                _odbcService.Connect(_cluster);

                DateTime? validUntil = null;
                if (chkValidUntilEnabled.Checked)
                {
                    validUntil = dtpValidUntil.Value;
                }

                PermissionOperationResult result = _permService.CreateUser(
                    txtNewUsername.Text.Trim(),
                    txtNewPassword.Text,
                    (int)numConnectionLimit.Value,
                    validUntil);

                DisplayResult(result, txtUserResult);

                if (result.Success)
                {
                    txtNewUsername.Clear();
                    txtNewPassword.Clear();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (_odbcService != null)
                    _odbcService.Disconnect();
            }
        }

        private void BtnAlterPassword_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtAlterUsername.Text))
                {
                    MessageBox.Show("Username is required", "Validation",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtAlterPassword.Text))
                {
                    MessageBox.Show("Password is required", "Validation",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                _odbcService.Connect(_cluster);

                PermissionOperationResult result = _permService.AlterUserPassword(
                    txtAlterUsername.Text.Trim(),
                    txtAlterPassword.Text);

                DisplayResult(result, txtUserResult);

                if (result.Success)
                {
                    txtAlterPassword.Clear();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (_odbcService != null)
                    _odbcService.Disconnect();
            }
        }

        private void BtnDropUser_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtDropUsername.Text))
                {
                    MessageBox.Show("Username is required", "Validation",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DialogResult confirm = MessageBox.Show(
                    "Are you sure you want to DROP user: " + txtDropUsername.Text + "?\n\n" +
                    "This action cannot be undone!",
                    "Confirm Drop User",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (confirm != DialogResult.Yes)
                    return;

                _odbcService.Connect(_cluster);

                PermissionOperationResult result = _permService.DropUser(
                    txtDropUsername.Text.Trim());

                DisplayResult(result, txtUserResult);

                if (result.Success)
                {
                    txtDropUsername.Clear();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (_odbcService != null)
                    _odbcService.Disconnect();
            }
        }

        private void BtnCreateGroup_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtNewGroupName.Text))
                {
                    MessageBox.Show("Group name is required", "Validation",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                _odbcService.Connect(_cluster);

                PermissionOperationResult result = _permService.CreateGroup(
                    txtNewGroupName.Text.Trim());

                DisplayResult(result, txtGroupResult);

                if (result.Success)
                {
                    txtNewGroupName.Clear();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (_odbcService != null)
                    _odbcService.Disconnect();
            }
        }

        private void BtnAddMember_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtMemberGroupName.Text))
                {
                    MessageBox.Show("Group name is required", "Validation",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtMemberUsername.Text))
                {
                    MessageBox.Show("Username is required", "Validation",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                _odbcService.Connect(_cluster);

                PermissionOperationResult result = _permService.AddUserToGroup(
                    txtMemberUsername.Text.Trim(),
                    txtMemberGroupName.Text.Trim());

                DisplayResult(result, txtGroupResult);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (_odbcService != null)
                    _odbcService.Disconnect();
            }
        }

        private void BtnRemoveMember_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtMemberGroupName.Text))
                {
                    MessageBox.Show("Group name is required", "Validation",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtMemberUsername.Text))
                {
                    MessageBox.Show("Username is required", "Validation",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                _odbcService.Connect(_cluster);

                PermissionOperationResult result = _permService.RemoveUserFromGroup(
                    txtMemberUsername.Text.Trim(),
                    txtMemberGroupName.Text.Trim());

                DisplayResult(result, txtGroupResult);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (_odbcService != null)
                    _odbcService.Disconnect();
            }
        }

        private void BtnDropGroup_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtDropGroupName.Text))
                {
                    MessageBox.Show("Group name is required", "Validation",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DialogResult confirm = MessageBox.Show(
                    "Are you sure you want to DROP group: " + txtDropGroupName.Text + "?\n\n" +
                    "This action cannot be undone!",
                    "Confirm Drop Group",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (confirm != DialogResult.Yes)
                    return;

                _odbcService.Connect(_cluster);

                PermissionOperationResult result = _permService.DropGroup(
                    txtDropGroupName.Text.Trim());

                DisplayResult(result, txtGroupResult);

                if (result.Success)
                {
                    txtDropGroupName.Clear();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (_odbcService != null)
                    _odbcService.Disconnect();
            }
        }

        private void DisplayResult(PermissionOperationResult result, TextBox outputBox)
        {
            if (result == null)
            {
                outputBox.Text = "No result returned";
                return;
            }

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine("=".PadRight(50, '='));
            sb.AppendLine("STATUS: " + (result.Success ? "SUCCESS" : "FAILED"));
            sb.AppendLine("=".PadRight(50, '='));
            sb.AppendLine();

            if (result.ReadOnlyMode)
            {
                sb.AppendLine("READ-ONLY MODE: Command was simulated, NOT executed");
                sb.AppendLine();
            }

            sb.AppendLine("MESSAGE:");
            sb.AppendLine(result.Message);
            sb.AppendLine();

            sb.AppendLine("SQL EXECUTED:");
            sb.AppendLine(result.SqlExecuted);
            sb.AppendLine();

            if (result.Error != null)
            {
                sb.AppendLine("ERROR DETAILS:");
                sb.AppendLine(result.Error.Message);
                if (result.Error.InnerException != null)
                {
                    sb.AppendLine();
                    sb.AppendLine("INNER ERROR:");
                    sb.AppendLine(result.Error.InnerException.Message);
                }
            }

            sb.AppendLine();
            sb.AppendLine("TIME: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            sb.AppendLine("=".PadRight(50, '='));

            outputBox.Text = sb.ToString();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_permService != null)
                {
                    _permService.Dispose();
                    _permService = null;
                }

                if (_odbcService != null)
                {
                    _odbcService.Dispose();
                    _odbcService = null;
                }
            }

            base.Dispose(disposing);
        }
    }
}

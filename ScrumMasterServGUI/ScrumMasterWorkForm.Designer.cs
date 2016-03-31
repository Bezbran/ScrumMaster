namespace ScrumMasterServGUI
{
    partial class ScrumMasterWorkForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
                if(host!=null) host.Close();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.scrumNameTB = new System.Windows.Forms.TextBox();
            this.OpenBtn = new System.Windows.Forms.Button();
            this.urlTB = new System.Windows.Forms.TextBox();
            this.urlLabel = new System.Windows.Forms.Label();
            this.stpBtn = new System.Windows.Forms.Button();
            this.fileNameTB = new System.Windows.Forms.TextBox();
            this.ipCB = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.teamManagerNameTB = new System.Windows.Forms.TextBox();
            this.teamManagerPasswordTB = new System.Windows.Forms.TextBox();
            this.createTeamBTN = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // scrumNameTB
            // 
            this.scrumNameTB.Location = new System.Drawing.Point(55, 34);
            this.scrumNameTB.Name = "scrumNameTB";
            this.scrumNameTB.Size = new System.Drawing.Size(174, 20);
            this.scrumNameTB.TabIndex = 0;
            this.scrumNameTB.Text = "Choose a name to the scrum team";
            this.scrumNameTB.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox1_KeyDown);
            // 
            // OpenBtn
            // 
            this.OpenBtn.AutoSize = true;
            this.OpenBtn.Location = new System.Drawing.Point(234, 108);
            this.OpenBtn.Name = "OpenBtn";
            this.OpenBtn.Size = new System.Drawing.Size(174, 23);
            this.OpenBtn.TabIndex = 1;
            this.OpenBtn.Text = "OR open an existing team";
            this.OpenBtn.UseVisualStyleBackColor = true;
            this.OpenBtn.Click += new System.EventHandler(this.OpenBtn_Click);
            // 
            // urlTB
            // 
            this.urlTB.Location = new System.Drawing.Point(234, 227);
            this.urlTB.Name = "urlTB";
            this.urlTB.Size = new System.Drawing.Size(174, 20);
            this.urlTB.TabIndex = 2;
            this.urlTB.Text = "http://localhost:10002/ScrumMaster";
            // 
            // urlLabel
            // 
            this.urlLabel.AutoSize = true;
            this.urlLabel.Location = new System.Drawing.Point(234, 208);
            this.urlLabel.Name = "urlLabel";
            this.urlLabel.Size = new System.Drawing.Size(60, 13);
            this.urlLabel.TabIndex = 3;
            this.urlLabel.Text = "Service url:";
            // 
            // stpBtn
            // 
            this.stpBtn.Location = new System.Drawing.Point(234, 182);
            this.stpBtn.Name = "stpBtn";
            this.stpBtn.Size = new System.Drawing.Size(174, 23);
            this.stpBtn.TabIndex = 4;
            this.stpBtn.Text = "Stop without saving";
            this.stpBtn.UseVisualStyleBackColor = true;
            this.stpBtn.Visible = false;
            this.stpBtn.Click += new System.EventHandler(this.stpBtn_Click);
            // 
            // fileNameTB
            // 
            this.fileNameTB.Location = new System.Drawing.Point(234, 138);
            this.fileNameTB.Name = "fileNameTB";
            this.fileNameTB.Size = new System.Drawing.Size(174, 20);
            this.fileNameTB.TabIndex = 5;
            this.fileNameTB.Visible = false;
            // 
            // ipCB
            // 
            this.ipCB.FormattingEnabled = true;
            this.ipCB.Location = new System.Drawing.Point(234, 271);
            this.ipCB.Name = "ipCB";
            this.ipCB.Size = new System.Drawing.Size(174, 21);
            this.ipCB.TabIndex = 6;
            this.ipCB.SelectedIndexChanged += new System.EventHandler(this.ipCB_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(234, 254);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(127, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "All localhost IP adrresses:";
            // 
            // teamManagerNameTB
            // 
            this.teamManagerNameTB.Location = new System.Drawing.Point(238, 34);
            this.teamManagerNameTB.Name = "teamManagerNameTB";
            this.teamManagerNameTB.Size = new System.Drawing.Size(174, 20);
            this.teamManagerNameTB.TabIndex = 8;
            this.teamManagerNameTB.Text = "Team manager name";
            // 
            // teamManagerPasswordTB
            // 
            this.teamManagerPasswordTB.Location = new System.Drawing.Point(418, 34);
            this.teamManagerPasswordTB.Name = "teamManagerPasswordTB";
            this.teamManagerPasswordTB.Size = new System.Drawing.Size(174, 20);
            this.teamManagerPasswordTB.TabIndex = 9;
            this.teamManagerPasswordTB.UseSystemPasswordChar = true;
            // 
            // createTeamBTN
            // 
            this.createTeamBTN.AutoSize = true;
            this.createTeamBTN.Location = new System.Drawing.Point(234, 60);
            this.createTeamBTN.Name = "createTeamBTN";
            this.createTeamBTN.Size = new System.Drawing.Size(174, 23);
            this.createTeamBTN.TabIndex = 10;
            this.createTeamBTN.Text = "Create team";
            this.createTeamBTN.UseVisualStyleBackColor = true;
            this.createTeamBTN.Click += new System.EventHandler(this.createTeamBTN_Click);
            // 
            // ScrumMasterWorkForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(611, 321);
            this.Controls.Add(this.createTeamBTN);
            this.Controls.Add(this.teamManagerPasswordTB);
            this.Controls.Add(this.teamManagerNameTB);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ipCB);
            this.Controls.Add(this.fileNameTB);
            this.Controls.Add(this.stpBtn);
            this.Controls.Add(this.urlLabel);
            this.Controls.Add(this.urlTB);
            this.Controls.Add(this.OpenBtn);
            this.Controls.Add(this.scrumNameTB);
            this.Name = "ScrumMasterWorkForm";
            this.Text = "ScrumMasterWorkForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox scrumNameTB;
        private System.Windows.Forms.Button OpenBtn;
        private System.Windows.Forms.TextBox urlTB;
        private System.Windows.Forms.Label urlLabel;
        private System.Windows.Forms.Button stpBtn;
        private System.Windows.Forms.TextBox fileNameTB;
        private System.Windows.Forms.ComboBox ipCB;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox teamManagerNameTB;
        private System.Windows.Forms.TextBox teamManagerPasswordTB;
        private System.Windows.Forms.Button createTeamBTN;
    }
}
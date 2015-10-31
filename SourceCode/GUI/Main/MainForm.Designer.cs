namespace AutoUSBBackup.GUI
{
	partial class MainForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose( bool disposing )
		{
			if ( disposing && ( components != null ) )
			{
				components.Dispose();
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager( typeof( MainForm ) );
			this.mTrayIcon = new System.Windows.Forms.NotifyIcon( this.components );
			this.mTrayIconMenu = new System.Windows.Forms.ContextMenuStrip( this.components );
			this.showToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.mStatusStrip = new System.Windows.Forms.StatusStrip();
			this.mProgressBar = new System.Windows.Forms.ToolStripProgressBar();
			this.mStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
			this.mUserControlSwitcher = new AutoUSBBackup.GUI.UserControlSwitcher();
			this.mTrayIconMenu.SuspendLayout();
			this.mStatusStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// mTrayIcon
			// 
			this.mTrayIcon.ContextMenuStrip = this.mTrayIconMenu;
			this.mTrayIcon.Icon = ( ( System.Drawing.Icon )( resources.GetObject( "mTrayIcon.Icon" ) ) );
			this.mTrayIcon.Text = "AutoBackup";
			this.mTrayIcon.Visible = true;
			this.mTrayIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler( this.mTrayIcon_DoubleClick );
			// 
			// mTrayIconMenu
			// 
			this.mTrayIconMenu.Items.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.showToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem} );
			this.mTrayIconMenu.Name = "mTrayIconMenu";
			this.mTrayIconMenu.Size = new System.Drawing.Size( 104, 54 );
			// 
			// showToolStripMenuItem
			// 
			this.showToolStripMenuItem.Name = "showToolStripMenuItem";
			this.showToolStripMenuItem.Size = new System.Drawing.Size( 103, 22 );
			this.showToolStripMenuItem.Text = "&Show";
			this.showToolStripMenuItem.Click += new System.EventHandler( this.showToolStripMenuItem_Click );
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size( 100, 6 );
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			this.exitToolStripMenuItem.Size = new System.Drawing.Size( 103, 22 );
			this.exitToolStripMenuItem.Text = "E&xit";
			this.exitToolStripMenuItem.Click += new System.EventHandler( this.exitToolStripMenuItem_Click );
			// 
			// mStatusStrip
			// 
			this.mStatusStrip.Items.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.mProgressBar,
            this.mStatusLabel} );
			this.mStatusStrip.Location = new System.Drawing.Point( 0, 370 );
			this.mStatusStrip.Name = "mStatusStrip";
			this.mStatusStrip.Size = new System.Drawing.Size( 907, 22 );
			this.mStatusStrip.TabIndex = 1;
			this.mStatusStrip.Text = "Ready";
			// 
			// mProgressBar
			// 
			this.mProgressBar.AutoSize = false;
			this.mProgressBar.Margin = new System.Windows.Forms.Padding( 3 );
			this.mProgressBar.Name = "mProgressBar";
			this.mProgressBar.Size = new System.Drawing.Size( 300, 16 );
			// 
			// mStatusLabel
			// 
			this.mStatusLabel.Margin = new System.Windows.Forms.Padding( 3 );
			this.mStatusLabel.Name = "mStatusLabel";
			this.mStatusLabel.Size = new System.Drawing.Size( 39, 16 );
			this.mStatusLabel.Text = "Ready";
			this.mStatusLabel.TextChanged += new System.EventHandler( this.mStatusLabel_TextChanged );
			// 
			// mUserControlSwitcher
			// 
			this.mUserControlSwitcher.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mUserControlSwitcher.Location = new System.Drawing.Point( 0, 0 );
			this.mUserControlSwitcher.Name = "mUserControlSwitcher";
			this.mUserControlSwitcher.Size = new System.Drawing.Size( 907, 370 );
			this.mUserControlSwitcher.TabIndex = 2;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size( 907, 392 );
			this.Controls.Add( this.mUserControlSwitcher );
			this.Controls.Add( this.mStatusStrip );
			this.MinimumSize = new System.Drawing.Size( 345, 266 );
			this.Name = "MainForm";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "AutoBackup";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler( this.MainForm_FormClosing );
			this.Resize += new System.EventHandler( this.MainForm_Resize );
			this.mTrayIconMenu.ResumeLayout( false );
			this.mStatusStrip.ResumeLayout( false );
			this.mStatusStrip.PerformLayout();
			this.ResumeLayout( false );
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.NotifyIcon mTrayIcon;
		private System.Windows.Forms.ContextMenuStrip mTrayIconMenu;
		private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem showToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.StatusStrip mStatusStrip;
		private UserControlSwitcher mUserControlSwitcher;
		private System.Windows.Forms.ToolStripStatusLabel mStatusLabel;
		private System.Windows.Forms.ToolStripProgressBar mProgressBar;

	}
}
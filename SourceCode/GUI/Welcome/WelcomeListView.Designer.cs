namespace AutoUSBBackup.GUI.Shared
{
	partial class WelcomeListView
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
				if ( mIcon != null )
					mIcon.Dispose();
				if ( mFolderIcon != null )
					mFolderIcon.Dispose();
				components.Dispose();
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.mVolumeContextMenu = new System.Windows.Forms.ContextMenuStrip( this.components );
			this.restoreVolumeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.addNewVolumeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.removeVolumeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.mSpecificOperationsGroupBox = new System.Windows.Forms.GroupBox();
			this.mRemoveVolumeButton = new System.Windows.Forms.Button();
			this.mRestoreVolumeButton = new System.Windows.Forms.Button();
			this.mCommonOperationsGroupBox = new System.Windows.Forms.GroupBox();
			this.mBackupAllNowButton = new System.Windows.Forms.Button();
			this.mButtonOptions = new System.Windows.Forms.Button();
			this.mAddVolumeButton = new System.Windows.Forms.Button();
			this.mSplitContainer = new System.Windows.Forms.SplitContainer();
			this.mVolumeListView = new BrightIdeasSoftware.ObjectListView();
			this.mColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.mLogTextBox = new AutoUSBBackup.GUI.Shared.LogTextBox();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.openLocationInExplorer = new System.Windows.Forms.ToolStripMenuItem();
			this.openBackupLocationInExplorer = new System.Windows.Forms.ToolStripMenuItem();
			this.mVolumeContextMenu.SuspendLayout();
			this.mSpecificOperationsGroupBox.SuspendLayout();
			this.mCommonOperationsGroupBox.SuspendLayout();
			this.mSplitContainer.Panel1.SuspendLayout();
			this.mSplitContainer.Panel2.SuspendLayout();
			this.mSplitContainer.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.mVolumeListView)).BeginInit();
			this.SuspendLayout();
			// 
			// mVolumeContextMenu
			// 
			this.mVolumeContextMenu.Items.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.restoreVolumeToolStripMenuItem,
            this.toolStripSeparator1,
            this.addNewVolumeToolStripMenuItem,
            this.removeVolumeToolStripMenuItem,
            this.toolStripSeparator2,
            this.openLocationInExplorer,
            this.openBackupLocationInExplorer} );
			this.mVolumeContextMenu.Name = "mVolumeContextMenu";
			this.mVolumeContextMenu.Size = new System.Drawing.Size( 250, 148 );
			this.mVolumeContextMenu.Opening += new System.ComponentModel.CancelEventHandler( this.mVolumeContextMenu_Opening );
			// 
			// restoreVolumeToolStripMenuItem
			// 
			this.restoreVolumeToolStripMenuItem.Name = "restoreVolumeToolStripMenuItem";
			this.restoreVolumeToolStripMenuItem.Size = new System.Drawing.Size( 249, 22 );
			this.restoreVolumeToolStripMenuItem.Text = "R&estore volume...";
			this.restoreVolumeToolStripMenuItem.Click += new System.EventHandler( this.restoreVolumeToolStripMenuItem_Click );
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size( 246, 6 );
			// 
			// addNewVolumeToolStripMenuItem
			// 
			this.addNewVolumeToolStripMenuItem.Name = "addNewVolumeToolStripMenuItem";
			this.addNewVolumeToolStripMenuItem.Size = new System.Drawing.Size( 249, 22 );
			this.addNewVolumeToolStripMenuItem.Text = "&Add new volume...";
			this.addNewVolumeToolStripMenuItem.Click += new System.EventHandler( this.addNewVolumeToolStripMenuItem_Click );
			// 
			// removeVolumeToolStripMenuItem
			// 
			this.removeVolumeToolStripMenuItem.Name = "removeVolumeToolStripMenuItem";
			this.removeVolumeToolStripMenuItem.Size = new System.Drawing.Size( 249, 22 );
			this.removeVolumeToolStripMenuItem.Text = "&Remove volume...";
			this.removeVolumeToolStripMenuItem.Click += new System.EventHandler( this.removeVolumeToolStripMenuItem_Click );
			// 
			// mSpecificOperationsGroupBox
			// 
			this.mSpecificOperationsGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.mSpecificOperationsGroupBox.Controls.Add( this.mRemoveVolumeButton );
			this.mSpecificOperationsGroupBox.Controls.Add( this.mRestoreVolumeButton );
			this.mSpecificOperationsGroupBox.Location = new System.Drawing.Point( 382, 119 );
			this.mSpecificOperationsGroupBox.MinimumSize = new System.Drawing.Size( 102, 105 );
			this.mSpecificOperationsGroupBox.Name = "mSpecificOperationsGroupBox";
			this.mSpecificOperationsGroupBox.Size = new System.Drawing.Size( 116, 219 );
			this.mSpecificOperationsGroupBox.TabIndex = 3;
			this.mSpecificOperationsGroupBox.TabStop = false;
			// 
			// mRemoveVolumeButton
			// 
			this.mRemoveVolumeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.mRemoveVolumeButton.Enabled = false;
			this.mRemoveVolumeButton.Location = new System.Drawing.Point( 6, 190 );
			this.mRemoveVolumeButton.Name = "mRemoveVolumeButton";
			this.mRemoveVolumeButton.Size = new System.Drawing.Size( 104, 23 );
			this.mRemoveVolumeButton.TabIndex = 2;
			this.mRemoveVolumeButton.Text = "Remove";
			this.mRemoveVolumeButton.UseVisualStyleBackColor = true;
			this.mRemoveVolumeButton.Click += new System.EventHandler( this.mRemoveVolumeButton_Click );
			// 
			// mRestoreVolumeButton
			// 
			this.mRestoreVolumeButton.Enabled = false;
			this.mRestoreVolumeButton.Location = new System.Drawing.Point( 6, 19 );
			this.mRestoreVolumeButton.Name = "mRestoreVolumeButton";
			this.mRestoreVolumeButton.Size = new System.Drawing.Size( 104, 23 );
			this.mRestoreVolumeButton.TabIndex = 1;
			this.mRestoreVolumeButton.Text = "Restore...";
			this.mRestoreVolumeButton.UseVisualStyleBackColor = true;
			this.mRestoreVolumeButton.Click += new System.EventHandler( this.mRestoreVolumeButton_Click );
			// 
			// mCommonOperationsGroupBox
			// 
			this.mCommonOperationsGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.mCommonOperationsGroupBox.Controls.Add( this.mBackupAllNowButton );
			this.mCommonOperationsGroupBox.Controls.Add( this.mButtonOptions );
			this.mCommonOperationsGroupBox.Controls.Add( this.mAddVolumeButton );
			this.mCommonOperationsGroupBox.Location = new System.Drawing.Point( 382, 3 );
			this.mCommonOperationsGroupBox.MinimumSize = new System.Drawing.Size( 102, 79 );
			this.mCommonOperationsGroupBox.Name = "mCommonOperationsGroupBox";
			this.mCommonOperationsGroupBox.Size = new System.Drawing.Size( 116, 118 );
			this.mCommonOperationsGroupBox.TabIndex = 4;
			this.mCommonOperationsGroupBox.TabStop = false;
			// 
			// mBackupAllNowButton
			// 
			this.mBackupAllNowButton.Location = new System.Drawing.Point( 6, 77 );
			this.mBackupAllNowButton.Name = "mBackupAllNowButton";
			this.mBackupAllNowButton.Size = new System.Drawing.Size( 104, 23 );
			this.mBackupAllNowButton.TabIndex = 5;
			this.mBackupAllNowButton.Text = "Backup All Now";
			this.mBackupAllNowButton.UseVisualStyleBackColor = true;
			this.mBackupAllNowButton.Click += new System.EventHandler( this.mBackupAllNowButton_Click );
			// 
			// mButtonOptions
			// 
			this.mButtonOptions.Enabled = false;
			this.mButtonOptions.Location = new System.Drawing.Point( 7, 48 );
			this.mButtonOptions.Name = "mButtonOptions";
			this.mButtonOptions.Size = new System.Drawing.Size( 104, 23 );
			this.mButtonOptions.TabIndex = 4;
			this.mButtonOptions.Text = "Options";
			this.mButtonOptions.UseVisualStyleBackColor = true;
			this.mButtonOptions.Click += new System.EventHandler( this.mButtonOptions_Click );
			// 
			// mAddVolumeButton
			// 
			this.mAddVolumeButton.Location = new System.Drawing.Point( 6, 19 );
			this.mAddVolumeButton.Name = "mAddVolumeButton";
			this.mAddVolumeButton.Size = new System.Drawing.Size( 104, 23 );
			this.mAddVolumeButton.TabIndex = 0;
			this.mAddVolumeButton.Text = "Add...";
			this.mAddVolumeButton.UseVisualStyleBackColor = true;
			this.mAddVolumeButton.Click += new System.EventHandler( this.mAddVolumeButton_Click );
			// 
			// mSplitContainer
			// 
			this.mSplitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.mSplitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this.mSplitContainer.Location = new System.Drawing.Point( 0, 3 );
			this.mSplitContainer.Name = "mSplitContainer";
			this.mSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// mSplitContainer.Panel1
			// 
			this.mSplitContainer.Panel1.Controls.Add( this.mVolumeListView );
			// 
			// mSplitContainer.Panel2
			// 
			this.mSplitContainer.Panel2.Controls.Add( this.mLogTextBox );
			this.mSplitContainer.Size = new System.Drawing.Size( 376, 335 );
			this.mSplitContainer.SplitterDistance = 167;
			this.mSplitContainer.TabIndex = 12;
			// 
			// mVolumeListView
			// 
			this.mVolumeListView.AllColumns.Add( this.mColumn );
			this.mVolumeListView.CheckBoxes = true;
			this.mVolumeListView.Columns.AddRange( new System.Windows.Forms.ColumnHeader[] {
            this.mColumn} );
			this.mVolumeListView.ContextMenuStrip = this.mVolumeContextMenu;
			this.mVolumeListView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mVolumeListView.FullRowSelect = true;
			this.mVolumeListView.Location = new System.Drawing.Point( 0, 0 );
			this.mVolumeListView.MultiSelect = false;
			this.mVolumeListView.Name = "mVolumeListView";
			this.mVolumeListView.OwnerDraw = true;
			this.mVolumeListView.ShowGroups = false;
			this.mVolumeListView.ShowImagesOnSubItems = true;
			this.mVolumeListView.Size = new System.Drawing.Size( 376, 167 );
			this.mVolumeListView.TabIndex = 13;
			this.mVolumeListView.UseCompatibleStateImageBehavior = false;
			this.mVolumeListView.View = System.Windows.Forms.View.Details;
			// 
			// mColumn
			// 
			this.mColumn.FillsFreeSpace = true;
			this.mColumn.MinimumWidth = 150;
			this.mColumn.Text = "Volume";
			this.mColumn.Width = 190;
			// 
			// mLogTextBox
			// 
			this.mLogTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mLogTextBox.Location = new System.Drawing.Point( 0, 0 );
			this.mLogTextBox.Name = "mLogTextBox";
			this.mLogTextBox.Size = new System.Drawing.Size( 376, 164 );
			this.mLogTextBox.TabIndex = 14;
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size( 246, 6 );
			// 
			// openLocationInExplorer
			// 
			this.openLocationInExplorer.Name = "openLocationInExplorer";
			this.openLocationInExplorer.Size = new System.Drawing.Size( 249, 22 );
			this.openLocationInExplorer.Text = "Open location in explorer";
			this.openLocationInExplorer.Click += new System.EventHandler( this.openLocationInExplorer_Click );
			// 
			// openBackupLocationInExplorer
			// 
			this.openBackupLocationInExplorer.Name = "openBackupLocationInExplorer";
			this.openBackupLocationInExplorer.Size = new System.Drawing.Size( 249, 22 );
			this.openBackupLocationInExplorer.Text = "Open backup location in explorer";
			this.openBackupLocationInExplorer.Click += new System.EventHandler( this.openBackupLocationInExplorer_Click );
			// 
			// WelcomeListView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add( this.mSplitContainer );
			this.Controls.Add( this.mCommonOperationsGroupBox );
			this.Controls.Add( this.mSpecificOperationsGroupBox );
			this.MinimumSize = new System.Drawing.Size( 325, 196 );
			this.Name = "WelcomeListView";
			this.Size = new System.Drawing.Size( 501, 341 );
			this.mVolumeContextMenu.ResumeLayout( false );
			this.mSpecificOperationsGroupBox.ResumeLayout( false );
			this.mCommonOperationsGroupBox.ResumeLayout( false );
			this.mSplitContainer.Panel1.ResumeLayout( false );
			this.mSplitContainer.Panel2.ResumeLayout( false );
			this.mSplitContainer.ResumeLayout( false );
			((System.ComponentModel.ISupportInitialize)(this.mVolumeListView)).EndInit();
			this.ResumeLayout( false );

		}

		#endregion

		private System.Windows.Forms.ContextMenuStrip mVolumeContextMenu;
		private System.Windows.Forms.ToolStripMenuItem restoreVolumeToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem addNewVolumeToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem removeVolumeToolStripMenuItem;
		private System.Windows.Forms.GroupBox mSpecificOperationsGroupBox;
		private System.Windows.Forms.GroupBox mCommonOperationsGroupBox;
		private System.Windows.Forms.Button mRemoveVolumeButton;
		private System.Windows.Forms.Button mRestoreVolumeButton;
		private System.Windows.Forms.Button mAddVolumeButton;
		private System.Windows.Forms.Button mButtonOptions;
		private System.Windows.Forms.SplitContainer mSplitContainer;
		private BrightIdeasSoftware.ObjectListView mVolumeListView;
		private BrightIdeasSoftware.OLVColumn mColumn;
		private LogTextBox mLogTextBox;
		private System.Windows.Forms.Button mBackupAllNowButton;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripMenuItem openLocationInExplorer;
		private System.Windows.Forms.ToolStripMenuItem openBackupLocationInExplorer;
	}
}

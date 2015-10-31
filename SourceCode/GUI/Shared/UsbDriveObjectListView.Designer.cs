namespace AutoUSBBackup.GUI.Shared
{
	partial class UsbDriveObjectListView
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.mTreeView = new BrightIdeasSoftware.ObjectListView();
			( ( System.ComponentModel.ISupportInitialize )( this.mTreeView ) ).BeginInit();
			this.SuspendLayout();
			// 
			// mTreeView
			// 
			this.mTreeView.CheckBoxes = true;
			this.mTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mTreeView.FullRowSelect = true;
			this.mTreeView.HideSelection = false;
			this.mTreeView.Location = new System.Drawing.Point( 0, 0 );
			this.mTreeView.MultiSelect = false;
			this.mTreeView.Name = "mTreeView";
			this.mTreeView.OwnerDraw = true;
			this.mTreeView.ShowGroups = false;
			this.mTreeView.ShowImagesOnSubItems = true;
			this.mTreeView.Size = new System.Drawing.Size( 488, 286 );
			this.mTreeView.TabIndex = 0;
			this.mTreeView.UseCompatibleStateImageBehavior = false;
			this.mTreeView.View = System.Windows.Forms.View.Details;
			// 
			// UsbDriveObjectListView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add( this.mTreeView );
			this.Name = "UsbDriveObjectListView";
			this.Size = new System.Drawing.Size( 488, 286 );
			( ( System.ComponentModel.ISupportInitialize )( this.mTreeView ) ).EndInit();
			this.ResumeLayout( false );

		}

		#endregion

		private BrightIdeasSoftware.ObjectListView mTreeView;
	}
}

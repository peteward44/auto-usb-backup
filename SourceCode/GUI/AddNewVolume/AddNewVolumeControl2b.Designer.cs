namespace AutoUSBBackup.GUI
{
	partial class AddNewVolumeControl2b
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
			this.mButtonNext = new System.Windows.Forms.Button();
			this.mButtonBack = new System.Windows.Forms.Button();
			this.mUsbDeviceView = new AutoUSBBackup.GUI.Shared.UsbDriveObjectListView();
			this.SuspendLayout();
			// 
			// mButtonNext
			// 
			this.mButtonNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.mButtonNext.Enabled = false;
			this.mButtonNext.Location = new System.Drawing.Point(398, 298);
			this.mButtonNext.Name = "mButtonNext";
			this.mButtonNext.Size = new System.Drawing.Size(93, 40);
			this.mButtonNext.TabIndex = 2;
			this.mButtonNext.Text = "Next";
			this.mButtonNext.UseVisualStyleBackColor = true;
			this.mButtonNext.Click += new System.EventHandler(this.mButtonNext_Click);
			// 
			// mButtonBack
			// 
			this.mButtonBack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.mButtonBack.Location = new System.Drawing.Point(3, 298);
			this.mButtonBack.Name = "mButtonBack";
			this.mButtonBack.Size = new System.Drawing.Size(93, 40);
			this.mButtonBack.TabIndex = 1;
			this.mButtonBack.Text = "Back";
			this.mButtonBack.UseVisualStyleBackColor = true;
			this.mButtonBack.Click += new System.EventHandler(this.mButtonBack_Click);
			// 
			// mUsbDeviceView
			// 
			this.mUsbDeviceView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.mUsbDeviceView.Location = new System.Drawing.Point(3, 3);
			this.mUsbDeviceView.Name = "mUsbDeviceView";
			this.mUsbDeviceView.Size = new System.Drawing.Size(488, 289);
			this.mUsbDeviceView.TabIndex = 0;
			// 
			// AddNewVolumeControl2b
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.mUsbDeviceView);
			this.Controls.Add(this.mButtonBack);
			this.Controls.Add(this.mButtonNext);
			this.Name = "AddNewVolumeControl2b";
			this.Size = new System.Drawing.Size(494, 341);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button mButtonNext;
		private System.Windows.Forms.Button mButtonBack;
		private Shared.UsbDriveObjectListView mUsbDeviceView;
	}
}

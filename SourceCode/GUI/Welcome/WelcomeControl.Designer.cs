namespace AutoUSBBackup.GUI
{
	partial class WelcomeControl
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
			this.mTreeView = new AutoUSBBackup.GUI.Shared.WelcomeListView();
			this.SuspendLayout();
			// 
			// mTreeView
			// 
			this.mTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.mTreeView.Location = new System.Drawing.Point(3, 3);
			this.mTreeView.Name = "mTreeView";
			this.mTreeView.Size = new System.Drawing.Size(488, 335);
			this.mTreeView.TabIndex = 0;
			this.mTreeView.Load += new System.EventHandler(this.mTreeView_Load);
			// 
			// WelcomeControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.mTreeView);
			this.Name = "WelcomeControl";
			this.Size = new System.Drawing.Size(494, 341);
			this.ResumeLayout(false);

		}

		#endregion

		private GUI.Shared.WelcomeListView mTreeView;

	}
}

namespace AutoUSBBackup.GUI
{
	partial class RemoveVolumeForm
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
			this.mLabel = new System.Windows.Forms.Label();
			this.mYesButton = new System.Windows.Forms.Button();
			this.mNoButton = new System.Windows.Forms.Button();
			this.mRemoveDataCheckbox = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// mLabel
			// 
			this.mLabel.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left )
						| System.Windows.Forms.AnchorStyles.Right ) ) );
			this.mLabel.Location = new System.Drawing.Point( 12, 9 );
			this.mLabel.Name = "mLabel";
			this.mLabel.Size = new System.Drawing.Size( 316, 50 );
			this.mLabel.TabIndex = 0;
			this.mLabel.Text = "Are you sure you wish to remove volume?";
			this.mLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// mYesButton
			// 
			this.mYesButton.Location = new System.Drawing.Point( 31, 77 );
			this.mYesButton.Name = "mYesButton";
			this.mYesButton.Size = new System.Drawing.Size( 75, 23 );
			this.mYesButton.TabIndex = 1;
			this.mYesButton.Text = "Yes";
			this.mYesButton.UseVisualStyleBackColor = true;
			this.mYesButton.Click += new System.EventHandler( this.mYesButton_Click );
			// 
			// mNoButton
			// 
			this.mNoButton.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right ) ) );
			this.mNoButton.Location = new System.Drawing.Point( 232, 77 );
			this.mNoButton.Name = "mNoButton";
			this.mNoButton.Size = new System.Drawing.Size( 75, 23 );
			this.mNoButton.TabIndex = 2;
			this.mNoButton.Text = "No";
			this.mNoButton.UseVisualStyleBackColor = true;
			this.mNoButton.Click += new System.EventHandler( this.mNoButton_Click );
			// 
			// mRemoveDataCheckbox
			// 
			this.mRemoveDataCheckbox.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.mRemoveDataCheckbox.Location = new System.Drawing.Point( 59, 120 );
			this.mRemoveDataCheckbox.Name = "mRemoveDataCheckbox";
			this.mRemoveDataCheckbox.Size = new System.Drawing.Size( 218, 24 );
			this.mRemoveDataCheckbox.TabIndex = 3;
			this.mRemoveDataCheckbox.Text = "Permanently delete all backed up data";
			this.mRemoveDataCheckbox.UseVisualStyleBackColor = true;
			// 
			// RemoveVolumeForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size( 340, 161 );
			this.ControlBox = false;
			this.Controls.Add( this.mRemoveDataCheckbox );
			this.Controls.Add( this.mNoButton );
			this.Controls.Add( this.mYesButton );
			this.Controls.Add( this.mLabel );
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "RemoveVolumeForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Remove Volume";
			this.ResumeLayout( false );

		}

		#endregion

		private System.Windows.Forms.Label mLabel;
		private System.Windows.Forms.Button mYesButton;
		private System.Windows.Forms.Button mNoButton;
		private System.Windows.Forms.CheckBox mRemoveDataCheckbox;
	}
}
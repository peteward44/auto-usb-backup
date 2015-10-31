namespace AutoUSBBackup.GUI
{
	partial class AddNewVolumeControl2a
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
			this.mGroupBox = new System.Windows.Forms.GroupBox();
			this.mButtonBrowse = new System.Windows.Forms.Button();
			this.mLocalFolderLabel = new System.Windows.Forms.Label();
			this.mGroupBox.SuspendLayout();
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
			// mGroupBox
			// 
			this.mGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.mGroupBox.Controls.Add(this.mButtonBrowse);
			this.mGroupBox.Controls.Add(this.mLocalFolderLabel);
			this.mGroupBox.Location = new System.Drawing.Point(16, 38);
			this.mGroupBox.Name = "mGroupBox";
			this.mGroupBox.Size = new System.Drawing.Size(461, 68);
			this.mGroupBox.TabIndex = 3;
			this.mGroupBox.TabStop = false;
			this.mGroupBox.Text = "Local folder path";
			// 
			// mButtonBrowse
			// 
			this.mButtonBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.mButtonBrowse.Location = new System.Drawing.Point(387, 23);
			this.mButtonBrowse.Name = "mButtonBrowse";
			this.mButtonBrowse.Size = new System.Drawing.Size(68, 28);
			this.mButtonBrowse.TabIndex = 1;
			this.mButtonBrowse.Text = "Browse...";
			this.mButtonBrowse.UseVisualStyleBackColor = true;
			this.mButtonBrowse.Click += new System.EventHandler(this.mButtonBrowse_Click);
			// 
			// mLocalFolderLabel
			// 
			this.mLocalFolderLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.mLocalFolderLabel.Location = new System.Drawing.Point(19, 31);
			this.mLocalFolderLabel.Name = "mLocalFolderLabel";
			this.mLocalFolderLabel.Size = new System.Drawing.Size(362, 18);
			this.mLocalFolderLabel.TabIndex = 0;
			this.mLocalFolderLabel.Text = "label1";
			// 
			// AddNewVolumeControl2a
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.mGroupBox);
			this.Controls.Add(this.mButtonBack);
			this.Controls.Add(this.mButtonNext);
			this.Name = "AddNewVolumeControl2a";
			this.Size = new System.Drawing.Size(494, 341);
			this.mGroupBox.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button mButtonNext;
		private System.Windows.Forms.Button mButtonBack;
		private System.Windows.Forms.GroupBox mGroupBox;
		private System.Windows.Forms.Label mLocalFolderLabel;
		private System.Windows.Forms.Button mButtonBrowse;
	}
}

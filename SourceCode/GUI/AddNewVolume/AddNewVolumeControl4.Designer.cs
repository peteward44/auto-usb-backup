namespace AutoUSBBackup.GUI
{
	partial class AddNewVolumeControl4
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
			this.mButtonBack = new System.Windows.Forms.Button();
			this.mButtonNext = new System.Windows.Forms.Button();
			this.mLabel = new System.Windows.Forms.Label();
			this.mStartBackupNow = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// mButtonBack
			// 
			this.mButtonBack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.mButtonBack.Location = new System.Drawing.Point(3, 298);
			this.mButtonBack.Name = "mButtonBack";
			this.mButtonBack.Size = new System.Drawing.Size(93, 40);
			this.mButtonBack.TabIndex = 3;
			this.mButtonBack.Text = "Back";
			this.mButtonBack.UseVisualStyleBackColor = true;
			this.mButtonBack.Click += new System.EventHandler(this.mButtonBack_Click);
			// 
			// mButtonNext
			// 
			this.mButtonNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.mButtonNext.Location = new System.Drawing.Point(398, 298);
			this.mButtonNext.Name = "mButtonNext";
			this.mButtonNext.Size = new System.Drawing.Size(93, 40);
			this.mButtonNext.TabIndex = 4;
			this.mButtonNext.Text = "Finish";
			this.mButtonNext.UseVisualStyleBackColor = true;
			this.mButtonNext.Click += new System.EventHandler(this.mButtonNext_Click);
			// 
			// mLabel
			// 
			this.mLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.mLabel.Location = new System.Drawing.Point(15, 16);
			this.mLabel.Name = "mLabel";
			this.mLabel.Size = new System.Drawing.Size(465, 168);
			this.mLabel.TabIndex = 5;
			this.mLabel.Text = "LabelText";
			this.mLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// mStartBackupNow
			// 
			this.mStartBackupNow.AutoSize = true;
			this.mStartBackupNow.Location = new System.Drawing.Point(28, 201);
			this.mStartBackupNow.Name = "mStartBackupNow";
			this.mStartBackupNow.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.mStartBackupNow.Size = new System.Drawing.Size(110, 17);
			this.mStartBackupNow.TabIndex = 6;
			this.mStartBackupNow.Text = "Start backup now";
			this.mStartBackupNow.UseVisualStyleBackColor = true;
			// 
			// AddNewVolumeControl4
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.mStartBackupNow);
			this.Controls.Add(this.mLabel);
			this.Controls.Add(this.mButtonBack);
			this.Controls.Add(this.mButtonNext);
			this.Name = "AddNewVolumeControl4";
			this.Size = new System.Drawing.Size(494, 341);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button mButtonBack;
		private System.Windows.Forms.Button mButtonNext;
		private System.Windows.Forms.Label mLabel;
		private System.Windows.Forms.CheckBox mStartBackupNow;
	}
}

namespace AutoUSBBackup.GUI
{
	partial class AddNewVolumeControl3
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
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.mStorageLabel = new System.Windows.Forms.Label();
			this.mTrackBarStorage = new System.Windows.Forms.TrackBar();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.mButtonBrowse = new System.Windows.Forms.Button();
			this.mBackupToLabel = new System.Windows.Forms.Label();
			this.groupBox1.SuspendLayout();
			( ( System.ComponentModel.ISupportInitialize )( this.mTrackBarStorage ) ).BeginInit();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// mButtonBack
			// 
			this.mButtonBack.Anchor = ( ( System.Windows.Forms.AnchorStyles )( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left ) ) );
			this.mButtonBack.Location = new System.Drawing.Point( 3, 298 );
			this.mButtonBack.Name = "mButtonBack";
			this.mButtonBack.Size = new System.Drawing.Size( 93, 40 );
			this.mButtonBack.TabIndex = 3;
			this.mButtonBack.Text = "Back";
			this.mButtonBack.UseVisualStyleBackColor = true;
			this.mButtonBack.Click += new System.EventHandler( this.mButtonBack_Click );
			// 
			// mButtonNext
			// 
			this.mButtonNext.Anchor = ( ( System.Windows.Forms.AnchorStyles )( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right ) ) );
			this.mButtonNext.Location = new System.Drawing.Point( 398, 298 );
			this.mButtonNext.Name = "mButtonNext";
			this.mButtonNext.Size = new System.Drawing.Size( 93, 40 );
			this.mButtonNext.TabIndex = 4;
			this.mButtonNext.Text = "Next";
			this.mButtonNext.UseVisualStyleBackColor = true;
			this.mButtonNext.Click += new System.EventHandler( this.mButtonNext_Click );
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ( ( System.Windows.Forms.AnchorStyles )( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left )
									| System.Windows.Forms.AnchorStyles.Right ) ) );
			this.groupBox1.Controls.Add( this.mStorageLabel );
			this.groupBox1.Controls.Add( this.mTrackBarStorage );
			this.groupBox1.Location = new System.Drawing.Point( 3, 3 );
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size( 488, 93 );
			this.groupBox1.TabIndex = 5;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Storage";
			// 
			// mStorageLabel
			// 
			this.mStorageLabel.Anchor = ( ( System.Windows.Forms.AnchorStyles )( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left )
									| System.Windows.Forms.AnchorStyles.Right ) ) );
			this.mStorageLabel.Location = new System.Drawing.Point( 6, 16 );
			this.mStorageLabel.Name = "mStorageLabel";
			this.mStorageLabel.Size = new System.Drawing.Size( 476, 20 );
			this.mStorageLabel.TabIndex = 1;
			this.mStorageLabel.Text = "Storage Label";
			this.mStorageLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// mTrackBarStorage
			// 
			this.mTrackBarStorage.Anchor = ( ( System.Windows.Forms.AnchorStyles )( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left )
									| System.Windows.Forms.AnchorStyles.Right ) ) );
			this.mTrackBarStorage.Location = new System.Drawing.Point( 6, 45 );
			this.mTrackBarStorage.Name = "mTrackBarStorage";
			this.mTrackBarStorage.Size = new System.Drawing.Size( 476, 45 );
			this.mTrackBarStorage.TabIndex = 0;
			this.mTrackBarStorage.Value = 5;
			this.mTrackBarStorage.ValueChanged += new System.EventHandler( this.mTrackBarStorage_ValueChanged );
			// 
			// groupBox2
			// 
			this.groupBox2.Anchor = ( ( System.Windows.Forms.AnchorStyles )( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left )
									| System.Windows.Forms.AnchorStyles.Right ) ) );
			this.groupBox2.Controls.Add( this.mBackupToLabel );
			this.groupBox2.Controls.Add( this.mButtonBrowse );
			this.groupBox2.Location = new System.Drawing.Point( 3, 102 );
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size( 482, 105 );
			this.groupBox2.TabIndex = 6;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Backup to...";
			// 
			// mButtonBrowse
			// 
			this.mButtonBrowse.Anchor = ( ( System.Windows.Forms.AnchorStyles )( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right ) ) );
			this.mButtonBrowse.Location = new System.Drawing.Point( 402, 46 );
			this.mButtonBrowse.Name = "mButtonBrowse";
			this.mButtonBrowse.Size = new System.Drawing.Size( 74, 22 );
			this.mButtonBrowse.TabIndex = 0;
			this.mButtonBrowse.Text = "Browse...";
			this.mButtonBrowse.UseVisualStyleBackColor = true;
			this.mButtonBrowse.Click += new System.EventHandler( this.mButtonBrowse_Click );
			// 
			// mBackupToLabel
			// 
			this.mBackupToLabel.Anchor = ( ( System.Windows.Forms.AnchorStyles )( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left )
									| System.Windows.Forms.AnchorStyles.Right ) ) );
			this.mBackupToLabel.Location = new System.Drawing.Point( 16, 46 );
			this.mBackupToLabel.Name = "mBackupToLabel";
			this.mBackupToLabel.Size = new System.Drawing.Size( 380, 23 );
			this.mBackupToLabel.TabIndex = 1;
			this.mBackupToLabel.Text = "label1";
			// 
			// AddNewVolumeControl3
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add( this.groupBox2 );
			this.Controls.Add( this.groupBox1 );
			this.Controls.Add( this.mButtonBack );
			this.Controls.Add( this.mButtonNext );
			this.Name = "AddNewVolumeControl3";
			this.Size = new System.Drawing.Size( 494, 341 );
			this.groupBox1.ResumeLayout( false );
			this.groupBox1.PerformLayout();
			( ( System.ComponentModel.ISupportInitialize )( this.mTrackBarStorage ) ).EndInit();
			this.groupBox2.ResumeLayout( false );
			this.ResumeLayout( false );

		}

		#endregion

		private System.Windows.Forms.Button mButtonBack;
		private System.Windows.Forms.Button mButtonNext;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label mStorageLabel;
		private System.Windows.Forms.TrackBar mTrackBarStorage;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label mBackupToLabel;
		private System.Windows.Forms.Button mButtonBrowse;
	}
}

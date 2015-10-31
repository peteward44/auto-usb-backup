namespace AutoUSBBackup.GUI
{
	partial class AddNewVolumeControl2
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
			this.mRadio2 = new System.Windows.Forms.RadioButton();
			this.mRadio1 = new System.Windows.Forms.RadioButton();
			this.mAddExistingRadio = new System.Windows.Forms.RadioButton();
			this.mGroupBox.SuspendLayout();
			this.SuspendLayout();
			// 
			// mButtonNext
			// 
			this.mButtonNext.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right ) ) );
			this.mButtonNext.Enabled = false;
			this.mButtonNext.Location = new System.Drawing.Point( 398, 298 );
			this.mButtonNext.Name = "mButtonNext";
			this.mButtonNext.Size = new System.Drawing.Size( 93, 40 );
			this.mButtonNext.TabIndex = 2;
			this.mButtonNext.Text = "Next";
			this.mButtonNext.UseVisualStyleBackColor = true;
			this.mButtonNext.Click += new System.EventHandler( this.mButtonNext_Click );
			// 
			// mButtonBack
			// 
			this.mButtonBack.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left ) ) );
			this.mButtonBack.Location = new System.Drawing.Point( 3, 298 );
			this.mButtonBack.Name = "mButtonBack";
			this.mButtonBack.Size = new System.Drawing.Size( 93, 40 );
			this.mButtonBack.TabIndex = 1;
			this.mButtonBack.Text = "Back";
			this.mButtonBack.UseVisualStyleBackColor = true;
			this.mButtonBack.Click += new System.EventHandler( this.mButtonBack_Click );
			// 
			// mGroupBox
			// 
			this.mGroupBox.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom )
						| System.Windows.Forms.AnchorStyles.Left )
						| System.Windows.Forms.AnchorStyles.Right ) ) );
			this.mGroupBox.Controls.Add( this.mAddExistingRadio );
			this.mGroupBox.Controls.Add( this.mRadio2 );
			this.mGroupBox.Controls.Add( this.mRadio1 );
			this.mGroupBox.Location = new System.Drawing.Point( 3, 3 );
			this.mGroupBox.Name = "mGroupBox";
			this.mGroupBox.Size = new System.Drawing.Size( 488, 289 );
			this.mGroupBox.TabIndex = 3;
			this.mGroupBox.TabStop = false;
			this.mGroupBox.Text = "Backup type";
			// 
			// mRadio2
			// 
			this.mRadio2.AutoSize = true;
			this.mRadio2.Location = new System.Drawing.Point( 27, 74 );
			this.mRadio2.Name = "mRadio2";
			this.mRadio2.Size = new System.Drawing.Size( 140, 17 );
			this.mRadio2.TabIndex = 1;
			this.mRadio2.TabStop = true;
			this.mRadio2.Text = "Folder on local hard disk";
			this.mRadio2.UseVisualStyleBackColor = true;
			// 
			// mRadio1
			// 
			this.mRadio1.AutoSize = true;
			this.mRadio1.Location = new System.Drawing.Point( 27, 39 );
			this.mRadio1.Name = "mRadio1";
			this.mRadio1.Size = new System.Drawing.Size( 82, 17 );
			this.mRadio1.TabIndex = 0;
			this.mRadio1.TabStop = true;
			this.mRadio1.Text = "USB device";
			this.mRadio1.UseVisualStyleBackColor = true;
			// 
			// mAddExistingRadio
			// 
			this.mAddExistingRadio.AutoSize = true;
			this.mAddExistingRadio.Location = new System.Drawing.Point( 27, 125 );
			this.mAddExistingRadio.Name = "mAddExistingRadio";
			this.mAddExistingRadio.Size = new System.Drawing.Size( 158, 17 );
			this.mAddExistingRadio.TabIndex = 2;
			this.mAddExistingRadio.TabStop = true;
			this.mAddExistingRadio.Text = "Add existing volume backup";
			this.mAddExistingRadio.UseVisualStyleBackColor = true;
			// 
			// AddNewVolumeControl2
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add( this.mGroupBox );
			this.Controls.Add( this.mButtonBack );
			this.Controls.Add( this.mButtonNext );
			this.Name = "AddNewVolumeControl2";
			this.Size = new System.Drawing.Size( 494, 341 );
			this.mGroupBox.ResumeLayout( false );
			this.mGroupBox.PerformLayout();
			this.ResumeLayout( false );

		}

		#endregion

		private System.Windows.Forms.Button mButtonNext;
		private System.Windows.Forms.Button mButtonBack;
		private System.Windows.Forms.GroupBox mGroupBox;
		private System.Windows.Forms.RadioButton mRadio2;
		private System.Windows.Forms.RadioButton mRadio1;
		private System.Windows.Forms.RadioButton mAddExistingRadio;
	}
}

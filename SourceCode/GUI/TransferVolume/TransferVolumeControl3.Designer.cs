namespace AutoUSBBackup.GUI
{
	partial class TransferVolumeControl3
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
			this.mLabel = new System.Windows.Forms.Label();
			this.mCancelButton = new System.Windows.Forms.Button();
			this.mStartButton = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// mButtonBack
			// 
			this.mButtonBack.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left ) ) );
			this.mButtonBack.Location = new System.Drawing.Point( 3, 295 );
			this.mButtonBack.Name = "mButtonBack";
			this.mButtonBack.Size = new System.Drawing.Size( 93, 40 );
			this.mButtonBack.TabIndex = 5;
			this.mButtonBack.Text = "Back";
			this.mButtonBack.UseVisualStyleBackColor = true;
			this.mButtonBack.Click += new System.EventHandler( this.mButtonBack_Click );
			// 
			// mButtonNext
			// 
			this.mButtonNext.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right ) ) );
			this.mButtonNext.Location = new System.Drawing.Point( 398, 295 );
			this.mButtonNext.Name = "mButtonNext";
			this.mButtonNext.Size = new System.Drawing.Size( 93, 40 );
			this.mButtonNext.TabIndex = 6;
			this.mButtonNext.Text = "Finish";
			this.mButtonNext.UseVisualStyleBackColor = true;
			this.mButtonNext.Click += new System.EventHandler( this.mButtonNext_Click );
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom )
						| System.Windows.Forms.AnchorStyles.Left )
						| System.Windows.Forms.AnchorStyles.Right ) ) );
			this.groupBox1.Controls.Add( this.mStartButton );
			this.groupBox1.Controls.Add( this.mCancelButton );
			this.groupBox1.Controls.Add( this.mLabel );
			this.groupBox1.Location = new System.Drawing.Point( 3, 0 );
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size( 488, 289 );
			this.groupBox1.TabIndex = 7;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Transfer";
			// 
			// mLabel
			// 
			this.mLabel.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left )
						| System.Windows.Forms.AnchorStyles.Right ) ) );
			this.mLabel.Location = new System.Drawing.Point( 6, 28 );
			this.mLabel.Name = "mLabel";
			this.mLabel.Size = new System.Drawing.Size( 476, 23 );
			this.mLabel.TabIndex = 1;
			this.mLabel.Text = "label1";
			this.mLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// mCancelButton
			// 
			this.mCancelButton.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right ) ) );
			this.mCancelButton.Location = new System.Drawing.Point( 407, 101 );
			this.mCancelButton.Name = "mCancelButton";
			this.mCancelButton.Size = new System.Drawing.Size( 75, 23 );
			this.mCancelButton.TabIndex = 2;
			this.mCancelButton.Text = "Cancel";
			this.mCancelButton.UseVisualStyleBackColor = true;
			this.mCancelButton.Click += new System.EventHandler( this.mCancelButton_Click );
			// 
			// mStartButton
			// 
			this.mStartButton.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.mStartButton.Location = new System.Drawing.Point( 202, 101 );
			this.mStartButton.Name = "mStartButton";
			this.mStartButton.Size = new System.Drawing.Size( 75, 23 );
			this.mStartButton.TabIndex = 3;
			this.mStartButton.Text = "Start Format";
			this.mStartButton.UseVisualStyleBackColor = true;
			this.mStartButton.Click += new System.EventHandler( this.mStartButton_Click );
			// 
			// TransferVolumeControl3
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add( this.groupBox1 );
			this.Controls.Add( this.mButtonBack );
			this.Controls.Add( this.mButtonNext );
			this.Name = "TransferVolumeControl3";
			this.Size = new System.Drawing.Size( 494, 341 );
			this.groupBox1.ResumeLayout( false );
			this.ResumeLayout( false );

		}

		#endregion

		private System.Windows.Forms.Button mButtonBack;
		private System.Windows.Forms.Button mButtonNext;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label mLabel;
		private System.Windows.Forms.Button mCancelButton;
		private System.Windows.Forms.Button mStartButton;
	}
}

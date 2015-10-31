namespace AutoUSBBackup.GUI
{
	partial class AddNewVolumeControl1
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
			this.mTextBox = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.mErrorLabel = new System.Windows.Forms.Label();
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
			this.mButtonNext.Enabled = false;
			this.mButtonNext.Location = new System.Drawing.Point(398, 298);
			this.mButtonNext.Name = "mButtonNext";
			this.mButtonNext.Size = new System.Drawing.Size(93, 40);
			this.mButtonNext.TabIndex = 4;
			this.mButtonNext.Text = "Next";
			this.mButtonNext.UseVisualStyleBackColor = true;
			this.mButtonNext.Click += new System.EventHandler(this.mButtonNext_Click);
			// 
			// mTextBox
			// 
			this.mTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.mTextBox.Location = new System.Drawing.Point(43, 78);
			this.mTextBox.Name = "mTextBox";
			this.mTextBox.Size = new System.Drawing.Size(400, 20);
			this.mTextBox.TabIndex = 5;
			this.mTextBox.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(40, 48);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(84, 13);
			this.label1.TabIndex = 6;
			this.label1.Text = "Name of volume";
			// 
			// mErrorLabel
			// 
			this.mErrorLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.mErrorLabel.ForeColor = System.Drawing.Color.Red;
			this.mErrorLabel.Location = new System.Drawing.Point(43, 122);
			this.mErrorLabel.Name = "mErrorLabel";
			this.mErrorLabel.Size = new System.Drawing.Size(400, 43);
			this.mErrorLabel.TabIndex = 7;
			this.mErrorLabel.Text = "Error Label";
			this.mErrorLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// AddNewVolumeControl1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.mErrorLabel);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.mTextBox);
			this.Controls.Add(this.mButtonBack);
			this.Controls.Add(this.mButtonNext);
			this.Name = "AddNewVolumeControl1";
			this.Size = new System.Drawing.Size(494, 341);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button mButtonBack;
		private System.Windows.Forms.Button mButtonNext;
		private System.Windows.Forms.TextBox mTextBox;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label mErrorLabel;
	}
}

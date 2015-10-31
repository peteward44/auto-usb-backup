namespace AutoUSBBackup.GUI
{
	partial class RestoreVolumesControl1
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
			this.mRevisionListView = new BrightIdeasSoftware.ObjectListView();
			this.mRevisionNameColumn = new BrightIdeasSoftware.OLVColumn();
			this.mDateColumn = new BrightIdeasSoftware.OLVColumn();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.mRestoreFileNameLabel = new System.Windows.Forms.Label();
			this.mFuzzyTimeColumn = new BrightIdeasSoftware.OLVColumn();
			( (System.ComponentModel.ISupportInitialize)( this.mRevisionListView ) ).BeginInit();
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
			this.mButtonNext.Text = "Next";
			this.mButtonNext.UseVisualStyleBackColor = true;
			this.mButtonNext.Click += new System.EventHandler( this.mButtonNext_Click );
			// 
			// mRevisionListView
			// 
			this.mRevisionListView.AllColumns.Add( this.mRevisionNameColumn );
			this.mRevisionListView.AllColumns.Add( this.mFuzzyTimeColumn );
			this.mRevisionListView.AllColumns.Add( this.mDateColumn );
			this.mRevisionListView.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom )
						| System.Windows.Forms.AnchorStyles.Left )
						| System.Windows.Forms.AnchorStyles.Right ) ) );
			this.mRevisionListView.CheckBoxes = true;
			this.mRevisionListView.Columns.AddRange( new System.Windows.Forms.ColumnHeader[] {
            this.mRevisionNameColumn,
            this.mFuzzyTimeColumn,
            this.mDateColumn} );
			this.mRevisionListView.FullRowSelect = true;
			this.mRevisionListView.Location = new System.Drawing.Point( 9, 60 );
			this.mRevisionListView.MultiSelect = false;
			this.mRevisionListView.Name = "mRevisionListView";
			this.mRevisionListView.Size = new System.Drawing.Size( 472, 220 );
			this.mRevisionListView.TabIndex = 7;
			this.mRevisionListView.UseCompatibleStateImageBehavior = false;
			this.mRevisionListView.View = System.Windows.Forms.View.Details;
			// 
			// mRevisionNameColumn
			// 
			this.mRevisionNameColumn.AspectName = "Name";
			this.mRevisionNameColumn.FillsFreeSpace = true;
			this.mRevisionNameColumn.Text = "Revision";
			this.mRevisionNameColumn.Width = 250;
			// 
			// mDateColumn
			// 
			this.mDateColumn.AspectName = "DateString";
			this.mDateColumn.Text = "Date";
			this.mDateColumn.Width = 200;
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left )
						| System.Windows.Forms.AnchorStyles.Right ) ) );
			this.groupBox1.Controls.Add( this.mRestoreFileNameLabel );
			this.groupBox1.Controls.Add( this.mRevisionListView );
			this.groupBox1.Location = new System.Drawing.Point( 3, 3 );
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size( 487, 286 );
			this.groupBox1.TabIndex = 8;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Restore";
			// 
			// mRestoreFileNameLabel
			// 
			this.mRestoreFileNameLabel.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left )
						| System.Windows.Forms.AnchorStyles.Right ) ) );
			this.mRestoreFileNameLabel.Location = new System.Drawing.Point( 6, 25 );
			this.mRestoreFileNameLabel.Name = "mRestoreFileNameLabel";
			this.mRestoreFileNameLabel.Size = new System.Drawing.Size( 475, 13 );
			this.mRestoreFileNameLabel.TabIndex = 0;
			this.mRestoreFileNameLabel.Text = "label1";
			// 
			// mFuzzyTimeColumn
			// 
			this.mFuzzyTimeColumn.AspectName = "FuzzyTimeString";
			this.mFuzzyTimeColumn.Text = "Created";
			this.mFuzzyTimeColumn.Width = 100;
			// 
			// RestoreVolumesControl1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add( this.groupBox1 );
			this.Controls.Add( this.mButtonBack );
			this.Controls.Add( this.mButtonNext );
			this.Name = "RestoreVolumesControl1";
			this.Size = new System.Drawing.Size( 494, 341 );
			( (System.ComponentModel.ISupportInitialize)( this.mRevisionListView ) ).EndInit();
			this.groupBox1.ResumeLayout( false );
			this.ResumeLayout( false );

		}

		#endregion

		private System.Windows.Forms.Button mButtonBack;
		private System.Windows.Forms.Button mButtonNext;
		private BrightIdeasSoftware.ObjectListView mRevisionListView;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label mRestoreFileNameLabel;
		private BrightIdeasSoftware.OLVColumn mRevisionNameColumn;
		private BrightIdeasSoftware.OLVColumn mDateColumn;
		private BrightIdeasSoftware.OLVColumn mFuzzyTimeColumn;
	}
}

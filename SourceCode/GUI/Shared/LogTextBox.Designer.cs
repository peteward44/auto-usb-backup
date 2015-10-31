namespace AutoUSBBackup.GUI.Shared
{
	partial class LogTextBox
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
			this.mTextBox = new BrightIdeasSoftware.ObjectListView();
			this.mDateColumn = new BrightIdeasSoftware.OLVColumn();
			this.mNameColumn = new BrightIdeasSoftware.OLVColumn();
			( ( System.ComponentModel.ISupportInitialize )( this.mTextBox ) ).BeginInit();
			this.SuspendLayout();
			// 
			// mTextBox
			// 
			this.mTextBox.AllColumns.Add( this.mDateColumn );
			this.mTextBox.AllColumns.Add( this.mNameColumn );
			this.mTextBox.AutoArrange = false;
			this.mTextBox.Columns.AddRange( new System.Windows.Forms.ColumnHeader[] {
            this.mDateColumn,
            this.mNameColumn} );
			this.mTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mTextBox.FullRowSelect = true;
			this.mTextBox.GridLines = true;
			this.mTextBox.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.mTextBox.Location = new System.Drawing.Point( 0, 0 );
			this.mTextBox.MultiSelect = false;
			this.mTextBox.Name = "mTextBox";
			this.mTextBox.ShowGroups = false;
			this.mTextBox.Size = new System.Drawing.Size( 263, 139 );
			this.mTextBox.TabIndex = 0;
			this.mTextBox.UseCompatibleStateImageBehavior = false;
			this.mTextBox.View = System.Windows.Forms.View.Details;
			// 
			// mDateColumn
			// 
			this.mDateColumn.AspectName = "DateString";
			this.mDateColumn.Width = 120;
			// 
			// mNameColumn
			// 
			this.mNameColumn.AspectName = "Text";
			this.mNameColumn.FillsFreeSpace = true;
			// 
			// LogTextBox
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add( this.mTextBox );
			this.Name = "LogTextBox";
			this.Size = new System.Drawing.Size( 263, 139 );
			( ( System.ComponentModel.ISupportInitialize )( this.mTextBox ) ).EndInit();
			this.ResumeLayout( false );

		}

		#endregion

		private BrightIdeasSoftware.ObjectListView mTextBox;
		private BrightIdeasSoftware.OLVColumn mNameColumn;
		private BrightIdeasSoftware.OLVColumn mDateColumn;
	}
}

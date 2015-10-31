using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace AutoUSBBackup.GUI
{
	public partial class RemoveVolumeForm : Form
	{
		public bool DeleteAllData { get { return mRemoveDataCheckbox.Checked; } }


		public RemoveVolumeForm( string volumeName )
		{
			InitializeComponent();

			mLabel.Text = "Are you sure you wish to delete volume '" + volumeName + "'?";
		}

		private void mYesButton_Click( object sender, EventArgs e )
		{
			DialogResult = DialogResult.Yes;
			Close();
		}

		private void mNoButton_Click( object sender, EventArgs e )
		{
			DialogResult = DialogResult.No;
			Close();
		}
	}
}
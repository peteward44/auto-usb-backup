using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace AutoUSBBackup.GUI
{
	// Add local folder for backup
	public partial class AddNewVolumeControl2a : UserControl, FormUserControl
	{
		AddNewVolumeControlObject mObject = null;

		public AddNewVolumeControl2a()
		{
			InitializeComponent();

			mLocalFolderLabel.Text = "";
		}


		private void mButtonBack_Click( object sender, EventArgs e )
		{
			MainForm.Instance.ControlSwitcher.SwitchUserControl( FormControlType.AddVolume2, FormControlSwitchType.Back );
		}

		private void mButtonNext_Click( object sender, EventArgs e )
		{
			mObject.LocalBackupPath = mLocalFolderLabel.Text;
			MainForm.Instance.ControlSwitcher.SwitchUserControl( FormControlType.AddVolume1, FormControlSwitchType.Next, mObject );
		}


		public void OnControlHide( FormControlSwitchType switchType, object userObject )
		{
		}

		public void OnControlShow( FormControlSwitchType switchType, object userObject )
		{
			if (switchType == FormControlSwitchType.Next)
			{
				mObject = (AddNewVolumeControlObject)userObject;
			}
		}

		public Control GetControl() { return this; }


		private void mButtonBrowse_Click( object sender, EventArgs e )
		{
			FolderBrowserDialog fbd = new FolderBrowserDialog();
			fbd.ShowNewFolderButton = false;
			if ( fbd.ShowDialog() == DialogResult.OK )
			{
				mLocalFolderLabel.Text = fbd.SelectedPath;
				mButtonNext.Enabled = true;
			}
		}
	}
}

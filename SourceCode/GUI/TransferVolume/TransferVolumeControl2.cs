using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace AutoUSBBackup.GUI
{
	public partial class TransferVolumeControl2 : UserControl, FormUserControl
	{
		TransferVolumeControlObject mObject = null;


		public TransferVolumeControl2()
		{
			InitializeComponent();
		}


		public void OnControlHide( FormControlSwitchType switchType, object userObject )
		{
		}

		public void OnControlShow( FormControlSwitchType switchType, object userObject )
		{
			if ( switchType == FormControlSwitchType.Next )
			{
				System.Diagnostics.Debug.Assert( userObject is TransferVolumeControlObject && userObject != null );

				mObject = ( TransferVolumeControlObject )userObject;

				mLabel.Text = "WARNING: Contents of drive " + mObject.mNewUsbDrive.DriveId.DriveRootDirectory + " will be deleted! Are you sure?";
			}
		}


		public Control GetControl() { return this; }


		private void mButtonBack_Click(object sender, EventArgs e)
		{
			MainForm.Instance.ControlSwitcher.SwitchUserControl(FormControlType.TransferVolume1, FormControlSwitchType.Back);
		}

		private void mButtonNext_Click(object sender, EventArgs e)
		{
			MainForm.Instance.ControlSwitcher.SwitchUserControl( FormControlType.TransferVolume3, FormControlSwitchType.Next, mObject );
		}
	}
}

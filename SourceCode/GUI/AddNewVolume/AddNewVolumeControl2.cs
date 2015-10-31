using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace AutoUSBBackup.GUI
{
	public partial class AddNewVolumeControl2 : UserControl, FormUserControl
	{
		AddNewVolumeControlObject mObject = new AddNewVolumeControlObject();

		public AddNewVolumeControl2()
		{
			InitializeComponent();

		}

		private void mButtonBack_Click( object sender, EventArgs e )
		{
			MainForm.Instance.ControlSwitcher.SwitchUserControl( FormControlType.Welcome, FormControlSwitchType.Finish );
		}

		private void mButtonNext_Click( object sender, EventArgs e )
		{
			FormControlType nextForm = FormControlType.AddVolume2a;
			PWLib.FileSyncLib.VolumeType volType = PWLib.FileSyncLib.VolumeType.UsbDevice;
			if ( mRadio1.Checked )
			{
				volType = PWLib.FileSyncLib.VolumeType.UsbDevice;
				nextForm = FormControlType.AddVolume2b;
			}
			else if ( mRadio2.Checked )
			{
				volType = PWLib.FileSyncLib.VolumeType.LocalFolder;
				nextForm = FormControlType.AddVolume2a;
			}
			else if ( mAddExistingRadio.Checked )
			{
				nextForm = FormControlType.AddVolume2c;
			}

			mObject.VolumeType = volType;

			MainForm.Instance.ControlSwitcher.SwitchUserControl( nextForm, FormControlSwitchType.Next, mObject );
		}


		public void OnControlHide( FormControlSwitchType switchType, object userObject )
		{
		}

		public void OnControlShow( FormControlSwitchType switchType, object userObject )
		{
			if (switchType == FormControlSwitchType.Start)
			{
				mRadio1.Checked = true;
				mRadio2.Checked = false;
				mButtonNext.Enabled = true;
				mButtonBack.Enabled = true;
			}
		}

		public Control GetControl() { return this; }
	}
}

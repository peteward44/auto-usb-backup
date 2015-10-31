using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace AutoUSBBackup.GUI
{
	// Add USB device for backing up
	public partial class AddNewVolumeControl2b : UserControl, FormUserControl
	{
		AddNewVolumeControlObject mObject = null;

		public AddNewVolumeControl2b()
		{
			InitializeComponent();
			mUsbDeviceView.CheckedObjectChanged += new EventHandler( mUsbDeviceView_CheckedObjectChanged );
		}

		void mUsbDeviceView_CheckedObjectChanged( object sender, EventArgs e )
		{
			PWLib.UsbDrive.UsbDriveInfo o = mUsbDeviceView.SelectedDriveObject;
			mObject.UsbDrive = o;
			mButtonNext.Enabled = ( o != null );
		}


		private void mButtonBack_Click( object sender, EventArgs e )
		{
			MainForm.Instance.ControlSwitcher.SwitchUserControl( FormControlType.AddVolume2, FormControlSwitchType.Back );
		}

		private void mButtonNext_Click( object sender, EventArgs e )
		{
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
				mUsbDeviceView.RefreshDynamicList();
				mButtonNext.Enabled = false;
			}
		}

		public Control GetControl() { return this; }
	}
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace AutoUSBBackup.GUI
{
	public partial class TransferVolumeControl1 : UserControl, FormUserControl
	{
		TransferVolumeControlObject mObject = null;


		public TransferVolumeControl1()
		{
			InitializeComponent();

			mTreeView.DynamicItemVerifier = new AutoUSBBackup.GUI.Shared.UsbDriveObjectListView.ItemVerifierDelegate( VerifyUsbItem );
		}


		bool VerifyUsbItem( PWLib.UsbDrive.UsbDriveInfo driveInfo )
		{
			return false;
			//if ( mObject != null && mObject.mVolumeId != null )
			//{
			//    return !driveInfo.Equals( mObject.mVolumeId.Identifier.UsbDrive );
			//}
			//else
			//    return true;
		}


		public void OnControlHide( FormControlSwitchType switchType, object userObject )
		{
		}

		public void OnControlShow( FormControlSwitchType switchType, object userObject )
		{
			//if ( switchType == FormControlSwitchType.Start )
			//{
			//    mObject = new TransferVolumeControlObject();
			//    System.Diagnostics.Debug.Assert( userObject is VolumeIdentifier && userObject != null );
			//    mObject.mVolumeId = ( VolumeDescriptor )userObject;
			//    mLabel.Text = "Transfer volume '" + mObject.mVolumeId.VolumeName + "' to a new USB device...";
			//    mTreeView.RefreshDynamicList();
			//    mButtonNext.Enabled = false;
			//}
		}


		public Control GetControl() { return this; }


		private void mButtonBack_Click(object sender, EventArgs e)
		{
			MainForm.Instance.ControlSwitcher.SwitchUserControl(FormControlType.Welcome, FormControlSwitchType.Finish);
		}

		private void mButtonNext_Click(object sender, EventArgs e)
		{
			if ( mTreeView.SelectedDriveObject != null )
			{
				mObject.mNewUsbDrive = mTreeView.SelectedDriveObject;
				MainForm.Instance.ControlSwitcher.SwitchUserControl( FormControlType.TransferVolume2, FormControlSwitchType.Next, mObject );
			}
		}

		private void mTreeView_CheckedObjectChanged( object sender, EventArgs e )
		{
			mButtonNext.Enabled = mTreeView.SelectedDriveObject != null;
		}
	}
}

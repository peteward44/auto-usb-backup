using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using PWLib.FileSyncLib;


namespace AutoUSBBackup.GUI
{
	public partial class TransferVolumeControl3 : UserControl, FormUserControl
	{
		TransferVolumeControlObject mObject = null;
		bool mCancelledByUser = false;


		public TransferVolumeControl3()
		{
			InitializeComponent();
			MainForm.Instance.EventController.FormatUsbFinished += new EventHandler( Instance_FormatUsbFinished );
			MainForm.Instance.EventController.RestoreFinished += new VolumeRestoreHandler( Spine_RestoreFinished );
		}

		void Instance_FormatUsbFinished( object sender, EventArgs e )
		{
			// format finished, start restore onto drive
			mCancelButton.Enabled = true;
			MainForm.Instance.TransferVolume( mObject.mVolumeId, mObject.mNewUsbDrive );
			mLabel.Text = "Copying files to drive " + mObject.mNewUsbDrive.DriveId.DriveRootDirectory + "...";
		}


		void Spine_RestoreFinished( Volume volume, VolumeSnapshot snapshot )
		{
			mLabel.Text = mCancelledByUser ? "Transfer cancelled" : "Transfer complete";
			mButtonNext.Enabled = true;
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

				mLabel.Text = "Click 'Start Format' to delete the contents of " + mObject.mNewUsbDrive.DriveId.DriveRootDirectory;

				mButtonBack.Enabled = true;
				mCancelButton.Enabled = false;
				mButtonNext.Enabled = false;
				mCancelledByUser = false;
				mStartButton.Enabled = true;
			}
		}


		void StartFormat()
		{
			mButtonBack.Enabled = false;
			mLabel.Text = "Formatting drive " + mObject.mNewUsbDrive.DriveId.DriveRootDirectory + "...";
			MainForm.Instance.FormatUsbDrive( mObject.mNewUsbDrive );
		}


		public Control GetControl() { return this; }


		private void mButtonBack_Click(object sender, EventArgs e)
		{
			MainForm.Instance.ControlSwitcher.SwitchUserControl(FormControlType.TransferVolume2, FormControlSwitchType.Back);
		}

		private void mButtonNext_Click(object sender, EventArgs e)
		{
			MainForm.Instance.ControlSwitcher.SwitchUserControl( FormControlType.Welcome, FormControlSwitchType.Finish, mObject );
		}

		private void mCancelButton_Click( object sender, EventArgs e )
		{
			mCancelledByUser = true;
			mCancelButton.Enabled = false;
			MainForm.Instance.CancelOperation();
		}

		private void mStartButton_Click( object sender, EventArgs e )
		{
			mStartButton.Enabled = false;
			StartFormat();
		}
	}
}

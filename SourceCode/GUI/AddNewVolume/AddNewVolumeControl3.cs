using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace AutoUSBBackup.GUI
{
	public partial class AddNewVolumeControl3 : UserControl, FormUserControl
	{
		AddNewVolumeControlObject mObject = null;

		public AddNewVolumeControl3()
		{
			InitializeComponent();

			UpdateStorageLabelText();
		}


		int GetNumberOfRevisionsToKeep( int trackBarValue )
		{
			int[] RevisionCount = { 1, 2, 3, 4, 5, 10, 20, 30, 50, 100, -1 }; // -1 is all
			System.Diagnostics.Debug.Assert( trackBarValue >= 0 && trackBarValue < RevisionCount.Length );
			return RevisionCount[ trackBarValue ];
		}


		void UpdateStorageLabelText()
		{
			int revisionsToKeep = GetNumberOfRevisionsToKeep( mTrackBarStorage.Value );
			switch ( revisionsToKeep )
			{
				case 1:
					mStorageLabel.Text = "Keep only the most recent revision (minimal hard disk space used)";
					break;
				case -1:
					mStorageLabel.Text = "Keep all revisions (maximum hard disk space used)";
					break;
				default:
					mStorageLabel.Text = "Keep the last " + revisionsToKeep + " revisions";
					break;
			}
			if ( mObject != null )
				mObject.RevisionsToKeep = revisionsToKeep;
		}

		private void mButtonBack_Click( object sender, EventArgs e )
		{
			MainForm.Instance.ControlSwitcher.SwitchUserControl( FormControlType.AddVolume1, FormControlSwitchType.Back );
		}

		private void mButtonNext_Click( object sender, EventArgs e )
		{
			mObject.StoragePath = mBackupToLabel.Text;
			MainForm.Instance.ControlSwitcher.SwitchUserControl( FormControlType.AddVolume4, FormControlSwitchType.Next, mObject );
		}

		private void mTrackBarStorage_ValueChanged( object sender, EventArgs e )
		{
			UpdateStorageLabelText();
		}

		public void OnControlHide( FormControlSwitchType switchType, object userObject )
		{
		}

		public void OnControlShow( FormControlSwitchType switchType, object userObject )
		{
			if ( switchType == FormControlSwitchType.Next )
			{
				mObject = ( AddNewVolumeControlObject )userObject;
				mBackupToLabel.Text = Config.Active.DefaultRootDataDirectory + PWLib.Platform.Windows.Path.DirectorySeparatorChar + mObject.Name;
			}
		}

		public Control GetControl() { return this; }


		private void mButtonBrowse_Click( object sender, EventArgs e )
		{
			FolderBrowserDialog fbd = new FolderBrowserDialog();
			fbd.ShowNewFolderButton = true;
			if ( fbd.ShowDialog() == DialogResult.OK )
			{
				mBackupToLabel.Text = fbd.SelectedPath;
			}
		}
	}
}

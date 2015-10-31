using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace AutoUSBBackup.GUI
{
	public partial class RestoreVolumesControl2 : UserControl, FormUserControl
	{
		RestoreVolumesControlObject mObject = null;

		public RestoreVolumesControl2()
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
				mObject = ( RestoreVolumesControlObject )userObject;

				Init();
			}
		}


		public Control GetControl() { return this; }


		private void mButtonBack_Click(object sender, EventArgs e)
		{
			MainForm.Instance.ControlSwitcher.SwitchUserControl(FormControlType.RestoreVolume1, FormControlSwitchType.Back);
		}

		private void mButtonNext_Click(object sender, EventArgs e)
		{
			MainForm.Instance.RestoreVolume( mObject.mVolumeId, mObject.mRevision, mObject.mOutputPath );
			MainForm.Instance.ControlSwitcher.SwitchUserControl(FormControlType.Welcome, FormControlSwitchType.Finish, mObject );
		}


		void Init()
		{
			SetSelectedPath( Environment.GetFolderPath( Environment.SpecialFolder.DesktopDirectory ) );
		}


		void SetSelectedPath( string path )
		{
			mObject.mOutputPath = path;
			mRestoreFileNameLabel.Text = path;
		}


		private void mButtonBrowse_Click( object sender, EventArgs e )
		{
			FolderBrowserDialog outputDialog = new FolderBrowserDialog();
			outputDialog.ShowNewFolderButton = true;
			if ( outputDialog.ShowDialog() == DialogResult.OK )
			{
				SetSelectedPath( outputDialog.SelectedPath );
			}
		}
	}
}

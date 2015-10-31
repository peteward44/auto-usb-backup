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
	public partial class AddNewVolumeControl4 : UserControl, FormUserControl
	{
		AddNewVolumeControlObject mObject = null;

		VolumeSource CreateSource()
		{
			switch ( mObject.VolumeType )
			{
				case VolumeType.LocalFolder:
					{
						return new VolumeLocalFolderSource( mObject.LocalBackupPath );
					}
				case VolumeType.UsbDevice:
					{
						return new VolumeUsbSource( mObject.UsbDrive );
					}
			}
			throw new Exception( "Unsupported volume type" );
		}

		BaseArchive CreateArchive()
		{
			return new OpenArchive( mObject.StoragePath );
		}



		public AddNewVolumeControl4()
		{
			InitializeComponent();
		}

		private void mButtonBack_Click( object sender, EventArgs e )
		{
			MainForm.Instance.ControlSwitcher.SwitchUserControl( FormControlType.AddVolume3, FormControlSwitchType.Back );
		}

		private void mButtonNext_Click( object sender, EventArgs e )
		{
			try
			{
				VolumeSource source = CreateSource();
				BaseArchive archive = CreateArchive();

				string vidFilename = mObject.StoragePath + PWLib.Platform.Windows.Path.DirectorySeparatorChar + mObject.Name + ".vol";

				VolumeDescriptor vdesc = VolumeDescriptorList.Instance.AddNewDescriptor( mObject.Name, vidFilename, source, archive );
				if ( mObject.RevisionsToKeep > 0 )
					vdesc.RevisionsToKeep = mObject.RevisionsToKeep;
				else
					vdesc.TimePeriodToMonitor = mObject.TimePeriodToKeep;

				if ( mStartBackupNow.Checked )
				{
					Log.WriteLine( LogType.TextLogVerbose, "Backing up volume by adding it through GUI" );
					MainForm.Instance.BackupVolume( vdesc );
				}
			}
			catch ( System.Exception ex )
			{
				Log.WriteException( "Add new volume failed", ex );
			}

			MainForm.Instance.ControlSwitcher.SwitchUserControl( FormControlType.Welcome, FormControlSwitchType.Finish );
		}

		public void OnControlHide( FormControlSwitchType switchType, object userObject )
		{
		}

		public void OnControlShow( FormControlSwitchType switchType, object userObject )
		{
			mObject = (AddNewVolumeControlObject)userObject;
			if ( switchType == FormControlSwitchType.Next )
			{
				mLabel.Text = "Creating new volume " + mObject.Name + "\r\n";
				mStartBackupNow.Checked = true;
			}
		}

		public Control GetControl() { return this; }
	}
}

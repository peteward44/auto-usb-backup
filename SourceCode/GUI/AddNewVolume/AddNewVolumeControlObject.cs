using System;
using System.Collections.Generic;
using System.Text;

namespace AutoUSBBackup.GUI
{
	public class AddNewVolumeControlObject
	{
		string mName = "";
		public string Name { get { return mName; } set { mName = value; } }

		//VolumeIdentifier mExistingVID = null;
		//public VolumeIdentifier ExistingVID { get { return mExistingVID; } set { mExistingVID = value; } }

		PWLib.FileSyncLib.VolumeType mVolumeType = PWLib.FileSyncLib.VolumeType.UsbDevice;
		public PWLib.FileSyncLib.VolumeType VolumeType { get { return mVolumeType; } set { mVolumeType = value; } }

		PWLib.UsbDrive.UsbDriveInfo mUsbDriveInfo = null;
		public PWLib.UsbDrive.UsbDriveInfo UsbDrive { get { return mUsbDriveInfo; } set { mUsbDriveInfo = value; } }

		int mRevisionsToKeep = 5;
		public int RevisionsToKeep { get { return mRevisionsToKeep; } set { mRevisionsToKeep = value; } }

		TimeSpan mTimePeriodToKeep;
		public TimeSpan TimePeriodToKeep { get { return mTimePeriodToKeep; } set { mTimePeriodToKeep = value; } }

		string mLocalBackupPath;
		public string LocalBackupPath { get { return mLocalBackupPath; } set { mLocalBackupPath = value; } }

		string mBackupToPath;
		public string StoragePath { get { return mBackupToPath; } set { mBackupToPath = value; } }
		

		public AddNewVolumeControlObject()
		{
		}

	}
}

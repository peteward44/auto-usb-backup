using System;
using System.Collections.Generic;
using System.Text;
using PWLib.FileSyncLib;


namespace AutoUSBBackup.GUI
{
	public class RestoreVolumesControlObject
	{
		public VolumeSnapshotRevision mRevision = null;
		public string mOutputPath = "";
		public VolumeDescriptor mVolumeId;


		public RestoreVolumesControlObject()
		{
		}
	}
}

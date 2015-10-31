using System;
using System.Collections.Generic;
using System.Text;

namespace AutoUSBBackup.GUI
{
	public class TransferVolumeControlObject
	{
		public VolumeDescriptor mVolumeId;
		public PWLib.UsbDrive.UsbDriveInfo mNewUsbDrive = null;


		public TransferVolumeControlObject()
		{
		}
	}
}

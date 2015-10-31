using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using System.Threading;

// Main 'spine' of the program, to keep it seperate from the GUI

namespace AutoUSBBackup
{

	public class Spine : IDisposable
	{
		VolumeEventController mEventController;
		bool mInitialised = false;

		public bool Initialised { get { return mInitialised; } }


		public Spine( VolumeEventController eventController  )
		{
			mEventController = eventController;
		}



		public void Init()
		{
			mInitialised = true;
		}


		public void Dispose()
		{
			VolumeDescriptorList.Instance.Dispose();
		}


		public void BackupVolume( Volume volume, bool firstBackupOnLoad )
		{
			if ( volume.Descriptor.ReadyForBackup )
				volume.DoBackup();
		}


		public void CancelOperation()
		{
			lock ( VolumeDescriptorList.Instance.Descriptors )
			{
				foreach ( VolumeDescriptor desc in VolumeDescriptorList.Instance.Descriptors )
				{
					if ( desc.IsAvailable && desc.Volume != null )
						desc.Volume.CancelOperation( true );
				}
			}
		}


		public void FormatUsbDrive( PWLib.UsbDrive.UsbDriveInfo driveInfo )
		{
			string rootDir = driveInfo.DriveId.DriveRootDirectory;
			Log.WriteLine( LogType.All, "Commencing format of usb drive " + rootDir + "..." );
			PWLib.Platform.Windows.Path.DeletePath( rootDir, true );

			mEventController.InvokeFormatUsbFinished();

			Log.WriteLine( LogType.All, "Format complete" );
		}


		public void RemoveVolume( VolumeDescriptor desc, bool removeAllData )
		{
			if ( removeAllData && desc.IsAvailable )
			{
				desc.Volume.DeleteAllRevisions();
			}
			VolumeDescriptorList.Instance.RemoveDescriptor( desc );
		}

	}
}

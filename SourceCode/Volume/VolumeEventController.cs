using System;
using System.Collections.Generic;
using System.Text;
using PWLib.FileSyncLib;



namespace AutoUSBBackup
{
	public delegate void VolumeBackupHandler( Volume volume, VolumeSnapshot snapshot, bool snapshotHasBeenSaved, bool firstBackupOnLoad );
	public delegate void VolumeBackupProgressHandler( Volume volume, VolumeSnapshot snapshot, ulong fileNum, ulong fileCount, ulong bytesTranferred, ulong totalBytes );

	public delegate void VolumeRestoreHandler( Volume volume, VolumeSnapshot snapshot );
	public delegate void VolumeRestoreProgressHandler( Volume volume, VolumeSnapshot snapshot, ulong fileNum, ulong fileCount, ulong bytesTranferred, ulong totalBytes );

	public delegate void VolumeDescriptorChangedHandler( VolumeDescriptor volume, bool isActive );
	public delegate void VolumeSourceActiveChangedHandler( Volume volume, bool isActive );

	public delegate void USBDriveEventHandler( PWLib.UsbDrive.UsbDriveInfo usbDrive );


	public class VolumeEventController
	{
		#region Application Init

		public void InvokeApplicationInitialised( object sender )
		{
			if ( ApplicationInitialised != null )
				ApplicationInitialised( sender, null );
		}
		public event EventHandler ApplicationInitialised;

		#endregion

		#region Volume active state changes

		public void InvokeVolumeDescriptorActiveStateChanged( VolumeDescriptor volume, bool isActive )
		{
			if ( VolumeDescriptorActiveStateChanged != null )
				VolumeDescriptorActiveStateChanged( volume, isActive );
		}
		public event VolumeDescriptorChangedHandler VolumeDescriptorActiveStateChanged;

		public void InvokeVolumeSourceActiveStateChanged( Volume volume, bool isActive )
		{
			if ( VolumeSourceActiveStateChanged != null )
				VolumeSourceActiveStateChanged( volume, isActive );
		}
		public event VolumeSourceActiveChangedHandler VolumeSourceActiveStateChanged;

		#endregion


		#region USB

		public void InvokeFormatUsbFinished()
		{
			if ( FormatUsbFinished != null )
				FormatUsbFinished( null, null );
		}
		public event EventHandler FormatUsbFinished;


		#endregion

		#region Backup

		public void InvokeBackupStarted( Volume volume, VolumeSnapshot snapshot, bool firstBackupOnLoad )
		{
			if ( BackupStarted != null )
				BackupStarted( volume, snapshot, false, firstBackupOnLoad );
		}
		public event VolumeBackupHandler BackupStarted;

		public void InvokeBackupProgress( Volume volume, VolumeSnapshot snapshot, ulong fileNum, ulong fileCount, ulong bytesTranferred, ulong totalBytes )
		{
			if ( BackupProgress != null )
				BackupProgress( volume, snapshot, fileNum, fileCount, bytesTranferred, totalBytes );
		}
		public event VolumeBackupProgressHandler BackupProgress;

		public void InvokeBackupFinished( Volume volume, VolumeSnapshot snapshot, bool snapshotHasBeenSaved, bool firstBackupOnLoad )
		{
			if ( BackupFinished != null )
				BackupFinished( volume, snapshot, snapshotHasBeenSaved, firstBackupOnLoad );
		}
		public event VolumeBackupHandler BackupFinished;

		#endregion

		#region Restore


		public void InvokeRestoreProgress( Volume volume, VolumeSnapshot snapshot, ulong fileNum, ulong fileCount, ulong bytesTranferred, ulong totalBytes )
		{
			if ( RestoreProgress != null )
				RestoreProgress( volume, snapshot, fileNum, fileCount, bytesTranferred, totalBytes );
		}
		public event VolumeRestoreProgressHandler RestoreProgress;

		public void InvokeRestoreStarted( Volume volume, VolumeSnapshot snapshot )
		{
			if ( RestoreStarted != null )
				RestoreStarted( volume, snapshot );
		}
		public event VolumeRestoreHandler RestoreStarted;

		public void InvokeRestoreFinished( Volume volume, VolumeSnapshot snapshot )
		{
			if ( RestoreFinished != null )
				RestoreFinished( volume, snapshot );
		}
		public event VolumeRestoreHandler RestoreFinished;

		#endregion
	}
}



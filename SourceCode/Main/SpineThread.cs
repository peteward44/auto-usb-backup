using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using PWLib.FileSyncLib;



namespace AutoUSBBackup
{
	public class SpineThread : IDisposable
	{
		VolumeEventController mVolumeEventController = new VolumeEventController();
		PWLib.BackgroundWorkerThread mWorkerThread;

		Spine mSpine = null;

		class QueuedRestoreInfo
		{
			public VolumeDescriptor mDescriptor = null;
			public VolumeSnapshotRevision mVolumeRevision = null;
			public string mDiskPath = "";
		}

		class QueuedBackupInfo
		{
			public Volume mVolume = null;
			public bool mFirstBackupOnLoad = false;
		}

		class QueuedTransferInfo
		{
			public VolumeDescriptor mDescriptor = null;
			public PWLib.UsbDrive.UsbDriveInfo mNewDrive = null;
		}

		class QueuedRemoveInfo
		{
			public VolumeDescriptor mDescriptor = null;
			public bool mDeleteAllData = false;
		}


		volatile bool mForceFullBackup = false;
		bool mFirstFullBackupSinceLoad = true;

		DateTime mTimeOfLastBackupOfAllActiveVolumes = DateTime.Now;


		public Spine Spine { get { return mSpine; } }
		public VolumeEventController EventController { get { return mVolumeEventController; } }
		public bool Disposed { get { return mWorkerThread.Disposed; } }

		public DateTime TimeOfLastBackup { get { return mTimeOfLastBackupOfAllActiveVolumes; } }
		public DateTime TimeForNextBackup { get { return mTimeOfLastBackupOfAllActiveVolumes + Config.Active.BackupInterval; } }
		public TimeSpan NextBackupDueIn { get { return TimeForNextBackup - DateTime.Now; } }


		public SpineThread()
		{
			mSpine = new Spine( mVolumeEventController );
	
			VolumeSnapshotDirectory.IgnoredDirectoryNames.Add( "nobackup" );
			VolumeSnapshotDirectory.IgnoredDirectoryNames.Add( "System Volume Information" );

			mWorkerThread = new PWLib.BackgroundWorkerThread( 100 );
			mWorkerThread.InitialisationTask += new PWLib.BackgroundWorkerThread.Task(OnWorkerThreadInit);
			mWorkerThread.ShutdownTask += new PWLib.BackgroundWorkerThread.Task(OnWorkerShutdown);

			mVolumeEventController.VolumeDescriptorActiveStateChanged += new VolumeDescriptorChangedHandler( mVolumeEventController_VolumeDescriptorActiveStateChanged );
			mVolumeEventController.VolumeSourceActiveStateChanged += new VolumeSourceActiveChangedHandler( mVolumeEventController_VolumeSourceActiveStateChanged );

			mWorkerThread.RegisterPersistentTask( new PWLib.BackgroundWorkerThread.Task( OnWorkerPersistentTask ), null );
		}


		void mVolumeEventController_VolumeDescriptorActiveStateChanged( VolumeDescriptor desc, bool isActive )
		{
			if ( desc.ReadyForBackup )
			{
				Log.WriteLine( LogType.TextLogVerbose, "Backing up volume as the descriptor (.vol) file state has recently changed" );
				ForceBackup( desc.Volume );
			}
		}


		void mVolumeEventController_VolumeSourceActiveStateChanged( Volume volume, bool isActive )
		{
			// Find relevant VolumeSource, then do a backup (if enabled on that particular volume)
			try
			{
				if ( isActive )
				{
					lock ( VolumeDescriptorList.Instance.Descriptors )
					{
						if ( volume.Descriptor.ReadyForBackup )
						{
							// kick off first insert backup
							Log.WriteLine( LogType.TextLogVerbose, "Backing up volume by volume source state change (normally usb stick being plugged in)" );
							ForceBackup( volume );
						}
					}
				}
			}
			catch ( System.Exception e )
			{
				Log.WriteException( "DeviceInserted failed", e );
			}
		}


		public void Start()
		{
			mWorkerThread.Start();
		}


		void OnWorkerThreadInit( object userObject )
		{
			mTimeOfLastBackupOfAllActiveVolumes = DateTime.Now;

			mSpine.Init();

			mVolumeEventController.InvokeApplicationInitialised( this );

			mForceFullBackup = true; // Do a full backup on startup on any inserted usb drives.

			// TODO: Replace with this with something that can potentially cause an RC
			System.Threading.Thread.Sleep( 5000 ); // wait 5 seconds after load before performing any backups to give time to load form
		}


		void OnWorkerPersistentTask( object userObject )
		{
			VolumeDescriptorList.Instance.CheckAvailability();

			if ( !VolumeDescriptorList.Instance.AnyBusyVolumes )
			{
				DateTime testDT = DateTime.Now;
				if ( mTimeOfLastBackupOfAllActiveVolumes + Config.Active.BackupInterval <= testDT || mForceFullBackup )
				{
					mForceFullBackup = false;
					lock ( VolumeDescriptorList.Instance.Descriptors )
					{
						foreach ( VolumeDescriptor desc in VolumeDescriptorList.Instance.Descriptors )
						{
							if ( desc.ReadyForBackup )
							{
								Log.WriteLine( LogType.TextLogVerbose, "Adding intervaled backup to queue (" + desc.VolumeName + ")" );
								ForceBackup( desc.Volume, mFirstFullBackupSinceLoad );
							}
						}
					}
					mFirstFullBackupSinceLoad = false;
					mTimeOfLastBackupOfAllActiveVolumes = testDT;
				}
			}
		}


		public void CancelOperation()
		{
			if ( mSpine != null )
				mSpine.CancelOperation();
		}


		void OnWorkerShutdown( object userObject )
		{
			mSpine.Dispose();
		}


		public void Dispose()
		{
			mWorkerThread.Dispose();
		}


		public bool WndProc( ref Message m )
		{
			if ( PWLib.UsbDrive.UsbDriveList.Instance != null )
				return PWLib.UsbDrive.UsbDriveList.Instance.WndProc( ref m );
			else
				return true;
		}


		public void ForceFullBackup()
		{
			mForceFullBackup = true;
		}


		public void ForceBackup( Volume volume )
		{
			ForceBackup( volume, false );
		}

		public void ForceBackup( Volume volume, bool firstBackupOnLoad )
		{
			if ( volume.Descriptor.ReadyForBackup )
			{
				Log.WriteLine( LogType.TextLogVerbose, "Adding QueuedBackup (" + volume.Descriptor.VolumeName + ")" );

				QueuedBackupInfo qbi = new QueuedBackupInfo();
				qbi.mVolume = volume;
				qbi.mFirstBackupOnLoad = firstBackupOnLoad;

				mWorkerThread.RegisterOneShotTask( new PWLib.BackgroundWorkerThread.TaskPredicate( OnWorkerPredicate ),
					new PWLib.BackgroundWorkerThread.Task( OnWorkerBackup ), new PWLib.BackgroundWorkerThread.TaskError( OnWorkerError ), qbi );
			}
		}


		bool OnWorkerPredicate( object userObject )
		{
			return !VolumeDescriptorList.Instance.AnyBusyVolumes;
		}


		void OnWorkerBackup( object backupInfo )
		{
			QueuedBackupInfo backupVolume = (QueuedBackupInfo)backupInfo;
			Log.WriteLine( LogType.TextLogVerbose, "Processing QueuedBackup (" + backupVolume.mVolume.Descriptor.VolumeName + ")" );
			mSpine.BackupVolume( backupVolume.mVolume, backupVolume.mFirstBackupOnLoad );
		}


		void OnWorkerError( object userObject, Exception e )
		{
			Log.WriteException( "Spine.StartThread failed", e );
		}


		public void TransferVolume( VolumeDescriptor vid, PWLib.UsbDrive.UsbDriveInfo newDrive )
		{
			QueuedTransferInfo transferInfo = new QueuedTransferInfo();
			transferInfo.mDescriptor = vid;
			transferInfo.mNewDrive = newDrive;
			mWorkerThread.RegisterOneShotTask( new PWLib.BackgroundWorkerThread.TaskPredicate( OnWorkerPredicate ),
				new PWLib.BackgroundWorkerThread.Task( OnWorkerTransfer ), new PWLib.BackgroundWorkerThread.TaskError( OnWorkerError ), transferInfo );
		}


		void OnWorkerTransfer( object userObject )
		{
		}


		public void RestoreVolume( VolumeDescriptor vid, VolumeSnapshotRevision revision, string onDiskPath )
		{
			if ( vid.ReadyForRestore )
			{
				Log.WriteLine( LogType.TextLogVerbose, "Adding QueuedRestore (" + vid.VolumeName + ")" );

				QueuedRestoreInfo restoreInfo = new QueuedRestoreInfo();
				restoreInfo.mDescriptor = vid;
				restoreInfo.mVolumeRevision = revision;
				restoreInfo.mDiskPath = onDiskPath;
				mWorkerThread.RegisterOneShotTask( new PWLib.BackgroundWorkerThread.TaskPredicate( OnWorkerPredicate ),
					new PWLib.BackgroundWorkerThread.Task( OnWorkerRestore ), new PWLib.BackgroundWorkerThread.TaskError( OnWorkerError ), restoreInfo );
			}
		}


		void OnWorkerRestore( object userObject )
		{
			QueuedRestoreInfo restoreInfo = (QueuedRestoreInfo)userObject;
			Log.WriteLine( LogType.TextLogVerbose, "Processing QueuedRestore (" + restoreInfo.mDescriptor.VolumeName + ")" );
			if ( restoreInfo.mDescriptor.IsAvailable )
				restoreInfo.mDescriptor.Volume.Restore( restoreInfo.mVolumeRevision, restoreInfo.mDiskPath );
		}


		public void FormatUsbDrive( PWLib.UsbDrive.UsbDriveInfo driveInfo )
		{
			mWorkerThread.RegisterOneShotTask( new PWLib.BackgroundWorkerThread.TaskPredicate( OnWorkerPredicate ),
				new PWLib.BackgroundWorkerThread.Task( OnWorkerUsbFormat ), new PWLib.BackgroundWorkerThread.TaskError( OnWorkerError ), driveInfo );
		}


		void OnWorkerUsbFormat( object userObject )
		{
			PWLib.UsbDrive.UsbDriveInfo di = (PWLib.UsbDrive.UsbDriveInfo)userObject;
			mSpine.FormatUsbDrive( di );
		}


		public void RemoveVolume( VolumeDescriptor desc, bool removeAllData )
		{
			QueuedRemoveInfo removeInfo = new QueuedRemoveInfo();
			removeInfo.mDescriptor = desc;
			removeInfo.mDeleteAllData = removeAllData;
			mWorkerThread.RegisterOneShotTask( new PWLib.BackgroundWorkerThread.TaskPredicate( OnWorkerPredicate ),
				new PWLib.BackgroundWorkerThread.Task( OnWorkerRemove ), new PWLib.BackgroundWorkerThread.TaskError( OnWorkerError ), removeInfo );
		}


		void OnWorkerRemove( object userObject )
		{
			QueuedRemoveInfo removeInfo = (QueuedRemoveInfo)userObject;
			Log.WriteLine( LogType.TextLogVerbose, "Processing QueuedRemove (" + removeInfo.mDescriptor.VolumeName + ")" );
			mSpine.RemoveVolume( removeInfo.mDescriptor, removeInfo.mDeleteAllData );
		}
	}

}

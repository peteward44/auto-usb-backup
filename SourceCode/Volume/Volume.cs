using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using PWLib.FileSyncLib;
using System.Xml;


namespace AutoUSBBackup
{
	public class Volume : IDisposable
	{
		public override bool Equals( object obj )
		{
			return this.mVolumeDesc.Equals( obj );
		}

		public override int GetHashCode()
		{
			return this.mVolumeDesc.GetHashCode();
		}



		public void OutputToXml( XmlTextWriter xmlWriter )
		{
			mBackupRestoreObject.OutputToXml( xmlWriter );
		}


		public static Volume BuildFromXml( VolumeEventController eventController, VolumeDescriptor volumeDesc, XmlNode parentNode )
		{
			return new Volume( eventController, volumeDesc, parentNode );
		}


		public delegate void OutputToXmlRequiredDelegate( Volume volume );
		public event OutputToXmlRequiredDelegate OutputToXmlRequired;

		VolumeEventController mEventController;
		VolumeDescriptor mVolumeDesc;
		BackupRestoreVolume mBackupRestoreObject;

		volatile bool mBusy = false;
		volatile bool mBackupInProgress = false;
		volatile bool mDisposed = false;

		public VolumeDescriptor Descriptor { get { return mVolumeDesc; } }
		public bool Busy { get { return mBusy; } }
		public bool BackupInProgress { get { return mBackupInProgress; } }
		public bool Disposed { get { return mDisposed; } }

		public VolumeSnapshotDatabase Database { get { return mBackupRestoreObject.Database; } }
		public VolumeSource Source { get { return mBackupRestoreObject.Source; } }
		public BaseArchive Archive { get { return mBackupRestoreObject.Archive; } }


		public override string ToString()
		{
			return mVolumeDesc.ToString();
		}


		void Init()
		{
			mBackupRestoreObject.Source.MediaAvailableChanged += new VolumeSource.MediaAvailableChangedDelegate( Source_MediaAvailableChanged );
			mBackupRestoreObject.BackupProgress += new BackupRestoreVolume.OperationProgressDelegate( mBackupRestoreObject_BackupProgress );
			mBackupRestoreObject.RestoreProgress += new BackupRestoreVolume.OperationProgressDelegate( mBackupRestoreObject_RestoreProgress );
		}


		void Source_MediaAvailableChanged( VolumeSource source, bool isAvailable )
		{
			mEventController.InvokeVolumeSourceActiveStateChanged( this, isAvailable );
		}


		void mBackupRestoreObject_BackupProgress( BackupRestoreVolume volume, ulong fileNum, ulong fileCount, ulong bytesTranferred, ulong totalBytes )
		{
			mEventController.InvokeBackupProgress( this, null, fileNum, fileCount, bytesTranferred, totalBytes );
		}

		void mBackupRestoreObject_RestoreProgress( BackupRestoreVolume volume, ulong fileNum, ulong fileCount, ulong bytesTranferred, ulong totalBytes )
		{
			mEventController.InvokeRestoreProgress( this, null, fileNum, fileCount, bytesTranferred, totalBytes );
		}



		public void DeleteAllRevisions()
		{
			mBackupRestoreObject.DeleteAllRevisions();
		}


		public Volume( VolumeEventController eventController, VolumeDescriptor volumeDesc, VolumeSource source, BaseArchive archive )
		{
			mEventController = eventController;
			mVolumeDesc = volumeDesc;
			mBackupRestoreObject = new BackupRestoreVolume( mVolumeDesc.VolumeName, source, archive );
			Init();
		}


		public Volume( VolumeEventController eventController, VolumeDescriptor volumeDesc, XmlNode parentNode )
		{
			mEventController = eventController;
			mVolumeDesc = volumeDesc;
			mBackupRestoreObject = new BackupRestoreVolume( parentNode );
			Init();
		}


		public void Dispose()
		{
			mDisposed = true;
			mBackupRestoreObject.Dispose();
		}



		public void Restore( VolumeSnapshotRevision revision, string onDiskPath )
		{
			try
			{
				mBusy = true;
				mEventController.InvokeRestoreStarted( this, null );
				mBackupRestoreObject.Restore( revision, onDiskPath );
			}
			finally
			{
				mBusy = false;
				mEventController.InvokeRestoreFinished( this, null );
			}
		}


		public void DoBackup()
		{
			bool outputToXmlRequired = false;

			try
			{
				mBusy = true;
				mBackupInProgress = true;

				Log.WriteLine( LogType.All, "Commencing backup of volume '" + mVolumeDesc.VolumeName + "'" );

				mVolumeDesc.LastAttemptedBackup = DateTime.Now;
				mEventController.InvokeBackupStarted( this, null, false );

				bool backupSuccess = mBackupRestoreObject.DoBackup( ref outputToXmlRequired );

				if ( backupSuccess && outputToXmlRequired )
				{
					Log.WriteLine( LogType.TextLogVerbose, "XML update required, calculating database total size..." );

					CalculateTotalDatabaseSize();

					Log.WriteLine( LogType.TextLogVerbose, "Total database on-disk size " + mVolumeDesc.TotalDatabaseSize.ToString() );

					if ( mVolumeDesc.RevisionsToKeep > 0 )
					{
						Log.WriteLine( LogType.TextLogVerbose, "Merging old snapshot revisions by revision count (" + mVolumeDesc.RevisionsToKeep + ")" );
						mBackupRestoreObject.MergeSnapshotRevisionsByLimitedRevisionCount( mVolumeDesc.RevisionsToKeep );
						Log.WriteLine( LogType.TextLogVerbose, "Merge complete" );
					}
					else
					{
						Log.WriteLine( LogType.TextLogVerbose, "Merging old snapshot revisions by time period (" + mVolumeDesc.TimePeriodToMonitor.ToString() + ")" );
						mBackupRestoreObject.MergeSnapshotRevisionsByTimeLimit( mVolumeDesc.TimePeriodToMonitor );
						Log.WriteLine( LogType.TextLogVerbose, "Merge complete" );
					}

					if ( OutputToXmlRequired != null )
						OutputToXmlRequired( this );
				}

				if ( backupSuccess )
				{
					Log.WriteLine( LogType.All, "Successfully backed up '" + mVolumeDesc.VolumeName + "'" + (outputToXmlRequired ? " [Changes]" : " [No changes necessary]") );
				}
				else
				{
					Log.WriteLine( LogType.All, "Back up " + (mBackupRestoreObject.IsUserCancelled ? "cancelled" : "failed") );
				}
			}
			catch ( Exception e )
			{
				Log.WriteException( "Error during backup", e, true );
			}
			finally
			{
				mBackupInProgress = false;
				mBusy = false;
				mEventController.InvokeBackupFinished( this, null, outputToXmlRequired, false );
			}
		}


		public void CancelOperation( bool cancelByUser )
		{
			mBackupRestoreObject.CancelOperation( cancelByUser );
		}


		void CalculateTotalDatabaseSize()
		{
			ulong totalSize = 0;
			foreach ( VolumeSnapshotRevision revision in mBackupRestoreObject.Database.GetRevisionHistory() )
			{
				VolumeSnapshot snapshot = mBackupRestoreObject.Database.LoadSnapshotRevision( mBackupRestoreObject.Source, revision );
				totalSize += snapshot.Root.DetermineTotalSizeOfStoredFiles();
			}
			mVolumeDesc.TotalDatabaseSize = totalSize;
		}

	}
}



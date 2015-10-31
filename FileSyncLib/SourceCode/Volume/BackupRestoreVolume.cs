using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Xml;


namespace PWLib.FileSyncLib
{
	
	public class BackupRestoreVolume : IDisposable
	{
		string mName;
		VolumeSnapshotDatabase mDatabase;
		VolumeSource mSource;
		BaseArchive mArchive;
		bool mCancelledByUser = false;

		public VolumeSnapshotDatabase Database { get { return mDatabase; } }
		public VolumeSource Source { get { return mSource; } }
		public BaseArchive Archive { get { return mArchive; } }

		bool mBusy = false;
		public bool Busy { get { return mBusy; } }


		VolumeBackupOperation mBackupOperation = new VolumeBackupOperation();
		VolumeRestoreOperation mRestoreOperation = new VolumeRestoreOperation();
		VolumeSnapshotMergeOperation mMergeOperation = new VolumeSnapshotMergeOperation();

		public delegate void OperationProgressDelegate( BackupRestoreVolume volume, ulong fileNum, ulong fileCount, ulong bytesTranferred, ulong totalBytes );

		public event OperationProgressDelegate BackupProgress;
		public event OperationProgressDelegate RestoreProgress;


		public bool IsUserCancelled
		{
			get { return mCancelledByUser; }
		}


		public override string ToString()
		{
			return mName;
		}


		public void CancelOperation( bool cancelByUser )
		{
			mCancelledByUser = cancelByUser;
			mBackupOperation.Cancel( cancelByUser );
			mRestoreOperation.Cancel( cancelByUser );
		}


		void Init()
		{
			mBackupOperation.Progress += new VolumeBackupOperation.OperationProgressDelegate( mOperation_BackupProgress );
			mRestoreOperation.Progress += new VolumeRestoreOperation.OperationProgressDelegate( mOperation_RestoreProgress );
		}


		void mOperation_BackupProgress( ulong fileNum, ulong fileCount, ulong bytesTranferred, ulong totalBytes )
		{
			if ( BackupProgress != null )
				BackupProgress( this, fileNum, fileCount, bytesTranferred, totalBytes );
		}

		void mOperation_RestoreProgress( ulong fileNum, ulong fileCount, ulong bytesTranferred, ulong totalBytes )
		{
			if ( RestoreProgress != null )
				RestoreProgress( this, fileNum, fileCount, bytesTranferred, totalBytes );
		}


		public BackupRestoreVolume( string name, VolumeSource source, BaseArchive archive )
		{
			mName = name;
			mSource = source;
			mArchive = archive;
			mDatabase = new VolumeSnapshotDatabase( archive.GetSnapshotXmlDir() );
			Init();
		}


		public BackupRestoreVolume( XmlNode parentNode )
		{
			mName = PWLib.XmlHelp.GetAttribute( parentNode, "name", "" );
			mSource = VolumeSource.BuildFromXml( parentNode );
			mArchive = BaseArchive.BuildFromXml( parentNode );
			mDatabase = VolumeSnapshotDatabase.LoadFromXml( parentNode );
			Init();
		}



		public void OutputToXml( XmlTextWriter xmlWriter )
		{
			xmlWriter.WriteStartElement( "backuprestorevolume" );
			xmlWriter.WriteAttributeString( "name", mName );
			mSource.OutputToXml( xmlWriter );
			mArchive.OutputToXml( xmlWriter );
			mDatabase.OutputToXml( xmlWriter );
			xmlWriter.WriteEndElement();
		}


		public void Dispose()
		{
			if ( mArchive != null )
				mArchive.Dispose();
			mArchive = null;
		}


		public bool DoBackup( ref bool outputToXmlRequired )
		{
			VolumeSnapshot currentSnapshot = null;
			bool backupSuccess = false;
			bool noChangesRequired = false;
			outputToXmlRequired = false;
			mCancelledByUser = false;

			try
			{
				mBusy = true;

				FileSync.__Log( this, "Building standing on disk snapshot..." );

				// Take a snapshot of the volume as it stands on disk
				currentSnapshot = VolumeSnapshot.BuildFromSource( mSource );

				if ( !currentSnapshot.Empty )
				{
					FileSync.__Log( this, "Snapshot built, " + currentSnapshot.FileCount + " files found" );

					VolumeSnapshotRevision mostRecentRevision = mDatabase.GetMostRecentRevision();

					// Compare to previous snapshot
					if ( mostRecentRevision != null )
					{
						FileSync.__Log( this, "Starting incremental backup, " + mDatabase.GetRevisionHistory().Count
							+ " revisions existing [Most recent: " + mostRecentRevision.ToString() + "]" );
		
						// Only do an incremental backup to backup whats changed since last full snapshot
						VolumeSnapshot mostRecentSnapshot = mDatabase.LoadSnapshotRevision( mSource, mostRecentRevision );
						backupSuccess = mBackupOperation.DoIncrementalBackup( currentSnapshot, mostRecentSnapshot, mSource, mArchive, out noChangesRequired );

						FileSync.__Log( this, "Incremental backup " + ( backupSuccess ? "completed successfully" : "failed" )
							+ " " + (noChangesRequired ? "no changes required" : "changes detected" ) );
					}
					else
					{
						FileSync.__Log( this, "Starting full initial backup" );

						// Brand new volume backup, do a full backup of the volume
						backupSuccess = mBackupOperation.DoFullBackup( currentSnapshot, mSource, mArchive );

						FileSync.__Log( this, "Full backup " + (backupSuccess ? "completed successfully" : "failed") );
					}

					if ( backupSuccess )
					{
						if ( !noChangesRequired )
						{
							FileSync.__Log( this, "Saving snapshot revision file [" + currentSnapshot.Revision.ToString() + "]" );

							mDatabase.SaveSnapshotRevision( currentSnapshot );
							outputToXmlRequired = true;

							FileSync.__Log( this, "Snapshot file saved" );
						}
						else
							FileSync.__Log( this, "No changes required, not saving revision [" + currentSnapshot.Revision.ToString() + "]" );
					}
					else
					{
						FileSync.__Log( this, "Backup failed, deleting temporary files for revision [" + currentSnapshot.Revision.ToString() + "]" );
						mDatabase.DeleteSnapshotRevision( currentSnapshot.Revision ); // remove half-copied data
						mArchive.DeleteRevision( currentSnapshot.Revision );
						outputToXmlRequired = true;
					}
				}
				else
					FileSync.__Log( this, "Built snapshot is empty, aborting backup" );
			}
			catch ( Exception e )
			{
				backupSuccess = false;
				FileSync.__LogError( this, "BackupRestoreVolume.DoBackup", e );

				if ( currentSnapshot != null )
				{
					FileSync.__Log( this, "Backup failed due to exception caught, deleting temporary files for revision [" + currentSnapshot.Revision.ToString() + "]" );
					mDatabase.DeleteSnapshotRevision( currentSnapshot.Revision ); // remove half-copied data
					mArchive.DeleteRevision( currentSnapshot.Revision );
				}
				outputToXmlRequired = true;
			}
			finally
			{
				mBusy = false;
			}

			return backupSuccess;
		}


		public void DeleteAllRevisions()
		{
			mDatabase.DeleteAllRevisions();
			mArchive.DeleteAllRevisions();
		}



		public bool Restore( VolumeSnapshotRevision revision, string onDiskPath )
		{
			bool success = false;
			mCancelledByUser = false;

			try
			{
				mBusy = true;

				VolumeSnapshot snapshot = mDatabase.LoadSnapshotRevision( mSource, revision );
				mRestoreOperation.Restore( snapshot, mArchive, onDiskPath );
				success = true;	
			}
			catch ( Exception e )
			{
				success = false;
				FileSync.__LogError( this, "BackupRestoreVolume.Restore", e );
			}
			finally
			{
				mBusy = false;
			}

			return success;
		}


		public void MergeSnapshotRevisionsByLimitedRevisionCount( int maxRevisionCount )
		{
			try
			{
				mCancelledByUser = false;
				mBusy = true;
				mMergeOperation.MergeSnapshotRevisionsByLimitedRevisionCount( mDatabase, mSource, mArchive, maxRevisionCount );
			}
			finally
			{
				mBusy = false;
			}
		}


		public void MergeSnapshotRevisionsByTimeLimit( TimeSpan timePeriod )
		{
			try
			{
				mCancelledByUser = false;
				mBusy = true;
				mMergeOperation.MergeSnapshotRevisionsByTimeLimit( mDatabase, mSource, mArchive, timePeriod );
			}
			finally
			{
				mBusy = false;
			}
		}
	}
}



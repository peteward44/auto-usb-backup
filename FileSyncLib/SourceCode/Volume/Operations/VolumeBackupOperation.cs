using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;


namespace PWLib.FileSyncLib
{

	public class VolumeBackupOperation
	{
		// backup statistics
		ulong mFileRunningCount = 0;
		ulong mFileTotalCount = 0;
		ulong mFilesTotalSizeRunning = 0;
		ulong mFilesTotalSize = 0;

		VolumeSource mSource = null;
		BaseArchive mArchive = null;
		VolumeSnapshotComparator mComparator = new VolumeSnapshotComparator();
		VolumeOperationController mOperation = new VolumeOperationController();
		volatile bool mBusy = false;

		public bool Busy { get { return mBusy; } }

		public VolumeSnapshotComparator Comparator { get { return mComparator; } }

		public delegate void OperationProgressDelegate( ulong fileNum, ulong fileCount, ulong bytesTranferred, ulong totalBytes );

		public event OperationProgressDelegate Progress;


		public VolumeBackupOperation()
		{
			mComparator.ChainOpController( mOperation );
			mComparator.Stats.DirectoryCreated += new VolumeSnapshotComparatorStatistics.DirectoryDelegate( mComparator_DirectoryCreated );
			mComparator.Stats.DirectoryTheSame += new VolumeSnapshotComparatorStatistics.DirectoryDelegate( mComparator_DirectoryTheSame );
			mComparator.Stats.FileCreated += new VolumeSnapshotComparatorStatistics.FileDelegate( mComparator_FileCreated );
			mComparator.Stats.FileChanged += new VolumeSnapshotComparatorStatistics.FileDelegate( mComparator_FileChanged );
			mComparator.Stats.FileTheSame += new VolumeSnapshotComparatorStatistics.FileDelegate( mComparator_FileTheSame );
		}


		public void Cancel( bool cancelByUser )
		{
			mOperation.Cancel( cancelByUser );
			if ( mArchive != null )
				mArchive.CancelOperation( cancelByUser );
		}



		#region Backup



		#region Incremental backup methods



		public bool DoIncrementalBackup( VolumeSnapshot currentSnapshot, VolumeSnapshot mostRecentSnapshot, VolumeSource source, BaseArchive archive, out bool noChangesRequired )
		{
			noChangesRequired = false;
			mSource = source;
			mArchive = archive;

			// see if anything's changed from the last one
			currentSnapshot.Root.Revision = mostRecentSnapshot.Root.Revision;
			if ( mComparator.CompareSnapshots( mostRecentSnapshot, currentSnapshot, true ) )
			{
				FileSync.__Log( this, "Incremental comparison complete, changes detected" );
				FileSync.__Log( this, "Total file count " + mComparator.Stats.FileCount );
				FileSync.__Log( this, "Total file size " + mComparator.Stats.TotalFileSize );
				FileSync.__Log( this, "New files " + mComparator.Stats.FilesCreated );
				FileSync.__Log( this, "Removed files " + mComparator.Stats.FilesRemoved );
				FileSync.__Log( this, "Changed files " + mComparator.Stats.FilesChanged );
				FileSync.__Log( this, "New directories " + mComparator.Stats.DirectoriesCreated );
				FileSync.__Log( this, "Removed directories " + mComparator.Stats.DirectoriesRemoved );
				FileSync.__Log( this, "Files unchanged " + mComparator.Stats.FilesTheSame );
				FileSync.__Log( this, "Directories unchanged " + mComparator.Stats.DirectoriesTheSame );

				mFileTotalCount = mComparator.Stats.FileCount;
				mFilesTotalSize = mComparator.Stats.TotalFileSize;
				mFileRunningCount = 0;
				mFilesTotalSizeRunning = 0;
				mComparator.CompareSnapshots( mostRecentSnapshot, currentSnapshot, false );
				return !mOperation.Cancelled;
			}
			else
			{
				FileSync.__Log( this, "Incremental comparison complete, no changes detected" );
				noChangesRequired = true;
				return true;
			}
		}



		void Backup_ArchiveStoreFile( ulong fileRunningBytes, ulong fileTotalSize, object userData )
		{
			VolumeSnapshotFile file = (VolumeSnapshotFile)userData;
			ulong runningSize = mFilesTotalSizeRunning + fileRunningBytes;
			if ( runningSize > mFilesTotalSize )
				runningSize = mFilesTotalSize;
			if ( Progress != null )
				Progress( mFileRunningCount, mFileTotalCount, runningSize, mFilesTotalSize );
		}


		void CopyFileToBackup( VolumeSnapshotFile file )
		{
			try
			{
				if ( !mOperation.CanContinue )
					return;
				file.Revision = file.Snapshot.Revision;
				mArchive.StoreFile( file.Snapshot.Revision, file.RelativePath, mSource.GetOnDiskPath( file.RelativePath ), new ArchiveFileDelegate( Backup_ArchiveStoreFile ), file );
				ulong runningSize = mFilesTotalSizeRunning + file.FileSize;
				if ( runningSize > mFilesTotalSize )
					runningSize = mFilesTotalSize;
				if ( Progress != null )
					Progress( mFileRunningCount + 1, mFileTotalCount, runningSize, mFilesTotalSize );
			}
			catch ( System.Exception e )
			{
				mOperation.Cancel( false );
				mComparator.Cancel();
				FileSync.__LogError( this, "Copy file failed '" + file.RelativePath + "'", e );
			}
			mFileRunningCount++;
			mFilesTotalSizeRunning += file.FileSize;
		}


		void mComparator_FileChanged( VolumeSnapshotFile oldFile, VolumeSnapshotFile newFile )
		{
			CopyFileToBackup( newFile );
		}

		void mComparator_FileCreated( VolumeSnapshotFile oldFile, VolumeSnapshotFile newFile )
		{
			CopyFileToBackup( newFile );
		}

		void mComparator_DirectoryCreated( VolumeSnapshotDirectory oldDir, VolumeSnapshotDirectory newDir )
		{
			newDir.Revision = newDir.Snapshot.Revision;
			CopyAll( newDir );
		}

		void mComparator_FileTheSame( VolumeSnapshotFile oldFile, VolumeSnapshotFile newFile )
		{
			newFile.Revision = oldFile.Revision;
		}

		void mComparator_DirectoryTheSame( VolumeSnapshotDirectory oldDirectory, VolumeSnapshotDirectory newDirectory )
		{
			newDirectory.Revision = oldDirectory.Revision;
		}


		#endregion


		#region Full backup methods


		public bool DoFullBackup( VolumeSnapshot currentSnapshot, VolumeSource source, BaseArchive archive )
		{
			mOperation.Reset();
			mFileTotalCount = currentSnapshot.FileCount;
			mFileRunningCount = 0;
			mFilesTotalSize = currentSnapshot.TotalFileSize;
			mFilesTotalSizeRunning = 0;
			mArchive = archive;
			mSource = source;
			return CopyAll( currentSnapshot.Root );
		}


		private bool CopyAll( VolumeSnapshotDirectory directory )
		{
			directory.Revision = directory.Snapshot.Revision;
			mArchive.CreateDirectory( directory.Snapshot.Revision, directory.RelativePath );

			if ( !mOperation.CanContinue )
				return false;

			foreach ( VolumeSnapshotFile fileEntry in directory.Files )
			{
				CopyFileToBackup( fileEntry );

				if ( !mOperation.CanContinue )
					return false;
			}

			foreach ( VolumeSnapshotDirectory dirEntry in directory.Directories )
			{
				if ( !CopyAll( dirEntry ) || !mOperation.CanContinue )
					return false;
			}

			return true;
		}


		#endregion

		#endregion

	
	}
}



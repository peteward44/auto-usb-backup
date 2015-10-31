using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;


namespace PWLib.FileSyncLib
{

	public class VolumeSyncOperation
	{
		//ulong mFileRunningCount = 0;
		//ulong mFileTotalCount = 0;
		//ulong mFilesTotalSizeRunning = 0;
		//ulong mFilesTotalSize = 0;
		//BaseArchive mArchive = null;
		VolumeSnapshotComparator mComparator = new VolumeSnapshotComparator();
		VolumeOperationController mOperation = new VolumeOperationController();
		volatile bool mBusy = false;

		public bool Busy { get { return mBusy; } }


		public VolumeSyncOperation()
		{
			//mComparator.DirectoryCreated += new VolumeSnapshotComparator.DirectoryDelegate( mComparator_DirectoryCreated );
			//mComparator.DirectoryTheSame += new VolumeSnapshotComparator.DirectoryDelegate( mComparator_DirectoryTheSame );
			//mComparator.FileCreated += new VolumeSnapshotComparator.FileDelegate( mComparator_FileCreated );
			//mComparator.FileChanged += new VolumeSnapshotComparator.FileDelegate( mComparator_FileChanged );
			//mComparator.FileTheSame += new VolumeSnapshotComparator.FileDelegate( mComparator_FileTheSame );
		}


		public void Cancel( bool cancelByUser )
		{
			mOperation.Cancel( cancelByUser );
		}



//        #region Backup



//        #region Incremental backup methods



//        public bool DoIncrementalBackup( VolumeSnapshot currentSnapshot, VolumeSnapshot mostRecentSnapshot, BaseArchive archive )
//        {
//            mArchive = archive;
//            // see if anything's changed from the last one
//            currentSnapshot.Root.Revision = mostRecentSnapshot.Root.Revision;
//            if ( mComparator.CompareSnapshots( mostRecentSnapshot, currentSnapshot, true ) )
//            {
//                mFileTotalCount = mComparator.FileCount;
//                mFilesTotalSize = mComparator.TotalFileSize;
//                mFileRunningCount = 0;
//                mFilesTotalSizeRunning = 0;
//                mComparator.CompareSnapshots( mostRecentSnapshot, currentSnapshot, false );
//                return !mOperation.Cancelled;
//            }
//            else
//                return false;
//        }



//        void Backup_ArchiveStoreFile( ulong fileRunningBytes, ulong fileTotalSize, object userData )
//        {
//            VolumeSnapshotFile file = (VolumeSnapshotFile)userData;
//            ulong runningSize = mFilesTotalSizeRunning + fileRunningBytes;
//            if ( runningSize > mFilesTotalSize )
//                runningSize = mFilesTotalSize;
////			mVolumeDesc.EventController.InvokeBackupProgress( this, file.Snapshot, mFileRunningCount, mFileTotalCount, runningSize, mFilesTotalSize );
//        }


//        void CopyFileToBackup( VolumeSnapshotFile file )
//        {
//            try
//            {
//                file.Revision = file.Snapshot.Revision;
//                mArchive.StoreFile( file.Snapshot.Revision, file.RelativePath, file.Snapshot.Source.GetOnDiskPath( file.RelativePath ), new ArchiveFileDelegate( Backup_ArchiveStoreFile ), file );
//                ulong runningSize = mFilesTotalSizeRunning + file.FileSize;
//                if ( runningSize > mFilesTotalSize )
//                    runningSize = mFilesTotalSize;
////				mVolumeDesc.EventController.InvokeBackupProgress( this, file.Snapshot, mFileRunningCount + 1, mFileTotalCount, runningSize, mFilesTotalSize );
//            }
//            catch ( System.Exception e )
//            {
////				Log.WriteException( "Copy file failed '" + file.OnDiskPath + "'", e );
//                // check if failure is due to usb drive being removed
//                //if ( mVolumeDesc.Identifier.UsbDrive != null && !mVolumeDesc.Source.IsActive )
//                //{
//                //    Log.WriteLine( LogType.All, "Volume drive '" + mVolumeDesc.Identifier.Name + "' has been forcably removed, aborting backup" );
//                //    CancelOperation( false );
//                //}
//                //else if ( mConsecutiveErrorsCount >= Config.ConsecutiveErrorsTillQuitOperation )
//                //{
//                //    Log.WriteLine( LogType.All, "Backup of '" + mVolumeDesc.Identifier.Name + "' has encountered too many consecutive errors, aborting backup" );
//                //    CancelOperation( false );
//                //}
//            }
//            mFileRunningCount++;
//            mFilesTotalSizeRunning += file.FileSize;
//        }


//        void mComparator_FileChanged( VolumeSnapshotFile oldFile, VolumeSnapshotFile newFile )
//        {
//            CopyFileToBackup( newFile );
//        }

//        void mComparator_FileCreated( VolumeSnapshotFile oldFile, VolumeSnapshotFile newFile )
//        {
//            CopyFileToBackup( newFile );
//        }

//        void mComparator_DirectoryCreated( VolumeSnapshotDirectory oldDir, VolumeSnapshotDirectory newDir )
//        {
//            newDir.Revision = newDir.Snapshot.Revision;
//            CopyAll( newDir );
//        }

//        void mComparator_FileTheSame( VolumeSnapshotFile oldFile, VolumeSnapshotFile newFile )
//        {
//            newFile.Revision = oldFile.Revision;
//        }

//        void mComparator_DirectoryTheSame( VolumeSnapshotDirectory oldDirectory, VolumeSnapshotDirectory newDirectory )
//        {
//            newDirectory.Revision = oldDirectory.Revision;
//        }


//        #endregion


//        #region Full backup methods


//        public bool DoFullBackup( VolumeSnapshot currentSnapshot, BaseArchive archive )
//        {
//            mFileTotalCount = currentSnapshot.FileCount;
//            mFileRunningCount = 0;
//            mFilesTotalSize = currentSnapshot.TotalFileSize;
//            mFilesTotalSizeRunning = 0;
//            mArchive = archive;
//            return CopyAll( currentSnapshot.Root );
//        }


//        private bool CopyAll( VolumeSnapshotDirectory directory )
//        {
//            directory.Revision = directory.Snapshot.Revision;
//            mArchive.CreateDirectory( directory.Snapshot.Revision, directory.RelativePath );

//            if ( !mOperation.CanContinue )
//                return false;

//            foreach ( VolumeSnapshotFile fileEntry in directory.Files )
//            {
//                CopyFileToBackup( fileEntry );

//                if ( !mOperation.CanContinue )
//                    return false;
//            }

//            foreach ( VolumeSnapshotDirectory dirEntry in directory.Directories )
//            {
//                if ( !CopyAll( dirEntry ) || !mOperation.CanContinue )
//                    return false;
//            }

//            return true;
//        }


//        #endregion

//        #endregion

	
	}
}



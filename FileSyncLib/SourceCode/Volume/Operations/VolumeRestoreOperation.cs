using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;


namespace PWLib.FileSyncLib
{

	public class VolumeRestoreOperation
	{
		ulong mFileRunningCount = 0;
		ulong mFileTotalCount = 0;
		ulong mFilesTotalSizeRunning = 0;
		ulong mFilesTotalSize = 0;


		public delegate void OperationProgressDelegate( ulong fileNum, ulong fileCount, ulong bytesTranferred, ulong totalBytes );
		public delegate void OperationErrorDelegate( string error, Exception e );

		public event OperationProgressDelegate Progress;
		public event OperationErrorDelegate Error;


		VolumeOperationController mOperation = new VolumeOperationController();
		volatile bool mBusy = false;

		public bool Busy { get { return mBusy; } }


		public VolumeRestoreOperation( )
		{
		}


		public void Cancel( bool cancelByUser )
		{
			mOperation.Cancel( cancelByUser );
		}



		public void Restore( VolumeSnapshot snapshot, BaseArchive archive, string onDiskPath )
		{
			mOperation.Reset();

			mFileTotalCount = 0;
			mFilesTotalSize = 0;
			mFileRunningCount = 0;
			mFilesTotalSizeRunning = 0;

			try
			{
				mBusy = true;

				VolumeSnapshotDirectory vsd = snapshot.Root;
			    mFileTotalCount = vsd.CountAllFiles( ref mFilesTotalSize );

				if ( !mOperation.CanContinue )
			        return;

			    int relativePathStartIndex = vsd.RelativePath.LastIndexOf( PWLib.Platform.Windows.Path.DirectorySeparatorChar );
			    if ( relativePathStartIndex < 0 )
			        relativePathStartIndex = 0;
			    ProcessDirectoryRestore( vsd, archive, relativePathStartIndex, onDiskPath );
			}
			finally
			{
			    mBusy = false;
			}
		}


		private void ProcessDirectoryRestore( VolumeSnapshotDirectory sourceDir, BaseArchive archive, int relativePathStartIndex, string targetDir )
		{
			if ( !mOperation.CanContinue )
		        return;

		    PWLib.Platform.Windows.Directory.CreateDirectory( targetDir );

		    foreach ( VolumeSnapshotFile fileEntry in sourceDir.Files )
		    {
		        try
		        {
		            string outDir = targetDir + fileEntry.RelativePath.Substring( relativePathStartIndex );
					archive.RestoreFile( fileEntry.Revision, fileEntry.RelativePath, outDir, new ArchiveFileDelegate( Restore_ArchiveStoreFile ), fileEntry );

					ulong runningSize = mFilesTotalSizeRunning + fileEntry.FileSize;
					if ( runningSize > mFilesTotalSize )
						runningSize = mFilesTotalSize;
					if ( Progress != null )
						Progress( mFileRunningCount + 1, mFileTotalCount, runningSize, mFilesTotalSize );
		        }
		        catch ( System.Exception e )
		        {
					if ( Error != null )
						Error( "Restore file failed '" + fileEntry.RelativePath + "'", e );
		        }

				if ( !mOperation.CanContinue )
		            return;

				mFileRunningCount++;
				mFilesTotalSizeRunning += fileEntry.FileSize;
		    }

		    foreach ( VolumeSnapshotDirectory directoryEntry in sourceDir.Directories )
		    {
		        ProcessDirectoryRestore( directoryEntry, archive, relativePathStartIndex, targetDir );
				if ( !mOperation.CanContinue )
		            return;
		    }
		}



		void Restore_ArchiveStoreFile( ulong fileRunningBytes, ulong fileTotalSize, object userData )
		{
			VolumeSnapshotFile file = (VolumeSnapshotFile)userData;
			ulong runningSize = mFilesTotalSizeRunning + fileRunningBytes;
			if ( runningSize > mFilesTotalSize )
				runningSize = mFilesTotalSize;
			if ( Progress != null )
				Progress( mFileRunningCount, mFileTotalCount, runningSize, mFilesTotalSize );
		}

	}
}



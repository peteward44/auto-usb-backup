using System;
using System.Collections.Generic;
using System.Text;


namespace PWLib.FileSyncLib
{
	public class VolumeSnapshotComparatorStatistics
	{
		public delegate void FileDelegate( VolumeSnapshotFile oldFile, VolumeSnapshotFile newFile );
		public delegate void DirectoryDelegate( VolumeSnapshotDirectory oldDirectory, VolumeSnapshotDirectory newDirectory );


		public event FileDelegate FileChanged;
		public event FileDelegate FileCreated;
		public event FileDelegate FileTheSame;

		public event DirectoryDelegate DirectoryCreated;
		public event DirectoryDelegate DirectoryTheSame;


		bool mInvokeCallbacks = false;
		bool mIsModified = false;
		public bool IsModified { get { return mIsModified; } }

		ulong mFileCount = 0;
		public ulong FileCount { get { return mFileCount; } }

		ulong mTotalFileSize = 0;
		public ulong TotalFileSize { get { return mTotalFileSize; } }

		ulong mDirectoriesCreated = 0;
		public ulong DirectoriesCreated { get { return mDirectoriesCreated; } }
		ulong mDirectoriesRemoved = 0;
		public ulong DirectoriesRemoved { get { return mDirectoriesRemoved; } }
		ulong mDirectoriesTheSame = 0;
		public ulong DirectoriesTheSame { get { return mDirectoriesTheSame; } }
		ulong mFilesChanged = 0;
		public ulong FilesChanged { get { return mFilesChanged; } }
		ulong mFilesCreated = 0;
		public ulong FilesCreated { get { return mFilesCreated; } }
		ulong mFilesRemoved = 0;
		public ulong FilesRemoved { get { return mFilesRemoved; } }
		ulong mFilesTheSame = 0;
		public ulong FilesTheSame { get { return mFilesTheSame; } }


		public void Reset( bool invokeCallbacks )
		{
			mInvokeCallbacks = invokeCallbacks;
			mIsModified = false;

			mFileCount = 0;
			mTotalFileSize = 0;

			mDirectoriesCreated = 0;
			mDirectoriesRemoved = 0;
			mDirectoriesTheSame = 0;
			mFilesChanged = 0;
			mFilesCreated = 0;
			mFilesRemoved = 0;
			mFilesTheSame = 0;
		}


		public void OnFileCreated( VolumeSnapshotFile file )
		{
			mFilesCreated++;
			mFileCount++;
			mTotalFileSize += file.FileSize;
			mIsModified = true;

			if ( mInvokeCallbacks )
			{
				if ( FileCreated != null )
					FileCreated( null, file );
			}
		}


		public void OnFileChanged( VolumeSnapshotFile oldFile, VolumeSnapshotFile newFile )
		{
			mFilesChanged++;
			mFileCount++;
			mTotalFileSize += newFile.FileSize;
			mIsModified = true;
			if ( mInvokeCallbacks )
			{
				if ( FileChanged != null )
					FileChanged( oldFile, newFile );
			}
		}


		public void OnFileTheSame( VolumeSnapshotFile oldFile, VolumeSnapshotFile newFile )
		{
			mFilesTheSame++;
			if ( mInvokeCallbacks )
			{
				if ( FileTheSame != null )
					FileTheSame( oldFile, newFile );
			}
		}


		public void OnFileRemoved( VolumeSnapshotFile oldFile )
		{
			mFilesRemoved++;
			mIsModified = true;
		}


		public void OnDirectoryRemoved( VolumeSnapshotDirectory oldDirectory )
		{
			mDirectoriesRemoved++;
			mIsModified = true;
		}


		public void OnDirectoryCreated( VolumeSnapshotDirectory newDirectory )
		{
			mDirectoriesCreated++;
			mIsModified = true;
			ulong numNewFilesInDir = newDirectory.CountAllFiles( ref mTotalFileSize );
			mFileCount += numNewFilesInDir;
			mFilesCreated += numNewFilesInDir;
			if ( mInvokeCallbacks )
			{
				if ( DirectoryCreated != null )
					DirectoryCreated( null, newDirectory );
			}
		}


		public void OnDirectoryTheSame( VolumeSnapshotDirectory oldDirectory, VolumeSnapshotDirectory newDirectory )
		{
			mDirectoriesTheSame++;
			if ( mInvokeCallbacks )
			{
				if ( DirectoryTheSame != null )
					DirectoryTheSame( oldDirectory, newDirectory );
			}
		}

	}
}

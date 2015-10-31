using System;
using System.Collections.Generic;
using System.Text;


namespace PWLib.FileSyncLib
{
	public class VolumeSnapshotComparator
	{
		VolumeSnapshotComparatorStatistics mStats = new VolumeSnapshotComparatorStatistics();
		VolumeOperationController mOperation = new VolumeOperationController();

		public VolumeSnapshotComparatorStatistics Stats { get { return mStats; } }


		public void Cancel()
		{
			mOperation.Cancel( false );
		}


		public VolumeSnapshotComparator()
		{
		}


		internal void ChainOpController( VolumeOperationController voc )
		{
			voc.Chain( mOperation );
		}


		public bool CompareSnapshots( VolumeSnapshot oldDb, VolumeSnapshot newDb, bool countFilesOnly )
		{
			mStats.Reset( !countFilesOnly );
			mOperation.Reset();
			return ProcessDirectory( oldDb.Root, newDb.Root );
		}


		private bool ProcessDirectory( VolumeSnapshotDirectory oldDb, VolumeSnapshotDirectory newDb )
		{
			// check for new/changed files
			foreach ( VolumeSnapshotFile newFile in newDb.Files )
			{
				// see if the file exists in the rhs snapshot and if it has changed
				VolumeSnapshotFile oldFile = oldDb.FindFile( newFile.FileName );
				if ( oldFile == null )
				{
					// File has been created
					mStats.OnFileCreated( newFile );
				}
				else
				{
					// File exists in both snapshots, check last modified date
					if ( oldFile.LastModified.Ticks != newFile.LastModified.Ticks )
					{ // file has been modified
						mStats.OnFileChanged( oldFile, newFile );
					}
					else
					{ // file the same
						mStats.OnFileTheSame( oldFile, newFile );
					}
				}

				if ( !mOperation.CanContinue )
					return mStats.IsModified;
			}

			// check for deleted files
			foreach ( VolumeSnapshotFile oldFile in oldDb.Files )
			{
				VolumeSnapshotFile newFile = newDb.FindFile( oldFile.FileName );
				if ( newFile == null )
				{
					mStats.OnFileRemoved( oldFile );
				}

				if ( !mOperation.CanContinue )
					return mStats.IsModified;
			}

			// check for deleted directories
			foreach ( VolumeSnapshotDirectory oldDir in oldDb.Directories )
			{
				VolumeSnapshotDirectory newDir = newDb.FindDirectory( oldDir.Name );
				if ( newDir == null )
				{
					mStats.OnDirectoryRemoved( oldDir );
				}

				if ( !mOperation.CanContinue )
					return mStats.IsModified;
			}

			// check for new directories
			foreach ( VolumeSnapshotDirectory newDir in newDb.Directories )
			{
				VolumeSnapshotDirectory oldDir = oldDb.FindDirectory( newDir.Name );
				if ( oldDir == null )
				{
					// Directory has been created
					mStats.OnDirectoryCreated( newDir );
				}
				else
				{
					mStats.OnDirectoryTheSame( oldDir, newDir );

					ProcessDirectory( oldDir, newDir );
				}

				if ( !mOperation.CanContinue )
					return mStats.IsModified;
			}

			return mStats.IsModified;
		}
	}
}

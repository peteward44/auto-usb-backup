using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;


namespace PWLib.FileSyncLib
{

	public class VolumeSnapshotMergeOperation
	{
		volatile bool mBusy = false;
		public bool Busy { get { return mBusy; } }

		VolumeSnapshotDatabase mDatabase = null;
		BaseArchive mArchive = null;


		public VolumeSnapshotMergeOperation()
		{
		}



		void MergeSnapshotRevisions( VolumeSnapshotDatabase database, VolumeSource source, BaseArchive archive, int numRevisionsToRemove )
		{
			try
			{
				mDatabase = database;
				mArchive = archive;
				mBusy = true;

				List<VolumeSnapshotRevision> revisionHistory = database.GetRevisionHistory();
				if ( numRevisionsToRemove == revisionHistory.Count )
				{
					// Need to remove all old revisions, delete everything
					FileSync.__Log( this, "Merge deleting all revisions" );
					database.DeleteAllRevisions();
				}
				else
				{
					// now we know how many revisions to remove, need to rebuild the new first revision.
					VolumeSnapshotRevision currentRevision = revisionHistory[ numRevisionsToRemove ];
					VolumeSnapshot currentSnapshot = database.LoadSnapshotRevision( source, currentRevision );

					FileSync.__Log( this, "Merge is turning revision [" + currentRevision.ToString() + "] into baseline" );
					TurnSnapshotIntoBaseline( currentSnapshot );

					// Now go through all existing snapshot .xml files and change any files referencing
					// a snapshot that has been removed and change it to the current snapshot revision.
					for ( int revisionNum = numRevisionsToRemove + 1; revisionNum < revisionHistory.Count; ++revisionNum )
					{
						VolumeSnapshotRevision incrementalRevision = revisionHistory[ revisionNum ];
						VolumeSnapshot incrementalSnapshot = database.LoadSnapshotRevision( source, incrementalRevision );

						FileSync.__Log( this, "Merge is reflecting revision [" + incrementalRevision.ToString() + "] into new baseline [" + currentRevision.ToString()  + "]" );
						UpdateSnapshotToReflectBaselineRevision( incrementalSnapshot, currentRevision );
					}

					// delete old revision data
					for ( int revisionNum = 0; revisionNum < numRevisionsToRemove; ++revisionNum )
					{
						VolumeSnapshotRevision revisionToDelete = revisionHistory[ revisionNum ];
						FileSync.__Log( this, "Merge is deleting revision [" + revisionToDelete.ToString() + "]" );
						database.DeleteSnapshotRevision( revisionToDelete );
					}
				}
			}
			catch ( System.Exception ex )
			{
				FileSync.__LogError( this, "Volume.CheckForExpiredSnapshotRevisions", ex );
			}
			finally
			{
				mBusy = false;
			}
		}



		public void MergeSnapshotRevisionsByLimitedRevisionCount( VolumeSnapshotDatabase database, VolumeSource source, BaseArchive archive, int maxRevisions )
		{
			// only keep a specific number of revisions
			List<VolumeSnapshotRevision> revisionHistory = database.GetRevisionHistory();
			int numRevisionsToRemove = 0;
			if ( revisionHistory.Count > maxRevisions )
			{
				numRevisionsToRemove = revisionHistory.Count - maxRevisions;
			}
			if ( numRevisionsToRemove > 0 )
			{
				FileSync.__Log( this, "Merge will remove " + numRevisionsToRemove + " revisions" );
				MergeSnapshotRevisions( database, source, archive, numRevisionsToRemove );
			}
			else
				FileSync.__Log( this, "Merge will not remove any revisions" );
		}


		public void MergeSnapshotRevisionsByTimeLimit( VolumeSnapshotDatabase database, VolumeSource source, BaseArchive archive, TimeSpan timePeriodToKeep )
		{
			// only keep revisions within a certain time period (e.g. last 6 months)
			List<VolumeSnapshotRevision> revisionHistory = database.GetRevisionHistory();
			int numRevisionsToRemove = 0;
			DateTime cutOffDate = DateTime.Now - timePeriodToKeep;
			foreach ( VolumeSnapshotRevision revision in revisionHistory )
			{
				if ( revision.CreationTime >= cutOffDate )
				{
					break;
				}
				else
					numRevisionsToRemove++;
			}
			if ( numRevisionsToRemove > 0 )
			{
				FileSync.__Log( this, "Merge will remove " + numRevisionsToRemove + " revisions" );
				MergeSnapshotRevisions( database, source, archive, numRevisionsToRemove );
			}
			else
				FileSync.__Log( this, "Merge will not remove any revisions" );
		}



		void TurnSnapshotIntoBaseline( VolumeSnapshot snapshot )
		{
			// iterate over each file in the snapshot. Copy any file that is from an
			// older revision to the current revision and update the .xml.
			List<VolumeSnapshotRevision> revisionsThatCanBeRemoved = new List<VolumeSnapshotRevision>();
			TurnSnapshotIntoBaselinePrivate( snapshot.Root, ref revisionsThatCanBeRemoved );
			snapshot.Root.Revision = snapshot.Root.Snapshot.Revision;

			RemoveOldRevisions( revisionsThatCanBeRemoved );
			mDatabase.SaveSnapshotRevision( snapshot );
		}


		void RemoveOldRevisions( List<VolumeSnapshotRevision> revisionsThatCanBeRemoved )
		{
			foreach ( VolumeSnapshotRevision revision in revisionsThatCanBeRemoved )
			{
				mDatabase.DeleteSnapshotRevision( revision );
				mArchive.DeleteRevision( revision );
			}
		}


		void TurnSnapshotIntoBaselinePrivate( VolumeSnapshotDirectory parentDir, ref List<VolumeSnapshotRevision> revisionsThatCanBeRemoved )
		{
			VolumeSnapshotRevision currentRevision = parentDir.Snapshot.Revision;
			foreach ( VolumeSnapshotFile file in parentDir.Files )
			{
				if ( file.Revision.Value < currentRevision.Value )
				{
					revisionsThatCanBeRemoved.Add( file.Revision );

					// file is older than current snapshot, copy it into the current snapshot
					// Use Move for speed - the file will be deleted in the next step anyway and
					// will no longer be needed
					mArchive.MoveFileFromRevision( file.Revision, currentRevision, file.RelativePath );

					// update revision id in .xml file
					file.Revision = currentRevision;
				}
			}

			foreach ( VolumeSnapshotDirectory dir in parentDir.Directories )
			{
				if ( dir.Revision.Value < currentRevision.Value )
					dir.Revision = currentRevision;
				TurnSnapshotIntoBaselinePrivate( dir, ref revisionsThatCanBeRemoved );
			}
		}


		void UpdateSnapshotToReflectBaselineRevision( VolumeSnapshot incrementalSnapshot, VolumeSnapshotRevision newRevision )
		{
			UpdateSnapshotToReflectBaselineRevisionPrivate( incrementalSnapshot.Root, newRevision );

			// save out .xml
			mDatabase.SaveSnapshotRevision( incrementalSnapshot );
		}


		void UpdateSnapshotToReflectBaselineRevisionPrivate( VolumeSnapshotDirectory parentDir, VolumeSnapshotRevision newRevision )
		{
			foreach ( VolumeSnapshotFile file in parentDir.Files )
			{
				if ( file.Revision.Value < newRevision.Value )
					file.Revision = newRevision;
			}

			foreach ( VolumeSnapshotDirectory dir in parentDir.Directories )
			{
				if ( dir.Revision.Value < newRevision.Value )
					dir.Revision = newRevision;
				UpdateSnapshotToReflectBaselineRevisionPrivate( dir, newRevision );
			}
		}

	}
}



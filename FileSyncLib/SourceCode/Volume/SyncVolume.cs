using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Xml;


namespace PWLib.FileSyncLib
{

	public class SyncVolume
	{
		string mName;
		VolumeSnapshotDatabase mDatabase;
		VolumeSource mSource;
		BaseArchive mArchive;

		public VolumeSnapshotDatabase Database { get { return mDatabase; } }
		public VolumeSource Source { get { return mSource; } }
		public BaseArchive Archive { get { return mArchive; } }

		bool mBusy = false;
		public bool Busy { get { return mBusy; } }

		VolumeSyncOperation mSyncOperation = new VolumeSyncOperation();

		public override string ToString()
		{
			return mName;
		}


		public SyncVolume( string name, VolumeSource source, BaseArchive archive )
		{
			mName = name;
			mSource = source;
			mDatabase = new VolumeSnapshotDatabase( archive.GetSnapshotXmlDir() );
			mArchive = archive;
		}


		public SyncVolume( XmlNode parentNode )
		{
			mName = PWLib.XmlHelp.GetAttribute( parentNode, "name", "" );
			mSource = VolumeSource.BuildFromXml( parentNode );
			mArchive = BaseArchive.BuildFromXml( parentNode );
			mDatabase = VolumeSnapshotDatabase.LoadFromXml( parentNode );
		}



		public void OutputToXml( XmlTextWriter xmlWriter )
		{
			xmlWriter.WriteStartElement( "syncvolume" );
			xmlWriter.WriteAttributeString( "name", mName );
			mSource.OutputToXml( xmlWriter );
			mArchive.OutputToXml( xmlWriter );
			mDatabase.OutputToXml( xmlWriter );
			xmlWriter.WriteEndElement();
		}



		public void DoSync()
		{
//            VolumeSnapshot currentSnapshot = null;

//            try
//            {
//                mBusy = true;
////				this.mVolumeDesc.Source.IsBusy = true;

////				Log.WriteLine( LogType.All, "Commencing backup of volume '" + mVolumeDesc.Identifier.Name + "'" );

////				mVolumeDesc.EventController.InvokeBackupInitStarted( this, firstBackupOnLoad );

//                // Take a snapshot of the volume as it stands on disk
//                currentSnapshot = VolumeSnapshot.BuildFromSource( mSource );

//                if ( !currentSnapshot.Empty )
//                {
//                    VolumeSnapshotRevision mostRecentRevision = mDatabase.GetMostRecentRevision();

////					mVolumeDesc.EventController.InvokeBackupStarted( this, currentSnapshot, firstBackupOnLoad );

//                    // Compare to previous snapshot
//                    if ( mostRecentRevision != null )
//                    {
//                        // Only do an incremental backup to backup whats changed since last full snapshot
//                        saveCurrentSnapshot = mBackupOperation.DoIncrementalBackup( currentSnapshot, mDatabase.LoadSnapshotRevision( mostRecentRevision ), mArchive );
//                    }
//                    else
//                    {
//                        // Brand new volume backup, do a full backup of the volume
//                        saveCurrentSnapshot = mBackupOperation.DoFullBackup( currentSnapshot, mArchive );
//                    }

//                    if ( saveCurrentSnapshot )
//                    {
//                        mDatabase.SaveSnapshotRevision( currentSnapshot );
//                    }
//                    else
//                    {
//                        mDatabase.DeleteSnapshotRevision( currentSnapshot.Revision ); // remove half-copied data
//                    }
//                }
//            }
//            catch ( Exception e )
//            {
////				Log.WriteException( "Backup failed", e );
//                mDatabase.DeleteSnapshotRevision( currentSnapshot.Revision ); // remove half-copied data
//            }
//            finally
//            {
//                mBusy = false;
//            }
		}
	}
}



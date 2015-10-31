 using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;


namespace PWLib.FileSyncLib
{
	public class VolumeSnapshot
	{

		VolumeSnapshotRevision mRevision;
		VolumeSnapshotDirectory mRoot = null;

		ulong mFileCount = 0;
		ulong mTotalFileSize = 0; // Total file size is size of files in bytes when it would be restored back onto disk.
		ulong mTotalStoredFileSize = 0; // Total stored file size is the size of the files which are stored only in this revision (ie the ones that have changed)
		ulong mTotalStoredInThisRevision = 0; // Total unique file size stored in this revision's directory

		public VolumeSnapshotDirectory Root { get { return mRoot; } }
		public VolumeSnapshotRevision Revision { get { return mRevision; } }

		public ulong FileCount { get { return mFileCount; } }
		public ulong TotalFileSize { get { return mTotalFileSize; } }
		public ulong TotalStoredFileSize { get { return mTotalStoredFileSize; } }
		public ulong TotalStoredInThisRevisionOnly { get { return mTotalStoredInThisRevision; } }
		public bool Empty { get { return mRoot.Directories.Count == 0 && mRoot.Files.Count == 0; } }


		private VolumeSnapshot( VolumeSource source )
		{
			mRevision = VolumeSnapshotRevision.CreateNew();
			mRoot = VolumeSnapshotDirectory.BuildFromSource( this, source, "" );
			mFileCount = mRoot.CountAllFiles( ref mTotalFileSize );
		}


		public static VolumeSnapshot BuildFromSource( VolumeSource source )
		{
			VolumeSnapshot snapshot = new VolumeSnapshot( source );
			return snapshot;
		}


		private VolumeSnapshot( VolumeSource source, XmlNode parentNode )
		{
			mRevision = VolumeSnapshotRevision.Create( PWLib.XmlHelp.GetAttribute( parentNode, VolumeSnapshotXml.XmlRevisionElement, "" ) );
			if ( mRevision == null )
				throw new Exception( "Could not read revision from snapshot xml '" + parentNode.Name + "'" );

			mRoot = VolumeSnapshotDirectory.BuildFromXml( this, PWLib.XmlHelp.GetFirstChildWithName( parentNode, "directory" ) );
			mFileCount = mRoot.CountAllFiles( ref mTotalFileSize );
		}


		public static VolumeSnapshot BuildFromXml( VolumeSource source, XmlNode parentNode )
		{
			VolumeSnapshot snapshot = new VolumeSnapshot( source, parentNode );
			return snapshot;
		}


		public void OutputToXml( XmlTextWriter xmlWriter )
		{
			xmlWriter.WriteStartElement( VolumeSnapshotXml.XmlSnapshotElement );
			xmlWriter.WriteAttributeString( VolumeSnapshotXml.XmlRevisionElement, mRevision.ToString() );

			mRoot.OutputToXml( xmlWriter );

			xmlWriter.WriteEndElement();
		}


		// Takes a full path to a sub directory and tries to find it
		public VolumeSnapshotDirectory GetSubDirectory( string directory )
		{
			VolumeSnapshotDirectory curDir = mRoot;
			foreach ( string dirname in directory.Split( PWLib.Platform.Windows.Path.DirectorySeparatorChar ) )
			{
				if ( dirname.Length > 0 )
				{
					VolumeSnapshotDirectory newCurDir = curDir.FindDirectory( dirname );
					if ( newCurDir == null )
						return null;
					else
						curDir = newCurDir;
				}
			}
			return curDir;
		}


		public VolumeSnapshotFile GetSubFile( string path )
		{
			VolumeSnapshotDirectory curDir = mRoot;
			string[] dirnames = path.Split( PWLib.Platform.Windows.Path.DirectorySeparatorChar );

			for ( int index = 0; index < dirnames.Length - 1; ++index )
			{
				string dirname = dirnames[ index ];
				if ( dirname.Length > 0 )
				{
					VolumeSnapshotDirectory newCurDir = curDir.FindDirectory( dirname );
					if ( newCurDir == null )
						return null;
					else
						curDir = newCurDir;
				}
			}

			return curDir.FindFile( dirnames[ dirnames.Length - 1 ] );
		}
	}
}

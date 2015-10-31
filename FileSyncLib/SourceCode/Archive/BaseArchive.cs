using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;


namespace PWLib.FileSyncLib
{
	public enum ArchiveType
	{
		Open,
		Big7z,
		Individual7z
	}


	public delegate void ArchiveFileDelegate( ulong totalRunningSize, ulong totalSize, object userData );


	public abstract class BaseArchive : IDisposable
	{
		public abstract void Dispose();

		public abstract ArchiveType Type { get; }

		public abstract bool IsAvailable { get; }

		public abstract void DeleteAllRevisions();
		public abstract void DeleteRevision( VolumeSnapshotRevision revision );

		public abstract void StoreFile( VolumeSnapshotRevision revision, string relativePath, string onDiskPath, ArchiveFileDelegate archiveFileDelegate, object userData );
		public abstract void RestoreFile( VolumeSnapshotRevision revision, string fileRelativePath, string onDiskOutputPath, ArchiveFileDelegate archiveFileDelegate, object userData );
		public abstract void CopyFileFromRevision( VolumeSnapshotRevision sourceRevision, VolumeSnapshotRevision targetRevision, string fileRelativePath, ArchiveFileDelegate archiveFileDelegate, object userData );
		public abstract void MoveFileFromRevision( VolumeSnapshotRevision sourceRevision, VolumeSnapshotRevision targetRevision, string fileRelativePath );
		public abstract void CreateDirectory( VolumeSnapshotRevision revision, string relativePath );
		public abstract void CancelOperation( bool cancelByUser );

		public abstract string GetSnapshotXmlDir();

		protected abstract void OutputToXmlProtected( XmlTextWriter xmlWriter );


		public void OutputToXml( XmlTextWriter xmlWriter )
		{
			xmlWriter.WriteStartElement( "archive" );
			xmlWriter.WriteAttributeString( "archivetype", Type.ToString() );
			OutputToXmlProtected( xmlWriter );
			xmlWriter.WriteEndElement();
		}

		public static BaseArchive BuildFromXml( XmlNode parentNode )
		{
			XmlNode element = PWLib.XmlHelp.GetFirstChildWithName( parentNode, "archive" );
			string archiveTypeString = PWLib.XmlHelp.GetAttribute( element, "archivetype", "" );
			ArchiveType at = (ArchiveType)Enum.Parse( typeof( ArchiveType ), archiveTypeString, true );

			switch ( at )
			{
				default:
					throw new Exception( "Unknown archive type '" + archiveTypeString + "'" );
				case ArchiveType.Open:
					return new OpenArchive( element );
				case ArchiveType.Big7z:
					return null;
				case ArchiveType.Individual7z:
					//return new IndividualCompressedArchive( element );
					return null;
			}
		}
	}
}

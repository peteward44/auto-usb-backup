using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;


namespace PWLib.FileSyncLib
{
	public class VolumeSnapshotFile
	{
		VolumeSnapshot mSnapshot;
		VolumeSnapshotRevision mRevision = null;

		string mFileName = "", mRelativePath = "";
		ulong mFileSize = 0;
		DateTime mLastModified;

		public string FileName { get { return mFileName; } }
		public string RelativePath { get { return mRelativePath; } }
		public DateTime LastModified { get { return mLastModified; } }
		public ulong FileSize { get { return mFileSize; } }
		public VolumeSnapshotRevision Revision { get { return mRevision; } set { mRevision = value; } }
		public VolumeSnapshot Snapshot { get { return mSnapshot; } }


		public override string ToString()
		{
			return mRevision.ToString() + ":" + mFileName;
		}

		public override bool Equals( object obj )
		{
			if ( obj is VolumeSnapshotFile )
			{
				VolumeSnapshotFile rhs = (VolumeSnapshotFile)obj;
				return string.Compare( rhs.mFileName, this.mFileName, true ) == 0;
			}
			else
				return false;
		}

		public override int GetHashCode()
		{
			return mFileName.GetHashCode() * mRevision.GetHashCode();
		}


		private VolumeSnapshotFile( VolumeSnapshot snapshot, XmlNode node )
		{
			mSnapshot = snapshot;
			mFileName = PWLib.XmlHelp.DirtyString( PWLib.XmlHelp.GetAttribute( node, VolumeSnapshotXml.XmlFilenameElement, "" ) );
			mRelativePath = PWLib.XmlHelp.DirtyString( PWLib.XmlHelp.GetAttribute( node, VolumeSnapshotXml.XmlRelativePathElement, "" ) );
			mRevision = VolumeSnapshotRevision.Create( PWLib.XmlHelp.GetAttribute( node, VolumeSnapshotXml.XmlRevisionElement, "" ) );

			ulong.TryParse( PWLib.XmlHelp.GetAttribute( node, VolumeSnapshotXml.XmlSizeElement, "0" ), out mFileSize );

			long ticks;
			if ( long.TryParse( PWLib.XmlHelp.GetAttribute( node, VolumeSnapshotXml.XmlLastModifiedElement, "0" ), out ticks ) )
				mLastModified = new DateTime( ticks );
		}

		private VolumeSnapshotFile( VolumeSnapshot snapshot, VolumeSource source, string relativePath )
		{
			mSnapshot = snapshot;
			mRevision = null;
			mRelativePath = relativePath;
			mFileName = PWLib.Platform.Windows.Path.GetFileName( relativePath );
			mLastModified = source.GetLastWriteTimeUtc( relativePath );
			mFileSize = source.GetFileSize( relativePath );
		}


		public static VolumeSnapshotFile BuildFromSource( VolumeSnapshot snapshot, VolumeSource source, string relativePath )
		{
			return new VolumeSnapshotFile( snapshot, source, relativePath );
		}

		public static VolumeSnapshotFile BuildFromXml( VolumeSnapshot snapshot, XmlNode node )
		{
			return new VolumeSnapshotFile( snapshot, node );
		}


		public void OutputToXml( XmlWriter xmlWriter )
		{
			xmlWriter.WriteStartElement( VolumeSnapshotXml.XmlFileElement );
			xmlWriter.WriteAttributeString( VolumeSnapshotXml.XmlFilenameElement, PWLib.XmlHelp.CleanString( mFileName ) );
			xmlWriter.WriteAttributeString( VolumeSnapshotXml.XmlRelativePathElement, PWLib.XmlHelp.CleanString( mRelativePath ) );
			xmlWriter.WriteAttributeString( VolumeSnapshotXml.XmlRevisionElement, mRevision.ToString() );
			xmlWriter.WriteAttributeString( VolumeSnapshotXml.XmlSizeElement, mFileSize.ToString() );
			xmlWriter.WriteAttributeString( VolumeSnapshotXml.XmlLastModifiedElement, mLastModified.Ticks.ToString() );
			xmlWriter.WriteEndElement();
		}
	}

}

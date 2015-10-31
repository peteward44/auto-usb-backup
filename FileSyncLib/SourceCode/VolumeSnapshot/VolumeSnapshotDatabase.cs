using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;


namespace PWLib.FileSyncLib
{
	public class VolumeSnapshotDatabase
	{
		Dictionary<VolumeSnapshotRevision, VolumeSnapshot> mSnapshotDictionary = new Dictionary<VolumeSnapshotRevision, VolumeSnapshot>();
		List<VolumeSnapshotRevision> mSnapshotsStored = new List<VolumeSnapshotRevision>();
		string mXmlRootDir;


		string GetRevisionFileName( VolumeSnapshotRevision revision )
		{
			return mXmlRootDir + PWLib.Platform.Windows.Path.DirectorySeparatorChar + revision.Value.ToString() + ".xml";
		}


		public static VolumeSnapshotDatabase LoadFromXml( XmlNode parentNode )
		{
			return new VolumeSnapshotDatabase( parentNode );
		}


		public VolumeSnapshotDatabase( string xmlRootDir )
		{
			mXmlRootDir = xmlRootDir;
		}


		private VolumeSnapshotDatabase( XmlNode parentNode )
		{
			XmlNode element = PWLib.XmlHelp.GetFirstChildWithName( parentNode, "snapshots" );
			mXmlRootDir = PWLib.XmlHelp.DirtyString( PWLib.XmlHelp.GetAttribute( element, "snapshotxmldir", "" ) );

			foreach ( XmlNode childNode in element.ChildNodes )
			{
				string lowerName = childNode.Name.ToLower();
				if ( lowerName == VolumeSnapshotXml.XmlSnapshotElement )
				{
					string revisionTicks = PWLib.XmlHelp.GetAttribute( childNode, VolumeSnapshotXml.XmlRevisionElement, "" );
					VolumeSnapshotRevision revision = VolumeSnapshotRevision.Create( revisionTicks );
					mSnapshotsStored.Add( revision );
				}
			}
		}


		public void OutputToXml( XmlTextWriter xmlWriter )
		{
			xmlWriter.WriteStartElement( "snapshots" );
			xmlWriter.WriteAttributeString( "snapshotxmldir", PWLib.XmlHelp.CleanString( mXmlRootDir ) );

			foreach ( VolumeSnapshotRevision revision in mSnapshotsStored )
			{
				xmlWriter.WriteStartElement( "snapshot" );
				xmlWriter.WriteAttributeString( "revision", revision.Value.ToString() );
				xmlWriter.WriteEndElement();
			}

			xmlWriter.WriteEndElement();
		}


		public List<VolumeSnapshotRevision> GetRevisionHistory()
		{
			List < VolumeSnapshotRevision > list = new List<VolumeSnapshotRevision>( mSnapshotsStored );
			list.Sort();
			return list;
		}


		public VolumeSnapshotRevision GetMostRecentRevision()
		{
			List<VolumeSnapshotRevision> list = GetRevisionHistory();
			if ( list.Count > 0 )
				return list[ list.Count - 1 ];
			else
				return null;
		}


		public void DeleteSnapshotRevision( VolumeSnapshotRevision revision )
		{
			PWLib.Platform.Windows.File.Delete( GetRevisionFileName( revision ) );

			if ( mSnapshotsStored.Contains( revision ) )
				mSnapshotsStored.Remove( revision );
			if ( mSnapshotDictionary.ContainsKey( revision ) )
				mSnapshotDictionary.Remove( revision );
		}


		public void DeleteAllRevisions()
		{
			foreach ( VolumeSnapshotRevision revision in mSnapshotsStored )
			{
				PWLib.Platform.Windows.File.Delete( GetRevisionFileName( revision ) );
			}
			mSnapshotDictionary.Clear();
			mSnapshotsStored.Clear();
		}


		public VolumeSnapshot LoadSnapshotRevision( VolumeSource source, VolumeSnapshotRevision revision )
		{
			if ( mSnapshotDictionary.ContainsKey( revision ) )
				return mSnapshotDictionary[ revision ];
			else
			{
				XmlDocument doc = new XmlDocument();
				doc.Load( GetRevisionFileName( revision ) );

				XmlNode snapshotXmlNode = PWLib.XmlHelp.GetFirstChildWithName( doc, VolumeSnapshotXml.XmlSnapshotElement );
				VolumeSnapshot snapshot = VolumeSnapshot.BuildFromXml( source, snapshotXmlNode );
				mSnapshotDictionary.Add( revision, snapshot );
				return snapshot;
			}
		}


		// This updates cached XML, will still need flushing to disk afterwards in OutputToXml()
		public void SaveSnapshotRevision( VolumeSnapshot snapshot )
		{
			VolumeSnapshotRevision revision = snapshot.Revision;
			if ( mSnapshotDictionary.ContainsKey( revision ) )
				mSnapshotDictionary[ revision ] = snapshot;
			else
				mSnapshotDictionary.Add( revision, snapshot );
			if ( !mSnapshotsStored.Contains( revision ) )
				mSnapshotsStored.Add( revision );

			PWLib.Platform.Windows.Directory.CreateDirectory( mXmlRootDir );
			XmlTextWriter xmlWriter = new XmlTextWriter( GetRevisionFileName( revision ), Encoding.Unicode );
			xmlWriter.Formatting = Formatting.Indented;
			xmlWriter.WriteStartDocument();
		
			snapshot.OutputToXml( xmlWriter );

			xmlWriter.Close();
		}



	}
}


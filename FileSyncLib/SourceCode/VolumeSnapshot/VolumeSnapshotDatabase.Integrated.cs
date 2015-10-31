using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;


namespace PWLib.FileSyncLib
{
	public class VolumeSnapshotDatabase
	{
		Dictionary<VolumeSnapshotRevision, VolumeSnapshot> mSnapshotDictionary = new Dictionary<VolumeSnapshotRevision, VolumeSnapshot>();
		Dictionary<VolumeSnapshotRevision, string> mSnapshotXmlCache = new Dictionary<VolumeSnapshotRevision, string>();


		public static VolumeSnapshotDatabase LoadFromXml( XmlNode parentNode )
		{
			return new VolumeSnapshotDatabase( parentNode );
		}


		public VolumeSnapshotDatabase()
		{
		}


		private VolumeSnapshotDatabase( XmlNode parentNode )
		{
			XmlNode element = PWLib.XmlHelp.GetFirstChildWithName( parentNode, "snapshots" );
			foreach ( XmlNode childNode in element.ChildNodes )
			{
				string lowerName = childNode.Name.ToLower();
				if ( lowerName == VolumeSnapshotXml.XmlSnapshotElement )
				{
					string revisionTicks = PWLib.XmlHelp.GetAttribute( childNode, VolumeSnapshotXml.XmlRevisionElement, "" );
					VolumeSnapshotRevision revision = VolumeSnapshotRevision.Create( revisionTicks );
					mSnapshotXmlCache.Add( revision, childNode.OuterXml );
				}
			}
		}


		public void OutputToXml( XmlTextWriter xmlWriter )
		{
			xmlWriter.WriteStartElement( "snapshots" );
			string finalString = "\r\n";
			foreach ( KeyValuePair<VolumeSnapshotRevision, string> kvp in mSnapshotXmlCache )
			{
				finalString += kvp.Value + "\r\n";
			}
			xmlWriter.WriteRaw( finalString );
			xmlWriter.WriteEndElement();
		}


		public List<VolumeSnapshotRevision> GetRevisionHistory()
		{
			List<VolumeSnapshotRevision> list = new List<VolumeSnapshotRevision>();
			foreach ( VolumeSnapshotRevision rev in mSnapshotXmlCache.Keys )
			{
				list.Add( rev );
			}
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
			if ( mSnapshotXmlCache.ContainsKey( revision ) )
				mSnapshotXmlCache.Remove( revision );
			if ( mSnapshotDictionary.ContainsKey( revision ) )
				mSnapshotDictionary.Remove( revision );
		}


		public void DeleteAllRevisions()
		{
			mSnapshotXmlCache.Clear();
			mSnapshotDictionary.Clear();
		}


		public VolumeSnapshot LoadSnapshotRevision( VolumeSource source, VolumeSnapshotRevision revision )
		{
			if ( mSnapshotDictionary.ContainsKey( revision ) )
				return mSnapshotDictionary[ revision ];
			else
			{
				if ( !mSnapshotXmlCache.ContainsKey( revision ) )
					throw new Exception( "Could not locate snapshot revision '" + revision.ToString() + "' in database" );
				string xmlFragment = mSnapshotXmlCache[ revision ];

				XmlDocument doc = new XmlDocument();
				doc.LoadXml( xmlFragment );

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

			System.IO.StringWriter stringWriter = new System.IO.StringWriter();
			XmlTextWriter xmlWriter = new XmlTextWriter( stringWriter );
			xmlWriter.Formatting = Formatting.Indented;

			snapshot.OutputToXml( xmlWriter );

			string xmlFragment = stringWriter.ToString();

			xmlWriter.Close();
			stringWriter.Close();

			if ( mSnapshotXmlCache.ContainsKey( revision ) )
				mSnapshotXmlCache[ revision ] = xmlFragment;
			else
				mSnapshotXmlCache.Add( revision, xmlFragment );
		}



	}
}


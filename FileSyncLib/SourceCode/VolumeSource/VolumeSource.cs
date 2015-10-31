using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;


namespace PWLib.FileSyncLib
{
	public enum VolumeType
	{
		UsbDevice,
		LocalFolder,
	}


	public abstract class VolumeSource
	{

		VolumeType mVolumeType;
		public VolumeType VolumeType { get { return mVolumeType; } }

		public abstract string GetOnDiskPath( string relativePath );
		public abstract string[] GetFiles( string relativePath );
		public abstract string[] GetRelativePathFiles( string relativePath );
		public abstract string[] GetDirectories( string relativePath );
		public abstract string[] GetRelativePathDirectories( string relativePath );

		public abstract DateTime GetLastWriteTimeUtc( string relativePath );
		public abstract ulong GetFileSize( string relativePath );

		protected abstract void OutputToXmlProtected( XmlTextWriter xmlWriter );

		public abstract bool MediaAvailable { get; }
		public abstract bool MediaLocked { get; set; }

		public delegate void MediaAvailableChangedDelegate( VolumeSource source, bool isAvailable );
		public event MediaAvailableChangedDelegate MediaAvailableChanged;

		protected void InvokeMediaChangedEvent()
		{
			if ( MediaAvailableChanged != null )
				MediaAvailableChanged( this, MediaAvailable );
		}


		public VolumeSource( VolumeType volumeType )
		{
			mVolumeType = volumeType;
		}


		public void OutputToXml( XmlTextWriter xmlWriter )
		{
			xmlWriter.WriteStartElement( "source" );
			xmlWriter.WriteAttributeString( "volumetype", mVolumeType.ToString() );
			OutputToXmlProtected( xmlWriter );
			xmlWriter.WriteEndElement();
		}


		public static VolumeSource BuildFromXml( XmlNode parentNode )
		{
			XmlNode element = PWLib.XmlHelp.GetFirstChildWithName( parentNode, "source" );
			string volumeTypeString = PWLib.XmlHelp.GetAttribute( element, "volumetype", "" );
			VolumeType vt = (VolumeType)Enum.Parse( typeof( VolumeType ), volumeTypeString, true );
			switch ( vt )
			{
				case VolumeType.LocalFolder:
					return new VolumeLocalFolderSource( element );
				case VolumeType.UsbDevice:
					return new VolumeUsbSource( element );
				default:
					throw new Exception( "Unknown volume type '" + volumeTypeString + "'" );
			}
		}


	}
}

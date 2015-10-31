using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Threading;


namespace PWLib.FileSyncLib
{

	public class VolumeUsbSource : VolumeSource
	{
		PWLib.UsbDrive.UsbDriveInfo mDrive;


		public override bool MediaLocked
		{
			get { return mDrive.MediaLocked; }
			set { mDrive.MediaLocked = value; }
		}

		public override bool MediaAvailable { get { return mDrive.MediaAvailable; } }


		string BuildOnDiskPath( string relativePath )
		{
			if ( !mDrive.MediaAvailable )
				return "";
			if ( !relativePath.StartsWith( @"\" ) && !relativePath.StartsWith( @"/" ) )
				relativePath = @"\" + relativePath;
			return mDrive.DriveId.DriveLetter + ":" + relativePath;
		}


		string BuildRelativePath( string absolutePath )
		{
			string driveString = mDrive.DriveId.DriveLetter + @":";
			if ( absolutePath.StartsWith( driveString, true, null ) )
			{
				return absolutePath.Substring( driveString.Length );
			}
			else
				return absolutePath;
		}


		string[] ConvertAbsolutePathsToRelative( string[] absolute )
		{
			string[] relativePaths = new string[ absolute.Length ];
			for ( int i = 0; i < absolute.Length; ++i )
			{
				string absFile = absolute[ i ];
				string relFile = BuildRelativePath( absFile );
				relativePaths[ i ] = relFile;
			}
			return relativePaths;
		}


		void Init()
		{
			mDrive.MediaAvailableChanged += new EventHandler( mDrive_MediaAvailableChanged );
		}


		void mDrive_MediaAvailableChanged( object sender, EventArgs e )
		{
			InvokeMediaChangedEvent();
		}


		public VolumeUsbSource( PWLib.UsbDrive.UsbDriveInfo driveInfo )
			: base( VolumeType.UsbDevice )
		{
			mDrive = driveInfo;
			Init();
		}


		public VolumeUsbSource( XmlNode parentNode )
			: base( VolumeType.UsbDevice )
		{
			mDrive = PWLib.UsbDrive.UsbDriveInfo.BuildFromXml( parentNode );
			Init();
		}



		public override string ToString()
		{
			return mDrive.ToString();
		}

		public override bool Equals( object obj )
		{
			if ( obj is VolumeUsbSource )
			{
				VolumeUsbSource rhs = (VolumeUsbSource)obj;
				if ( mDrive.Equals( rhs.mDrive ) )
					return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return mDrive.GetHashCode();
		}



		public override string[] GetFiles( string relativePath )
		{
			return PWLib.Platform.Windows.Directory.GetFiles( BuildOnDiskPath( relativePath ) );
		}


		public override string[] GetRelativePathFiles( string relativePath )
		{
			return ConvertAbsolutePathsToRelative( PWLib.Platform.Windows.Directory.GetFiles( BuildOnDiskPath( relativePath ) ) );
		}


		public override string[] GetDirectories( string relativePath )
		{
			return PWLib.Platform.Windows.Directory.GetDirectories( BuildOnDiskPath( relativePath ) );
		}


		public override string[] GetRelativePathDirectories( string relativePath )
		{
			return ConvertAbsolutePathsToRelative( PWLib.Platform.Windows.Directory.GetDirectories( BuildOnDiskPath( relativePath ) ) );
		}


		public override DateTime GetLastWriteTimeUtc( string relativePath )
		{
			return PWLib.Platform.Windows.File.GetLastWriteTimeUtc( BuildOnDiskPath( relativePath ) );
		}


		public override ulong GetFileSize( string relativePath )
		{
			return PWLib.Platform.Windows.File.GetFileSize( BuildOnDiskPath( relativePath ) );
		}


		protected override void OutputToXmlProtected( XmlTextWriter xmlWriter )
		{
			mDrive.OutputToXml( xmlWriter );
		}


		public override string GetOnDiskPath( string relativePath )
		{
			return BuildOnDiskPath( relativePath );
		}

	}
}

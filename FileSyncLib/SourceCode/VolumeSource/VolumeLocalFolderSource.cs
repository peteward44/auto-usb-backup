using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace PWLib.FileSyncLib
{
	public class VolumeLocalFolderSource : VolumeSource
	{
		string BuildOnDiskPath( string relativePath )
		{
			if ( !relativePath.StartsWith( @"\" ) && !relativePath.StartsWith( @"/" ) )
				relativePath = @"\" + relativePath;
			return mLocalRootPath + relativePath;
		}


		string BuildRelativePath( string absolutePath )
		{
			if ( absolutePath.StartsWith( mLocalRootPath, true, null ) )
			{
				return absolutePath.Substring( mLocalRootPath.Length );
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



		string mLocalRootPath;

		public string RootPath { get { return mLocalRootPath; } }

		bool mLocked = false;
		public override bool MediaLocked { get { return mLocked; } set { mLocked = value; } }

		public override bool MediaAvailable { get { return PWLib.Platform.Windows.Directory.Exists( mLocalRootPath ); } }


		public VolumeLocalFolderSource( string localFolderPath )
			: base( VolumeType.LocalFolder )
		{
			mLocalRootPath = localFolderPath;
			if ( mLocalRootPath[ mLocalRootPath.Length - 1 ] == PWLib.Platform.Windows.Path.DirectorySeparatorChar )
				mLocalRootPath = mLocalRootPath.Substring( 0, mLocalRootPath.Length - 1 );
		}


		public VolumeLocalFolderSource( XmlNode parentNode )
			: base( VolumeType.LocalFolder )
		{
			mLocalRootPath = PWLib.XmlHelp.DirtyString( PWLib.XmlHelp.GetAttribute( parentNode, "localfolder", "" ) );
		}


		protected override void OutputToXmlProtected( System.Xml.XmlTextWriter xmlWriter )
		{
			xmlWriter.WriteAttributeString( "localfolder", PWLib.XmlHelp.CleanString( mLocalRootPath ) );
		}


		public override string ToString()
		{
			return mLocalRootPath;
		}

		public override bool Equals( object obj )
		{
			if ( obj is VolumeLocalFolderSource )
			{
				VolumeLocalFolderSource rhs = (VolumeLocalFolderSource)obj;
				if ( string.Compare( mLocalRootPath, rhs.mLocalRootPath, true ) == 0 )
					return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return mLocalRootPath.GetHashCode();
		}


		public override string[] GetFiles( string relativePath )
		{
			return PWLib.Platform.Windows.Directory.GetFiles( BuildOnDiskPath( relativePath ) );
		}


		public override string[] GetDirectories( string relativePath )
		{
			return PWLib.Platform.Windows.Directory.GetDirectories( BuildOnDiskPath( relativePath ) );
		}


		public override DateTime GetLastWriteTimeUtc( string relativePath )
		{
			return PWLib.Platform.Windows.File.GetLastWriteTimeUtc( BuildOnDiskPath( relativePath ) );
		}


		public override ulong GetFileSize( string relativePath )
		{
			return PWLib.Platform.Windows.File.GetFileSize( BuildOnDiskPath( relativePath ) );
		}


		public override string GetOnDiskPath( string relativePath )
		{
			return BuildOnDiskPath( relativePath );
		}


		public override string[] GetRelativePathFiles( string relativePath )
		{
			return ConvertAbsolutePathsToRelative( PWLib.Platform.Windows.Directory.GetFiles( BuildOnDiskPath( relativePath ) ) );
		}


		public override string[] GetRelativePathDirectories( string relativePath )
		{
			return ConvertAbsolutePathsToRelative( PWLib.Platform.Windows.Directory.GetDirectories( BuildOnDiskPath( relativePath ) ) );
		}

	}
}

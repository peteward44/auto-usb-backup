using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace PWLib.FileSyncLib
{
	public class VolumeSnapshotDirectory
	{
		VolumeSnapshot mSnapshot;
		List<VolumeSnapshotFile> mFiles = new List<VolumeSnapshotFile>();
		List<VolumeSnapshotDirectory> mDirectories = new List<VolumeSnapshotDirectory>();

		string mName = "", mRelativePath = "";
		VolumeSnapshotRevision mRevision;
		DateTime mLastModified;


		public string RelativePath { get { return mRelativePath; } }
		public string Name { get { return mName; } }

		public DateTime LastModified { get { return mLastModified; } }
		public VolumeSnapshotRevision Revision { get { return mRevision; } set { mRevision = value; } }
		public VolumeSnapshot Snapshot { get { return mSnapshot; } }

		public List<VolumeSnapshotFile> Files { get { return mFiles; } }
		public List<VolumeSnapshotDirectory> Directories { get { return mDirectories; } }


		static List<string> sIgnoredDirectoryNames = new List<string>();
		public static List<string> IgnoredDirectoryNames { get { return sIgnoredDirectoryNames; } }

		static bool IsIgnoredDirectoryName( string dirName )
		{
			foreach ( string ignoredName in sIgnoredDirectoryNames )
			{
				if ( string.Compare( dirName, ignoredName, true ) == 0 )
					return true;
			}
			return false;
		}


		public override string ToString()
		{
			return mRevision.ToString() + ":" + mName;
		}

		public override bool Equals( object obj )
		{
			if ( obj is VolumeSnapshotDirectory )
			{
				VolumeSnapshotDirectory rhs = (VolumeSnapshotDirectory)obj;
				return string.Compare( rhs.mName, this.mName, true ) == 0;
			}
			else
				return false;
		}

		public override int GetHashCode()
		{
			return mName.GetHashCode() * mRevision.GetHashCode();
		}


		public static VolumeSnapshotDirectory BuildFromXml( VolumeSnapshot snapshot, XmlNode parentNode )
		{
			VolumeSnapshotDirectory dirEntry = new VolumeSnapshotDirectory( snapshot, parentNode );
			return dirEntry;
		}


		private VolumeSnapshotDirectory( VolumeSnapshot snapshot, XmlNode parentNode )
		{
			mSnapshot = snapshot;
			mName = PWLib.XmlHelp.DirtyString( PWLib.XmlHelp.GetAttribute( parentNode, VolumeSnapshotXml.XmlNameElement, "" ) );
			mRelativePath = PWLib.XmlHelp.DirtyString( PWLib.XmlHelp.GetAttribute( parentNode, VolumeSnapshotXml.XmlRelativePathElement, "" ) );
			mRevision = VolumeSnapshotRevision.Create( PWLib.XmlHelp.GetAttribute( parentNode, VolumeSnapshotXml.XmlRevisionElement, "" ) );

			long ticks;
			if ( long.TryParse( PWLib.XmlHelp.GetAttribute( parentNode, VolumeSnapshotXml.XmlLastModifiedElement, "0" ), out ticks ) )
				mLastModified = new DateTime( ticks );

			foreach ( XmlNode childNode in parentNode.ChildNodes )
			{
				string lowerName = childNode.Name.ToLower();
				if ( lowerName == VolumeSnapshotXml.XmlDirectoryElement ) // Sub directories
				{
					try
					{
						VolumeSnapshotDirectory subDirEntry = new VolumeSnapshotDirectory( mSnapshot, childNode );
						mDirectories.Add( subDirEntry );
					}
					catch ( System.Exception e )
					{
						FileSync.__LogError( this, "VolumeSnapshotDirectory.BuildFromXml failed in directory '" + mName + "' '" + mRelativePath + "'", e );
					}
				}
				else if ( lowerName == VolumeSnapshotXml.XmlFileElement ) // Files
				{
					try
					{
						VolumeSnapshotFile fileEntry = VolumeSnapshotFile.BuildFromXml( mSnapshot, childNode );
						mFiles.Add( fileEntry );
					}
					catch ( System.Exception e )
					{
						FileSync.__LogError( this, "VolumeSnapshotDirectory.BuildFromXml failed in directory on a file '" + mName + "' '" + mRelativePath + "'", e );
					}
				}
			}
		}


		public static VolumeSnapshotDirectory BuildFromSource( VolumeSnapshot snapshot, VolumeSource source, string relativePath )
		{
			return new VolumeSnapshotDirectory( snapshot, source, relativePath );
		}


		private VolumeSnapshotDirectory( VolumeSnapshot snapshot, VolumeSource source, string relativePath )
		{
			mSnapshot = snapshot;
			mRelativePath = relativePath;
			mName = mRelativePath.Length <= 1 ? "" : PWLib.Platform.Windows.Path.GetLeafName( relativePath );
			mLastModified = source.GetLastWriteTimeUtc( relativePath );// PWLib.Platform.Windows.Directory.GetLastWriteTimeUtc( rootDir );

			foreach ( string filename in source.GetRelativePathFiles( relativePath ) /* PWLib.Platform.Windows.Directory.GetFiles( rootDir ) */ )
			{
				try
				{
					VolumeSnapshotFile fileEntry = VolumeSnapshotFile.BuildFromSource( mSnapshot, source, filename );
					mFiles.Add( fileEntry );
				}
				catch ( System.Exception e )
				{
					FileSync.__LogError( this, "VolumeSnapshotDirectory.BuildFromFileSystem failed on file '" + filename + "'", e );
				}
			}

			foreach ( string subDir in source.GetRelativePathDirectories( relativePath ) )
			{
				try
				{
					if ( !IsIgnoredDirectoryName( PWLib.Platform.Windows.Path.GetLeafName( subDir ) ) )
					{
						VolumeSnapshotDirectory subDirEntry = new VolumeSnapshotDirectory( mSnapshot, source, subDir );
						mDirectories.Add( subDirEntry );
					}
				}
				catch ( System.Exception e )
				{
					FileSync.__LogError( this, "VolumeSnapshotDirectory.BuildFromFileSystem failed on directory '" + subDir + "'", e );
				}
			}
		}


		public void OutputToXml( XmlTextWriter xmlWriter )
		{
			xmlWriter.WriteStartElement( VolumeSnapshotXml.XmlDirectoryElement );
			xmlWriter.WriteAttributeString( VolumeSnapshotXml.XmlNameElement, PWLib.XmlHelp.CleanString( mName ) );
			xmlWriter.WriteAttributeString( VolumeSnapshotXml.XmlRelativePathElement, PWLib.XmlHelp.CleanString( mRelativePath ) );
			xmlWriter.WriteAttributeString( VolumeSnapshotXml.XmlRevisionElement, mRevision.ToString() );
			xmlWriter.WriteAttributeString( VolumeSnapshotXml.XmlLastModifiedElement, mLastModified.Ticks.ToString() );

			foreach ( VolumeSnapshotFile fileEntry in mFiles )
			{
				fileEntry.OutputToXml( xmlWriter );
			}

			foreach ( VolumeSnapshotDirectory dirEntry in mDirectories )
			{
				dirEntry.OutputToXml( xmlWriter );
			}

			xmlWriter.WriteEndElement();
		}


		public VolumeSnapshotFile FindFile( string filename )
		{
			foreach ( VolumeSnapshotFile file in mFiles )
			{
				if ( string.Compare( file.FileName, filename, true ) == 0 )
					return file;
			}
			return null;
		}


		public VolumeSnapshotDirectory FindDirectory( string name )
		{
			foreach ( VolumeSnapshotDirectory dir in mDirectories )
			{
				if ( string.Compare( dir.mName, name, true ) == 0 )
					return dir;
			}
			return null;
		}


		// Returns total number of files stored (recursively) in directory. Returns total file size, and the total stored in the
		// parent's snapshot
		public ulong CountAllFiles()
		{
			ulong totalFileSize = 0;
			return CountAllFiles( ref totalFileSize );
		}


		public ulong CountAllFiles( ref ulong totalFileSize )
		{
			ulong count = 0;
			CountAllFilesPrivate( ref count, ref totalFileSize );
			return count;
		}


		private void CountAllFilesPrivate( ref ulong count, ref ulong totalFileSize )
		{
			count += (ulong)mFiles.Count;

			foreach ( VolumeSnapshotFile vsf in mFiles )
			{
				totalFileSize += vsf.FileSize;
			}

			foreach ( VolumeSnapshotDirectory vsd in mDirectories )
			{
				vsd.CountAllFilesPrivate( ref count, ref totalFileSize );
			}
		}


		public ulong DetermineTotalSizeOfStoredFiles()
		{
			ulong totalStoredFileSize = 0;
			DetermineTotalSizeOfStoredFilesPrivate( ref totalStoredFileSize );
			return totalStoredFileSize;
		}


		private void DetermineTotalSizeOfStoredFilesPrivate( ref ulong totalStoredFileSize )
		{
			foreach ( VolumeSnapshotFile vsf in mFiles )
			{
				if ( vsf.Revision == null || vsf.Revision.Equals( mSnapshot.Revision ) )
					totalStoredFileSize += vsf.FileSize;
			}

			foreach ( VolumeSnapshotDirectory vsd in mDirectories )
			{
				vsd.DetermineTotalSizeOfStoredFilesPrivate( ref totalStoredFileSize );
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;


namespace PWLib.FileSyncLib
{
	public class OpenArchive : BaseArchive
	{
		string mStoragePath;
		PWLib.Platform.Windows.FileCopyEx mFileCopyEx = new PWLib.Platform.Windows.FileCopyEx();
		ArchiveFileDelegate mArchiveFileDelegate = null;
		object mUserData = null;

		public override ArchiveType Type { get { return ArchiveType.Open; } }

		public override bool IsAvailable { get { return PWLib.Platform.Windows.Directory.Exists( mStoragePath ); } }

		public override void CancelOperation( bool cancelByUser )
		{
			mFileCopyEx.Cancel();
		}



		private string RelativePathToOnDiskPath( VolumeSnapshotRevision revision, string relativePath )
		{
			string path = mStoragePath + PWLib.Platform.Windows.Path.DirectorySeparatorChar + revision.ToString() + relativePath;
			return path;
		}


		void Init()
		{
			mFileCopyEx.CopyProgress += new PWLib.Platform.Windows.CopyProgressEventHandler( mFileCopyEx_CopyProgress );
		}


		public OpenArchive( string storagePath )
		{
			mStoragePath = storagePath;
			Init();
		}

		public OpenArchive( XmlNode parentNode )
		{
			mStoragePath = PWLib.XmlHelp.GetAttribute( parentNode, "storagepath", "" );
			Init();
		}


		void mFileCopyEx_CopyProgress( object sender, PWLib.Platform.Windows.CopyProgressEventArgs e )
		{
			if ( mArchiveFileDelegate != null )
				mArchiveFileDelegate( ( ulong )e.TotalBytesTransferred, ( ulong )e.TotalFileSize, mUserData );
		}


		private void CopyFile( string source, string target, ArchiveFileDelegate archiveFileDelegate, object userData )
		{
			mArchiveFileDelegate = archiveFileDelegate;
			mUserData = userData;
			string dirPath = PWLib.Platform.Windows.Path.GetStemName( target );
			if ( !PWLib.Platform.Windows.Directory.Exists( dirPath ) )
				PWLib.Platform.Windows.Directory.CreateDirectory( dirPath );
			mFileCopyEx.CopyFile( source, target );
		}


		public override void StoreFile( VolumeSnapshotRevision revision, string relativePath, string onDiskPath, ArchiveFileDelegate archiveFileDelegate, object userData )
		{
			CopyFile( onDiskPath, RelativePathToOnDiskPath( revision, relativePath ), archiveFileDelegate, userData );
		}


		public override void RestoreFile( VolumeSnapshotRevision revision, string fileRelativePath, string onDiskOutputPath, ArchiveFileDelegate archiveFileDelegate, object userData )
		{
			if ( onDiskOutputPath[ onDiskOutputPath.Length - 1 ] == PWLib.Platform.Windows.Path.DirectorySeparatorChar )
				onDiskOutputPath = onDiskOutputPath.Substring( 0, onDiskOutputPath.Length - 1 );
			CopyFile( RelativePathToOnDiskPath( revision, fileRelativePath ), onDiskOutputPath, archiveFileDelegate, userData );
		}


		public override void CopyFileFromRevision( VolumeSnapshotRevision sourceRevision, VolumeSnapshotRevision targetRevision, string fileRelativePath, ArchiveFileDelegate archiveFileDelegate, object userData )
		{
			CopyFile( RelativePathToOnDiskPath( sourceRevision, fileRelativePath ), RelativePathToOnDiskPath( targetRevision, fileRelativePath ), archiveFileDelegate, userData );
		}


		public override void MoveFileFromRevision( VolumeSnapshotRevision sourceRevision, VolumeSnapshotRevision targetRevision, string fileRelativePath )
		{
			PWLib.Platform.Windows.File.Move( RelativePathToOnDiskPath( sourceRevision, fileRelativePath ), RelativePathToOnDiskPath( targetRevision, fileRelativePath ) );
		}


		public override void CreateDirectory( VolumeSnapshotRevision revision, string relativePath )
		{
			string absPath = RelativePathToOnDiskPath( revision, relativePath );
			if ( !PWLib.Platform.Windows.Directory.Exists( absPath ) )
				PWLib.Platform.Windows.Directory.CreateDirectory( absPath );
		}


		public override void Dispose()
		{
		}


		protected override void OutputToXmlProtected( XmlTextWriter xmlWriter )
		{
			xmlWriter.WriteAttributeString( "storagepath", mStoragePath );
		}


		public override void DeleteRevision( VolumeSnapshotRevision revision )
		{
			try
			{
				string revisionPath = RelativePathToOnDiskPath( revision, "" );
				PWLib.Platform.Windows.Path.DeletePath( revisionPath, true );
				PWLib.Platform.Windows.Directory.Remove( revisionPath, true );
			}
			catch ( Exception )
			{ }
		}


		public override void DeleteAllRevisions()
		{
			try
			{
				PWLib.Platform.Windows.Path.DeletePath( mStoragePath, true );
				PWLib.Platform.Windows.Directory.Remove( mStoragePath, true );
			}
			catch ( Exception )
			{ }
		}


		public override string GetSnapshotXmlDir()
		{
			return mStoragePath + PWLib.Platform.Windows.Path.DirectorySeparatorChar + "snapshots";
		}
	}
}

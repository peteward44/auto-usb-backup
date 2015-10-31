using System;
using System.Collections.Generic;
using System.Text;


namespace PWLib.FileSyncLib
{
	//public class IndividualCompressedArchive : BaseArchive
	//{
	//  VolumeIdentifier mIdentifier;

	//  SevenZip.SevenZipCompressor mCompressor;


	//  public void CancelOperation()
	//  {
	//  }


	//  private string GetArchiveName( VolumeSnapshotRevision revision, string relativePath )
	//  {
	//    return mIdentifier.DataDirectory
	//      + PWLib.Platform.Windows.Path.DirectorySeparatorChar + revision.ToString() + PWLib.Platform.Windows.Path.DirectorySeparatorChar + relativePath + ".7z";
	//  }


	//  public IndividualCompressedArchive( VolumeIdentifier vid )
	//  {
	//    mIdentifier = vid;

	//    mCompressor = new SevenZip.SevenZipCompressor( PWLib.Platform.Windows.Path.GetTempPath() );
	//    mCompressor.IncludeEmptyDirectories = true;
	//    mCompressor.PreserveDirectoryRoot = false;
	//  }


	//  public void StoreFile( VolumeSnapshotRevision revision, string relativePath, string onDiskPath )
	//  {
	//    Log.WriteLine( "Storing file: '" + onDiskPath + "'" );
	//    string archivePath = GetArchiveName( revision, relativePath );
	//    PWLib.Platform.Windows.Directory.CreateDirectory( PWLib.Platform.Windows.Path.GetDirectoryName( archivePath ) );
	//    mCompressor.CompressionMode = SevenZip.CompressionMode.Create;
	//    mCompressor.CompressFiles( archivePath, onDiskPath );
	//  }


	//  public void RestoreFile( VolumeSnapshotRevision revision, string fileRelativePath, string onDiskOutputPath )
	//  {
	//    //SevenZip.SevenZipExtractor extractor = new SevenZip.SevenZipExtractor( GetArchiveName( revision, relativePath ) );

	//    //string completeOutPath = onDiskPath + relativePath;
	//    //string completeOutPathDir = PWLib.Platform.Windows.Path.GetDirectoryName( completeOutPath );
	//    //PWLib.Platform.Windows.Directory.CreateDirectory( completeOutPathDir );
	//    //extractor.ExtractArchive( completeOutPathDir );
	//    //extractor.Dispose();
	//  }


	//  public void CreateDirectory( VolumeSnapshotRevision revision, string relativePath )
	//  {
	//    PWLib.Platform.Windows.Directory.CreateDirectory( PWLib.Platform.Windows.Path.GetDirectoryName( GetArchiveName( revision, relativePath ) ) );
	//  }


	//  public void Dispose()
	//  {
	//  }
	//}
}

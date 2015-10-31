using System;
using System.Collections.Generic;
using System.Text;


namespace PWLib.FileSyncLib
{
	//public class OneBigCompressedArchiveAddOneFileAtATime7Zip : BaseArchive
	//{
	//  VolumeIdentifier mIdentifier;

	//  SevenZip.SevenZipCompressor mCompressor;


	//  public void CancelOperation()
	//  {
	//  }


	//  private string GetArchiveName( VolumeSnapshotRevision revision )
	//  {
	//    return mIdentifier.DataDirectory
	//      + PWLib.Platform.Windows.Path.DirectorySeparatorChar + revision.ToString() + ".7z";
	//  }


	//  public OneBigCompressedArchiveAddOneFileAtATime7Zip( VolumeIdentifier vid )
	//  {
	//    mIdentifier = vid;

	//    mCompressor = new SevenZip.SevenZipCompressor( PWLib.Platform.Windows.Path.GetTempPath() );
	//    mCompressor.IncludeEmptyDirectories = true;
	//    mCompressor.PreserveDirectoryRoot = false;
	//  }


	//  public void StoreFile( VolumeSnapshotRevision revision, string relativePath, string onDiskPath )
	//  {
	//    Log.WriteLine( "Storing file: '" + onDiskPath + "'" );
	//    string archivePath = GetArchiveName( revision );
	//    mCompressor.CompressionMode = PWLib.Platform.Windows.File.Exists( archivePath ) ? SevenZip.CompressionMode.Append : SevenZip.CompressionMode.Create;

	//    int rootLength = onDiskPath.Length - relativePath.Length + 1;
	//    if ( rootLength > onDiskPath.Length )
	//      rootLength = onDiskPath.Length;
	//    else if ( rootLength < 0 )
	//      rootLength = 0;

	//    mCompressor.CompressFiles( archivePath, rootLength, onDiskPath );
	//  }


	//  public void RestoreFile( VolumeSnapshotRevision revision, string fileRelativePath, string onDiskOutputPath )
	//  {
	//    //SevenZip.SevenZipExtractor extractor = new SevenZip.SevenZipExtractor( GetArchiveName( revision ) );

	//    //string completeOutPath = onDiskPath + relativePath;
	//    //string completeOutPathDir = PWLib.Platform.Windows.Path.GetDirectoryName( completeOutPath );
	//    //PWLib.Platform.Windows.Directory.CreateDirectory( completeOutPathDir );

	//    //string trimmedPath = relativePath;
	//    //if ( trimmedPath[ 0 ] == PWLib.Platform.Windows.Path.DirectorySeparatorChar )
	//    //  trimmedPath = trimmedPath.Substring( 1 );
	//    //int index = 0;
	//    //foreach ( string filename in extractor.ArchiveFileNames )
	//    //{
	//    //  if ( string.Compare( trimmedPath, filename, true ) == 0 )
	//    //  {
	//    //    System.IO.FileStream fs = new System.IO.FileStream( completeOutPath, System.IO.FileMode.Create );
	//    //    extractor.ExtractFile( index, fs );
	//    //    break;
	//    //  }
	//    //  index++;
	//    //}

	//    //extractor.Dispose();
	//  }


	//  public void CreateDirectory( VolumeSnapshotRevision revision, string relativePath )
	//  {
	//  }


	//  public void Dispose()
	//  {
	//  }
	//}

	///// <summary>
	///// //////////////////////////////////////////////////////////////////////////////////////////////////////
	///// </summary>

	//public class OneBigCompressedArchiveCompressAfterwards7Zip : BaseArchive
	//{
	//  VolumeIdentifier mIdentifier;

	//  SevenZip.SevenZipCompressor mCompressor;


	//  public void CancelOperation()
	//  {
	//  }


	//  private string GetArchiveName( VolumeSnapshotRevision revision )
	//  {
	//    return mIdentifier.DataDirectory
	//      + PWLib.Platform.Windows.Path.DirectorySeparatorChar + revision.ToString() + ".7z";
	//  }


	//  public OneBigCompressedArchiveCompressAfterwards7Zip( VolumeIdentifier vid )
	//  {
	//    mIdentifier = vid;

	//    mCompressor = new SevenZip.SevenZipCompressor( PWLib.Platform.Windows.Path.GetTempPath() );
	//    mCompressor.IncludeEmptyDirectories = true;
	//    mCompressor.PreserveDirectoryRoot = false;
	//  }


	//  public void StoreFile( VolumeSnapshotRevision revision, string relativePath, string onDiskPath )
	//  {
	////		CopyFile( onDiskPath, GetTemporaryPath( revision, relativePath ) );
	//  }


	//  public void RestoreFile( VolumeSnapshotRevision revision, string fileRelativePath, string onDiskOutputPath )
	//  {
	//    //if ( onDiskPath[ onDiskPath.Length - 1 ] == Path.DirectorySeparatorChar )
	//    //  onDiskPath = onDiskPath.Substring( 0, onDiskPath.Length - 1 );
	//    //onDiskPath += relativePath;
	//    //CopyFile( GetTemporaryPath( revision, relativePath ), onDiskPath );
	//  }


	//  public void CreateDirectory( VolumeSnapshotRevision revision, string relativePath )
	//  {
	//  }


	//  public void Dispose()
	//  {
	//  }
	//}
}

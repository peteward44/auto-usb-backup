using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32.SafeHandles;
using IO = System.IO;



namespace PWLib.Platform.Windows
{

	#region New file replacements


	internal class PathInternal
	{
		public const UInt32 OurMaxPath = 32767;


		internal struct DllImport
		{
			[StructLayout( LayoutKind.Sequential, CharSet = CharSet.Unicode )]
			public struct FILETIME
			{
				public UInt32 dwLowDateTime;
				public UInt32 dwHighDateTime;
			}

			[StructLayout( LayoutKind.Sequential, CharSet = CharSet.Unicode )]
			public struct WIN32_FIND_DATA
			{
				public UInt32 dwFileAttributes;
				public FILETIME ftCreationTime;
				public FILETIME ftLastAccessTime;
				public FILETIME ftLastWriteTime;
				public UInt32 nFileSizeHigh;
				public UInt32 nFileSizeLow;
				public UInt32 dwReserved0;
				public UInt32 dwReserved1;
				[MarshalAs( UnmanagedType.ByValTStr, SizeConst = 260 )]
				public string cFileName;
				[MarshalAs( UnmanagedType.ByValTStr, SizeConst = 16 )]
				public string cAlternateFileName;
			}

			[StructLayout( LayoutKind.Sequential, CharSet = CharSet.Unicode )]
			public struct WIN32_FILE_ATTRIBUTE_DATA
			{
				public UInt32 dwFileAttributes;
				public FILETIME ftCreationTime;
				public FILETIME ftLastAccessTime;
				public FILETIME ftLastWriteTime;
				public UInt32 nFileSizeHigh;
				public UInt32 nFileSizeLow;
			}


			[DllImport( "kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode )]
			public static extern bool MoveFileW( string lpExistingFileName, string lpNewFileName );

			[DllImport( "kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode )]
			public static extern bool CopyFileW( string lpExistingFileName, string lpNewFileName, bool failIfExists );

			[DllImport( "kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode )]
			public static extern bool DeleteFileW( string lpFileName );

			[DllImport( "kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode )]
			public static extern bool RemoveDirectoryW( string lpPathName );

			[DllImport( "kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode )]
			public static extern bool CreateDirectoryW( string lpPathName, IntPtr lpSecurityAttributes );

			[DllImport( "kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode )]
			public static extern SafeFileHandle CreateFileW( string lpFileName, uint dwDesiredAccess,
																						uint dwShareMode, IntPtr lpSecurityAttributes, uint dwCreationDisposition,
																						uint dwFlagsAndAttributes, IntPtr hTemplateFile );

			[DllImport( "kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode )]
			public static extern uint SetFilePointer( SafeFileHandle hFile, long lDistanceToMove, IntPtr lpDistanceToMoveHigh, uint dwMoveMethod );

			[DllImport( "kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode )]
			public static extern UInt32 GetFileAttributesW( string filename );

			[DllImport( "kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode )]
			public static extern bool SetFileAttributesW( string lpFileName, UInt32 dwFileAttributes );

			[DllImport( "kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode )]
			public static extern bool GetFileAttributesExW( string filename, UInt32 fInfoLevelId, ref WIN32_FILE_ATTRIBUTE_DATA lpFileInformation );

			[DllImport( "kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode )]
			public static unsafe extern bool SetFileTime( SafeFileHandle hFile, FILETIME* lpCreationTime, FILETIME* lpLastAccessTime, FILETIME* lpLastWriteTime );

			[DllImport( "kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode )]
			public static extern bool GetFileSizeEx( SafeHandle hFile, ref ulong lpFileSize );

			[DllImport( "kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode )]
			public static extern IntPtr FindFirstFileExW( string lpFileName, int fInfoLevelId, ref WIN32_FIND_DATA lpFindFileData,
				int fSearchOp, IntPtr lpSearchFilter, UInt32 swAdditionalFlags );

			[DllImport( "kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode )]
			public static extern bool FindClose( IntPtr hFindFile );

			[DllImport( "kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode )]
			public static extern bool FindNextFile( IntPtr hFindFile, ref WIN32_FIND_DATA lpFindFileData );

			[DllImport( "kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode )]
			public static unsafe extern bool GetFullPathNameW( string lpFileName, UInt32 nBufferLength, char* lpBuffer, IntPtr lpFilePart );


			public const uint CREATE_NEW = 1;
			public const uint CREATE_ALWAYS = 2;
			public const uint OPEN_EXISTING = 3;
			public const uint OPEN_ALWAYS = 4;
			public const uint TRUNCATE_EXISTING = 5;

			public const uint GENERIC_READ = 0x80000000;
			public const uint GENERIC_WRITE = 0x40000000;
			public const uint GENERIC_EXECUTE = 0x20000000;
			public const uint GENERIC_ALL = 0x10000000;

			public const uint FILE_SHARE_READ = 0x00000001;
			public const uint FILE_SHARE_WRITE = 0x00000002;
			public const uint FILE_SHARE_DELETE = 0x00000004;

			public const uint FILE_APPEND_DATA = 0x0004;
			public const uint FILE_ATTRIBUTE_READONLY = 0x00000001;
			public const uint FILE_ATTRIBUTE_NORMAL = 0x00000080;
			public const uint ERROR_ALREADY_EXISTS = 183;
			public const uint FILE_END = 2;

			public const uint INVALID_FILE_ATTRIBUTES = 0xffffffff;
			public const uint FILE_ATTRIBUTE_DIRECTORY = 0x0010;
		}


		static string RootPathIfNecessary( string path )
		{
			return PWLib.Platform.Windows.Path.GetFullPath( path, false );
		}


		public static string ConvertToUnicodePath( string path )
		{
			// If file path is disk file path then prepend it with \\?\
			// if file path is UNC prepend it with \\?\UNC\ and remove \\ prefix in unc path.
			if ( path.StartsWith( @"\\?\" ) )
				return path;
			else if ( path.StartsWith( @"\\" ) )
				return @"\\?\UNC\" + path.Substring( 2, path.Length - 2 );
			else
				return @"\\?\" + RootPathIfNecessary( path );
		}

		public static DateTime FILETIMEToDateTime( DllImport.FILETIME fileTime, bool utc )
		{
			UInt64 high = ( (UInt64)( fileTime.dwHighDateTime ) );
			UInt64 shiftedHigh = ( high & 0xFFFFFFFF ) << 32;
			UInt64 low = (UInt64)( fileTime.dwLowDateTime & ( 0xFFFFFFFF ) );
			long final = (long)( low | shiftedHigh );
			return utc ? DateTime.FromFileTimeUtc( final ) : DateTime.FromFileTime( final );
		}

		public static UInt32 GetFileAttributes( string filename )
		{
			return PathInternal.DllImport.GetFileAttributesW( PathInternal.ConvertToUnicodePath( filename ) );
		}

		public static bool SetFileAttributes( string filename, UInt32 attribs )
		{
			return PathInternal.DllImport.SetFileAttributesW( PathInternal.ConvertToUnicodePath( filename ), attribs );
		}


		public static void RemoveReadOnlyFlag( string filename )
		{
			UInt32 attribs = PathInternal.GetFileAttributes( filename );
			if ( attribs != PathInternal.DllImport.INVALID_FILE_ATTRIBUTES && ( attribs & PathInternal.DllImport.FILE_ATTRIBUTE_READONLY ) > 0 )
			{
				PathInternal.SetFileAttributes( filename, ( attribs & ( ~PathInternal.DllImport.FILE_ATTRIBUTE_READONLY ) ) );
			}
		}

	}



  public class File
  {

		#region Win32 helper methods

		private static uint GetMode( IO.FileMode mode )
		{
			uint umode = 0;
			switch ( mode )
			{
				case IO.FileMode.CreateNew:
					umode = PathInternal.DllImport.CREATE_NEW;
					break;
				case IO.FileMode.Create:
					umode = PathInternal.DllImport.CREATE_ALWAYS;
					break;
				case IO.FileMode.Append:
					umode = PathInternal.DllImport.OPEN_ALWAYS;
					break;
				case IO.FileMode.Open:
					umode = PathInternal.DllImport.OPEN_EXISTING;
					break;
				case IO.FileMode.OpenOrCreate:
					umode = PathInternal.DllImport.OPEN_ALWAYS;
					break;
				case IO.FileMode.Truncate:
					umode = PathInternal.DllImport.TRUNCATE_EXISTING;
					break;
			}
			return umode;
		}

		private static uint GetAccess( IO.FileAccess access )
		{
			uint uaccess = 0;
			switch ( access )
			{
				case IO.FileAccess.Read:
					uaccess = PathInternal.DllImport.GENERIC_READ;
					break;
				case IO.FileAccess.ReadWrite:
					uaccess = PathInternal.DllImport.GENERIC_READ | PathInternal.DllImport.GENERIC_WRITE;
					break;
				case IO.FileAccess.Write:
					uaccess = PathInternal.DllImport.GENERIC_WRITE;
					break;
			}
			return uaccess;
		}

		private static uint GetShare( IO.FileShare share )
		{
			uint ushare = 0;
			switch ( share )
			{
				case IO.FileShare.Read:
					ushare = PathInternal.DllImport.FILE_SHARE_READ;
					break;
				case IO.FileShare.ReadWrite:
					ushare = PathInternal.DllImport.FILE_SHARE_READ | PathInternal.DllImport.FILE_SHARE_WRITE;
					break;
				case IO.FileShare.Write:
					ushare = PathInternal.DllImport.FILE_SHARE_WRITE;
					break;
				case IO.FileShare.Delete:
					ushare = PathInternal.DllImport.FILE_SHARE_DELETE;
					break;
				case IO.FileShare.None:
					ushare = 0;
					break;
			}
			return ushare;
		}

		#endregion


		#region Open file methods


		public static IO.FileStream Create( string filename )
		{
			return Open( filename, System.IO.FileMode.Create, System.IO.FileAccess.ReadWrite, System.IO.FileShare.ReadWrite );
		}

		public static IO.FileStream OpenRead( string filepath )
		{
			return Open( filepath, IO.FileMode.Open, IO.FileAccess.Read, IO.FileShare.Read );
		}

		public static IO.FileStream OpenWrite( string filepath )
		{
			return Open( filepath, IO.FileMode.OpenOrCreate, IO.FileAccess.Write, IO.FileShare.None );
		}

		public static IO.FileStream Open( string filepath, IO.FileMode mode, IO.FileAccess access )
		{
			return Open( filepath, mode, access, IO.FileShare.ReadWrite );
		}

		public static IO.FileStream Open( string filepath, IO.FileMode mode, IO.FileAccess access, IO.FileShare share )
		{
			//opened in the specified mode , access and  share
			IO.FileStream fs = null;
			uint umode = GetMode( mode );
			uint uaccess = GetAccess( access );
			uint ushare = GetShare( share );
			if ( mode == IO.FileMode.Append )
				uaccess = PathInternal.DllImport.FILE_APPEND_DATA;

			SafeFileHandle sh = PathInternal.DllImport.CreateFileW( PathInternal.ConvertToUnicodePath( filepath ), uaccess, ushare, IntPtr.Zero, umode, PathInternal.DllImport.FILE_ATTRIBUTE_NORMAL, IntPtr.Zero );
			int iError = Marshal.GetLastWin32Error();
			if ( ( iError > 0 && !( mode == IO.FileMode.Append && iError != PathInternal.DllImport.ERROR_ALREADY_EXISTS ) ) || sh.IsInvalid )
			{
				throw new Exception( "Error opening file Win32 Error:" + iError );
			}
			else
			{
				fs = new IO.FileStream( sh, access );
			}
			// if opened in append mode
			if ( mode == IO.FileMode.Append )
			{
				if ( !sh.IsInvalid )
				{
					PathInternal.DllImport.SetFilePointer( sh, 0, IntPtr.Zero, PathInternal.DllImport.FILE_END );
				}
			}
			return fs;
		}


		#endregion


		public static ulong GetFileSize( string path )
		{
			ulong num = 0;
			try
			{
				IO.FileStream fs = OpenRead( path );
				if ( !PathInternal.DllImport.GetFileSizeEx( fs.SafeFileHandle, ref num ) )
					num = 0;
				fs.Close();
			}
			catch ( System.Exception )
			{
			}
			return num;
		}


    public static bool Exists(string filename)
    {
			UInt32 attribs = PathInternal.GetFileAttributes( filename );
			return attribs != PathInternal.DllImport.INVALID_FILE_ATTRIBUTES && ( ( attribs & PathInternal.DllImport.FILE_ATTRIBUTE_DIRECTORY ) == 0 );
    }


		//public static bool CopyFile( string existingFile, string newFile, bool failIfExists )
		//{
		//  return PathInternal.DllImport.CopyFileW( PathInternal.ConvertToUnicodePath( existingFile ), PathInternal.ConvertToUnicodePath( newFile ), failIfExists );
		//}

		public static bool Delete( string filename )
		{
			return Delete( filename, false );
		}

		public static bool Delete( string filename, bool force )
    {
			if ( force )
			{
				PathInternal.RemoveReadOnlyFlag( filename );
			}
			return PathInternal.DllImport.DeleteFileW( PathInternal.ConvertToUnicodePath( filename ) );
    }


    public static bool Move(string source, string dest)
    {
			return PathInternal.DllImport.MoveFileW( PathInternal.ConvertToUnicodePath( source ), PathInternal.ConvertToUnicodePath( dest ) );
    }



		private enum DateTimeAttrib { Write, Access, Creation };


		private static DateTime GetDateTime( string path, DateTimeAttrib attrib, bool utc )
		{
			PathInternal.DllImport.WIN32_FILE_ATTRIBUTE_DATA fileData = new PathInternal.DllImport.WIN32_FILE_ATTRIBUTE_DATA();
			if ( PathInternal.DllImport.GetFileAttributesExW( PathInternal.ConvertToUnicodePath( path ), 0, ref fileData ) )
			{
				switch ( attrib )
				{
					case DateTimeAttrib.Write:
						return PathInternal.FILETIMEToDateTime( fileData.ftLastWriteTime, utc );
					case DateTimeAttrib.Access:
						return PathInternal.FILETIMEToDateTime( fileData.ftLastAccessTime, utc );
					case DateTimeAttrib.Creation:
						return PathInternal.FILETIMEToDateTime( fileData.ftCreationTime, utc );
					default:
						throw new Exception( "Unknown DateTimeAttrib in File.GetDateTime() function" );
				}
			}
			else
				throw new IO.FileNotFoundException( path );
		}


		private static unsafe void SetDateTime( string path, DateTime dateTime, DateTimeAttrib attrib, bool utc )
		{
			long rawFileTime = utc ? dateTime.ToFileTimeUtc() : dateTime.ToFileTime();
			PathInternal.DllImport.FILETIME fileTime = new PathInternal.DllImport.FILETIME();

			fileTime.dwLowDateTime = (uint)( rawFileTime & 0xFFFFFFFF );
			fileTime.dwHighDateTime = (uint)( ( rawFileTime >> 32 ) & 0xFFFFFFFF );

			IO.FileStream fs = OpenRead( path );

			switch ( attrib )
			{
				case DateTimeAttrib.Access:
					PathInternal.DllImport.SetFileTime( fs.SafeFileHandle, (PathInternal.DllImport.FILETIME*)0, &fileTime, (PathInternal.DllImport.FILETIME*)0 );
					break;
				case DateTimeAttrib.Creation:
					PathInternal.DllImport.SetFileTime( fs.SafeFileHandle, &fileTime, (PathInternal.DllImport.FILETIME*)0, (PathInternal.DllImport.FILETIME*)0 );
					break;
				case DateTimeAttrib.Write:
					PathInternal.DllImport.SetFileTime( fs.SafeFileHandle, (PathInternal.DllImport.FILETIME*)0, (PathInternal.DllImport.FILETIME*)0, &fileTime );
					break;
				default:
					throw new Exception( "Unknown DateTimeAttrib in File.SetDateTime() function" );
			}

			fs.Close();
		}


    public static void SetLastWriteTime(string filename, DateTime dateTime )
		{
			SetDateTime( filename, dateTime, DateTimeAttrib.Write, false );
		}

		public static void SetLastWriteTimeUtc( string filename, DateTime dateTime )
		{
			SetDateTime( filename, dateTime, DateTimeAttrib.Write, true );
		}

		public static void SetLastAccessTime( string filename, DateTime dateTime )
    {
			SetDateTime( filename, dateTime, DateTimeAttrib.Access, false );
		}

		public static void SetLastAccessTimeUtc( string filename, DateTime dateTime )
		{
			SetDateTime( filename, dateTime, DateTimeAttrib.Access, true );
		}

		public static void SetCreationTime( string filename, DateTime dateTime )
    {
			SetDateTime( filename, dateTime, DateTimeAttrib.Creation, false );
		}

		public static void SetCreationTimeUtc( string filename, DateTime dateTime )
		{
			SetDateTime( filename, dateTime, DateTimeAttrib.Creation, true );
		}




		public static DateTime GetLastWriteTime( string path )
		{
			return GetDateTime( path, DateTimeAttrib.Write, false );
		}

		public static DateTime GetLastWriteTimeUtc( string path )
		{
			return GetDateTime( path, DateTimeAttrib.Write, true );
		}

		public static DateTime GetLastAccessTime( string path )
		{
			return GetDateTime( path, DateTimeAttrib.Access, false );
		}

		public static DateTime GetLastAccessTimeUtc( string path )
		{
			return GetDateTime( path, DateTimeAttrib.Access, true );
		}

		public static DateTime GetCreationTime( string path )
		{
			return GetDateTime( path, DateTimeAttrib.Creation, false );
		}

		public static DateTime GetCreationTimeUtc( string path )
		{
			return GetDateTime( path, DateTimeAttrib.Creation, true );
		}
  }


  public class Directory
  {
    public static bool Exists(string filename)
    {
			UInt32 attribs = PathInternal.GetFileAttributes( filename );
			return attribs != PathInternal.DllImport.INVALID_FILE_ATTRIBUTES && ( attribs & PathInternal.DllImport.FILE_ATTRIBUTE_DIRECTORY ) > 0;
    }

    public static void CreateDirectory(string filename)
    {
			// peel back the path until we find a directory that exists
			string modPath = filename;
			Stack<string> directoryList = new Stack<string>();
			while ( !Exists( modPath ) )
			{
				int index1 = modPath.LastIndexOf( Path.DirectorySeparatorChar );
				if ( index1 < 0 )
					break;
				else
				{
					string dirName = modPath.Substring( index1 + 1 );
					directoryList.Push( dirName );
					modPath = modPath.Substring( 0, index1 );
				}
			}

			while ( directoryList.Count > 0 )
			{
				string dirName = directoryList.Pop();
				modPath += Path.DirectorySeparatorChar + dirName;
				PathInternal.DllImport.CreateDirectoryW( PathInternal.ConvertToUnicodePath( modPath ), IntPtr.Zero );
			}
    }


		public static string GetCurrentDirectory()
		{
			return IO.Directory.GetCurrentDirectory(); // safe
		}


		public static bool Move( string existingDir, string newDir )
		{
			// can use same method for moving files
			return File.Move( existingDir, newDir );
		}


		public static DateTime GetLastWriteTimeUtc( string path )
		{
			return File.GetLastWriteTimeUtc( path );
		}


		public static string[] GetFiles( string path )
		{
			return GetFiles( path, "*" );
		}


		public static string[] GetFiles( string path, string searchPattern )
		{
			List<string> list = new List<string>();

			PathInternal.DllImport.WIN32_FIND_DATA findData = new PathInternal.DllImport.WIN32_FIND_DATA();
			IntPtr findFileHandle = PathInternal.DllImport.FindFirstFileExW( PathInternal.ConvertToUnicodePath( Path.Combine( path, searchPattern ) ), 0, ref findData, 0, (IntPtr)0, 0 );

			if ( findFileHandle.ToInt64() > 0 )
			{
				do
				{
					if ( ( findData.dwFileAttributes & PathInternal.DllImport.FILE_ATTRIBUTE_DIRECTORY ) == 0 )
						list.Add( Path.Combine( path, findData.cFileName ) );
				} while ( PathInternal.DllImport.FindNextFile( findFileHandle, ref findData ) );

				PathInternal.DllImport.FindClose( findFileHandle );
			}

			return list.ToArray();
		}


		public static string[] GetDirectories( string path )
		{
			return GetDirectories( path, "*" );
		}
		

		public static string[] GetDirectories( string path, string searchPattern )
		{
			List<string> list = new List<string>();

			PathInternal.DllImport.WIN32_FIND_DATA findData = new PathInternal.DllImport.WIN32_FIND_DATA();
			IntPtr findFileHandle = PathInternal.DllImport.FindFirstFileExW( PathInternal.ConvertToUnicodePath( Path.Combine( path, searchPattern ) ), 0, ref findData, 0, (IntPtr)0, 0 );

			if ( findFileHandle.ToInt64() > 0 )
			{
				do
				{
					if ( ( findData.dwFileAttributes & PathInternal.DllImport.FILE_ATTRIBUTE_DIRECTORY ) > 0
						&& findData.cFileName != "." && findData.cFileName != ".." )
					{
						list.Add( Path.Combine( path, findData.cFileName ) );
					}
				} while ( PathInternal.DllImport.FindNextFile( findFileHandle, ref findData ) );

				PathInternal.DllImport.FindClose( findFileHandle );
			}

			return list.ToArray();
		}


		public static bool Remove( string path, bool force )
		{
			if ( force )
				PathInternal.RemoveReadOnlyFlag( path );
			return PathInternal.DllImport.RemoveDirectoryW( PathInternal.ConvertToUnicodePath( path ) );
		}

  }


  public class FileInfo
  {
    string mFullPath;

		public string FullName { get { return mFullPath; } }
		public string Name { get { return Path.GetFileName( mFullPath ); } }
		public string Extension { get { return Path.GetExtension( mFullPath ); } }
		public IO.FileAttributes Attributes { get { return (IO.FileAttributes)PathInternal.GetFileAttributes( mFullPath ); } }
    public long Length { get { return (long)File.GetFileSize( mFullPath ); } }
    public DateTime CreationTime { get { return File.GetCreationTime( mFullPath ); } }
		public DateTime LastAccessTime { get { return File.GetLastAccessTime( mFullPath ); } }
		public DateTime LastWriteTime { get { return File.GetLastWriteTime( mFullPath ); } }


    public FileInfo(string filename)
    {
			mFullPath = Path.GetFullPath( filename );
    }

    public bool Exists
    {
			get { return File.Exists( mFullPath ); }
    }


    public IO.FileStream OpenWrite()
    {
			return File.OpenWrite( mFullPath );
    }

  }


  public class DirectoryInfo
  {
		string mFullPath;

		public string FullName { get { return mFullPath; } }
	//	public string Name { get { return Path.GetDirectoryOnlyName( mFullPath ); } }


    public DirectoryInfo(string name)
    {
			mFullPath = Path.GetFullPath( name );
    }


		private FileInfo[] ConvertArray( string[] filenames )
		{
			FileInfo[] array = new FileInfo[ filenames.Length ];
			for ( int i = 0; i < filenames.Length; ++i )
			{
				array[ i ] = new FileInfo( filenames[ i ] );
			}
			return array;
		}


    public FileInfo[] GetFiles()
    {
			return ConvertArray( Directory.GetFiles( mFullPath ) );
    }


    public FileInfo[] GetFiles( string searchPattern )
    {
			return ConvertArray( Directory.GetFiles( mFullPath, searchPattern ) );
    }


    public DirectoryInfo[] GetDirectories()
    {
			string[] filenames = Directory.GetDirectories( mFullPath );
			DirectoryInfo[] array = new DirectoryInfo[ filenames.Length ];
			for ( int i = 0; i < filenames.Length; ++i )
			{
				array[ i ] = new DirectoryInfo( filenames[ i ] );
			}
			return array;
    }
  }


  public class Path
  {
    public static readonly char DirectorySeparatorChar = IO.Path.DirectorySeparatorChar;
	public static readonly char AltDirectorySeperatorChar = IO.Path.AltDirectorySeparatorChar;


	public static string GetLeafName( string path )
	{
		int index1 = path.LastIndexOf( Path.DirectorySeparatorChar );
		if ( index1 < 0 )
			return path;
		else if ( index1 == path.Length - 1 )
		{
			return GetLeafName( path.Substring( 0, index1 ) );
		}
		else
			return path.Substring( index1+1 );
	}


	public static string GetStemName( string path )
	{
		int index1 = path.LastIndexOf( Path.DirectorySeparatorChar );
		if ( index1 < 0 )
			return path;
		else if ( index1 == path.Length - 1 )
		{
			return GetStemName( path.Substring( 0, index1 ) );
		}
		else
			return path.Substring( 0, index1 );
	}

		//public static string GetDirectoryName( string path )
		//{
		//    if ( path == null )
		//        return path;
		//    else if ( Directory.Exists( path ) )
		//        return path;
		//    else
		//    {
		//        int index1 = path.LastIndexOf( Path.DirectorySeparatorChar );
		//        if (index1 < 0)
		//            return path;
		//        else if(index1 == path.Length-1)
		//            return path; // path.Substring(0, path.Length - 1);
		//        else
		//            return path.Substring( 0, index1 );
		//    }
		//}


    public static char[] GetInvalidFileNameChars()
    {
        return IO.Path.GetInvalidFileNameChars();
    }


		public static string GetTempPath()
		{
			return IO.Path.GetTempPath();
		}


	//    public static string GetDirectoryOnlyName( string path )
	//{
	//        string directoryPath = GetDirectoryName( path );
	//        int index1 = directoryPath.LastIndexOf( Path.DirectorySeparatorChar );
	//        if ( index1 < 0 || index1 == directoryPath.Length - 1 )
	//        {
	//            if ( directoryPath.Length == 0 )
	//            {
	//                if ( path.StartsWith( Path.DirectorySeparatorChar.ToString() ) || path.StartsWith( Path.AltDirectorySeperatorChar.ToString() ) )
	//                {
	//                    return path.Substring( 1 );
	//                }
	//                else
	//                    return path;
	//            }
	//            else
	//                return directoryPath;
	//        }
	//        return directoryPath.Substring( index1 + 1 );
	//}


    public static string GetExtension(string path)
    {
			if ( path == null )
				return null;
			int index1 = path.LastIndexOf( '.' );
			if ( index1 < 0 )
				return "";
			return path.Substring( index1 );
    }


		public static string GetFileNameWithoutExtension( string path )
		{
			if ( path == null )
				return null;
			string modPath = GetFileName( path );
			int index1 = modPath.LastIndexOf( '.' );
			if ( index1 < 0 )
				return modPath;
			return modPath.Substring( 0, index1 );
		}


		public static string GetFullPath( string path )
		{
			return GetFullPath( path, true );
		}

    public unsafe static string GetFullPath(string path, bool useUnicodePath)
    {
			fixed ( char* buffer = new char[ PathInternal.OurMaxPath ] )
			{
				PathInternal.DllImport.GetFullPathNameW( useUnicodePath ? PathInternal.ConvertToUnicodePath( path ) : path, PathInternal.OurMaxPath, buffer, (IntPtr)0 );
				return new string( buffer );
			}
    }

		public static string GetFileName( string filename )
		{
			return IO.Path.GetFileName( filename ); // Will not throw too long exception
		}


		public static string Combine( string path1, string path2 )
		{
			return IO.Path.Combine( path1, path2 ); // Will not throw too long exception
		}


		private static string GetLastErrorString()
		{
			return ( new System.ComponentModel.Win32Exception( Marshal.GetLastWin32Error() ) ).Message;
		}

		private static void CallCallback( DeletePathDelegate callback, string path, bool isFile, bool success )
		{
			if ( callback != null )
				callback( path, isFile, success, success ? "" : GetLastErrorString() );
		}

		private static void DeletePathRecurse( string directory, bool force, DeletePathDelegate callback )
		{
			foreach ( string file in Directory.GetFiles( directory ) )
			{
				CallCallback( callback, file, true, File.Delete( file, force ) );
			}

			foreach ( string subdir in Directory.GetDirectories( directory ) )
			{
				System.IO.Directory.Delete( subdir, true );
				CallCallback( callback, subdir, false, true );
			}
		}


		public static void DeletePath( string path, bool force )
		{
			DeletePath( path, force, null );
		}


		public delegate void DeletePathDelegate( string path, bool isFile, bool succeeded, string errorMessage );


		public static void DeletePath( string path, bool force, DeletePathDelegate callback )
		{
			if ( File.Exists( path ) )
			{
				bool success = File.Delete( path, force );
				CallCallback( callback, path, true, success );
			}
			else
			{
				DeletePathRecurse( path, force, callback );
				CallCallback( callback, path, false, Directory.Remove( path, force ) );
			}
		}


		static void CountFilesRecurse( string path, ref ulong count )
		{
			count += (ulong)Directory.GetFiles( path ).Length;

			foreach ( string subdir in Directory.GetDirectories( path ) )
			{
				CountFilesRecurse( subdir, ref count );
			}
		}


		public static ulong CountFiles( string path )
		{
			if ( File.Exists( path ) )
			{
				return 1;
			}
			else
			{
				ulong count=0;
				CountFilesRecurse( path, ref count );
				return count;
			}
		}
  }


	#endregion



	#region CopyFileEx


	/// <summary>
	/// /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// </summary>


	public class FileCopyEx
	{
		#region Constants

		//constants that specify how the file is to be copied

		private const uint COPY_FILE_ALLOW_DECRYPTED_DESTINATION = 0x00000008;
		private const uint COPY_FILE_FAIL_IF_EXISTS = 0x00000001;
		private const uint COPY_FILE_OPEN_SOURCE_FOR_WRITE = 0x00000004;
		private const uint COPY_FILE_RESTARTABLE = 0x00000002;

		/// <summary>
		/// Callback reason passed as dwCallbackReason in CopyProgressRoutine.
		/// Indicates another part of the data file was copied.
		/// </summary>
		private const uint CALLBACK_CHUNK_FINISHED = 0;

		/// <summary>
		/// Callback reason passed as dwCallbackReason in CopyProgressRoutine.
		/// Indicates another stream was created and is about to be copied. This is the callback reason given when the callback routine is first invoked.
		/// </summary>
		private const uint CALLBACK_STREAM_SWITCH = 1;

		/// <summary>
		/// Return value of the CopyProgressRoutine.
		/// Indicates continue the copy operation.
		/// </summary>
		private const uint PROGRESS_CONTINUE = 0;

		/// <summary>
		/// Return value of the CopyProgressRoutine.
		/// Indicates cancel the copy operation and delete the destination file.
		/// </summary>
		private const uint PROGRESS_CANCEL = 1;

		/// <summary>
		/// Return value of the CopyProgressRoutine.
		/// Indicates stop the copy operation. It can be restarted at a later time.
		/// </summary>
		private const uint PROGRESS_STOP = 2;

		/// <summary>
		/// Return value of the CopyProgressRoutine.
		/// Indicates continue the copy operation, but stop invoking CopyProgressRoutine to report progress.
		/// </summary>
		private const uint PROGRESS_QUIET = 3;

		#endregion


		volatile bool mCopyCancelled = false;

		public void Cancel()
		{
			mCopyCancelled = true;
		}


		#region Methods

		/// <summary>
		/// The CopyProgressRoutine delegate is an application-defined callback function used with the CopyFileEx and MoveFileWithProgress functions.
		/// It is called when a portion of a copy or move operation is completed.
		/// </summary>
		/// <param name="TotalFileSize">Total size of the file, in bytes.</param>
		/// <param name="TotalBytesTransferred">Total number of bytes transferred from the source file to the destination file since the copy operation began.</param>
		/// <param name="StreamSize">Total size of the current file stream, in bytes.</param>
		/// <param name="StreamBytesTransferred">Total number of bytes in the current stream that have been transferred from the source file to the destination file since the copy operation began. </param>
		/// <param name="dwStreamNumber">Handle to the current stream. The first time CopyProgressRoutine is called, the stream number is 1.</param>
		/// <param name="dwCallbackReason">Reason that CopyProgressRoutine was called.</param>
		/// <param name="hSourceFile">Handle to the source file.</param>
		/// <param name="hDestinationFile">Handle to the destination file.</param>
		/// <param name="lpData">Argument passed to CopyProgressRoutine by the CopyFileEx or MoveFileWithProgress function.</param>
		/// <returns>A value indicating how to proceed with the copy operation.</returns>
		protected uint CopyProgressCallback(
		long TotalFileSize,
		long TotalBytesTransferred,
		long StreamSize,
		long StreamBytesTransferred,
		uint dwStreamNumber,
		uint dwCallbackReason,
		IntPtr hSourceFile,
		IntPtr hDestinationFile,
		IntPtr lpData )
		{
			switch ( dwCallbackReason )
			{
				case CALLBACK_CHUNK_FINISHED:
					// Another part of the file was copied.
					CopyProgressEventArgs e = new CopyProgressEventArgs( TotalFileSize, TotalBytesTransferred );
					InvokeCopyProgress( e );
					return e.Cancel ? PROGRESS_CANCEL : PROGRESS_CONTINUE;

				case CALLBACK_STREAM_SWITCH:
					// A new stream was created. We don't care about this one - just continue the move operation.
					return PROGRESS_CONTINUE;

				default:
					return PROGRESS_CONTINUE;
			}
		}


		public unsafe void CopyFile( string sourceFile, string destinationFile )
		{
			mCopyCancelled = false;
			bool success = false;
#pragma warning disable 420
			fixed ( bool* cancelPtr = &mCopyCancelled )
			{
				success = CopyFileExW( PathInternal.ConvertToUnicodePath( sourceFile ), PathInternal.ConvertToUnicodePath( destinationFile ),
									new CopyProgressRoutine( this.CopyProgressCallback ), IntPtr.Zero, cancelPtr,
									COPY_FILE_ALLOW_DECRYPTED_DESTINATION );
			}
#pragma warning restore 420

			//Throw an exception if the copy failed
			if ( !success && !mCopyCancelled )
			{
				int error = Marshal.GetLastWin32Error();
				throw new System.ComponentModel.Win32Exception( error );
			}
		}

		#endregion

		#region Events

		public event CopyProgressEventHandler CopyProgress;

		protected void InvokeCopyProgress( CopyProgressEventArgs e )
		{
			if ( CopyProgress != null )
			{
				CopyProgress( this, e );
			}
		}
		#endregion

		#region P/Invoke

		[DllImport( "kernel32.dll", SetLastError = true, CharSet=CharSet.Unicode )]
		private static unsafe extern bool CopyFileExW(
		string lpExistingFileName,
		string lpNewFileName,
		CopyProgressRoutine lpProgressRoutine,
		IntPtr lpData,
		bool* pbCancel,
		uint dwCopyFlags
		);
		#endregion

		/// <summary>
		/// The CopyProgressRoutine delegate is an application-defined callback function used with the CopyFileEx and MoveFileWithProgress functions.
		/// It is called when a portion of a copy or move operation is completed.
		/// </summary>
		private delegate uint CopyProgressRoutine(
		long TotalFileSize,
		long TotalBytesTransferred,
		long StreamSize,
		long StreamBytesTransferred,
		uint dwStreamNumber,
		uint dwCallbackReason,
		IntPtr hSourceFile,
		IntPtr hDestinationFile,
		IntPtr lpData );
	}
	/// <summary>
	/// Represents the method which handles the CopyProgress event.
	/// </summary>
	public delegate void CopyProgressEventHandler( object sender, CopyProgressEventArgs e );

	/// <summary>
	/// Provides data for the CopyProgress event.
	/// </summary>
	public class CopyProgressEventArgs : EventArgs
	{
		private long _totalFileSize;
		private long _totalBytesTransferred;
		private bool _cancel = false;

		/// <summary>
		/// Initializes a new instance of the CopyProgressEventArgs class.
		/// </summary>
		/// <param name="totalFileSize">The total file size, in bytes.</param>
		/// <param name="totalBytesTransferred">The total bytes transferred so far.</param>
		public CopyProgressEventArgs( long totalFileSize, long totalBytesTransferred )
		{
			_totalFileSize = totalFileSize;
			_totalBytesTransferred = totalBytesTransferred;
		}

		/// <summary>
		/// Gets the total file size, in bytes, of the file being moved.
		/// </summary>
		/// <value>The total file size.</value>
		public long TotalFileSize
		{
			get { return _totalFileSize; }
		}

		/// <summary>
		/// Gets the total bytes transferred so far.
		/// </summary>
		/// <value>The total bytes transferred.</value>
		public long TotalBytesTransferred
		{
			get { return _totalBytesTransferred; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether the event should be canceled.
		/// </summary>
		/// <value>True if the event should be canceled, False otherwise.</value>
		public bool Cancel
		{
			get { return _cancel; }
			set { _cancel = value; }
		}
	}


	#endregion


}


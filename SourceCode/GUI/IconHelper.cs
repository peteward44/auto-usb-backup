using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace PWLib.Platform.Windows
{
	/// <summary>
	/// Provides static methods to read system icons for both folders and files.
	/// </summary>
	/// <example>
	/// <code>IconReader.GetFileIcon("c:\\general.xls");</code>
	/// </example>

	public static class IconHelper
	{
		#region Shell32 / interop stuff
		
		/// <summary>
		/// Wraps necessary Shell32.dll structures and functions required to retrieve Icon Handles using SHGetFileInfo. Code
		/// courtesy of MSDN Cold Rooster Consulting case study.
		/// </summary>
		/// 

		// This code has been left largely untouched from that in the CRC example. The main changes have been moving
		// the icon reading code over to the IconReader type.
		public class Shell32  
		{
			
			public const int 	MAX_PATH = 256;
			[StructLayout(LayoutKind.Sequential)]
				public struct SHITEMID
			{
				public ushort cb;
				[MarshalAs(UnmanagedType.LPArray)]
				public byte[] abID;
			}

			[StructLayout(LayoutKind.Sequential)]
				public struct ITEMIDLIST
			{
				public SHITEMID mkid;
			}

			[StructLayout(LayoutKind.Sequential)]
				public struct BROWSEINFO 
			{ 
				public IntPtr		hwndOwner; 
				public IntPtr		pidlRoot; 
				public IntPtr 		pszDisplayName;
				[MarshalAs(UnmanagedType.LPTStr)] 
				public string 		lpszTitle; 
				public uint 		ulFlags; 
				public IntPtr		lpfn; 
				public int			lParam; 
				public IntPtr 		iImage; 
			} 

			// Browsing for directory.
			public const uint BIF_RETURNONLYFSDIRS   =	0x0001;
			public const uint BIF_DONTGOBELOWDOMAIN  =	0x0002;
			public const uint BIF_STATUSTEXT         =	0x0004;
			public const uint BIF_RETURNFSANCESTORS  =	0x0008;
			public const uint BIF_EDITBOX            =	0x0010;
			public const uint BIF_VALIDATE           =	0x0020;
			public const uint BIF_NEWDIALOGSTYLE     =	0x0040;
			public const uint BIF_USENEWUI           =	(BIF_NEWDIALOGSTYLE | BIF_EDITBOX);
			public const uint BIF_BROWSEINCLUDEURLS  =	0x0080;
			public const uint BIF_BROWSEFORCOMPUTER  =	0x1000;
			public const uint BIF_BROWSEFORPRINTER   =	0x2000;
			public const uint BIF_BROWSEINCLUDEFILES =	0x4000;
			public const uint BIF_SHAREABLE          =	0x8000;

			[StructLayout(LayoutKind.Sequential)]
				public struct SHFILEINFO
			{ 
				public const int NAMESIZE = 80;
				public IntPtr	hIcon; 
				public int		iIcon; 
				public uint	dwAttributes; 
				[MarshalAs(UnmanagedType.ByValTStr, SizeConst=MAX_PATH)]
				public string szDisplayName; 
				[MarshalAs(UnmanagedType.ByValTStr, SizeConst=NAMESIZE)]
				public string szTypeName; 
			};

			public const uint SHGFI_ICON				= 0x000000100;     // get icon
			public const uint SHGFI_DISPLAYNAME			= 0x000000200;     // get display name
			public const uint SHGFI_TYPENAME          	= 0x000000400;     // get type name
			public const uint SHGFI_ATTRIBUTES        	= 0x000000800;     // get attributes
			public const uint SHGFI_ICONLOCATION      	= 0x000001000;     // get icon location
			public const uint SHGFI_EXETYPE           	= 0x000002000;     // return exe type
			public const uint SHGFI_SYSICONINDEX      	= 0x000004000;     // get system icon index
			public const uint SHGFI_LINKOVERLAY       	= 0x000008000;     // put a link overlay on icon
			public const uint SHGFI_SELECTED          	= 0x000010000;     // show icon in selected state
			public const uint SHGFI_ATTR_SPECIFIED    	= 0x000020000;     // get only specified attributes
			public const uint SHGFI_LARGEICON         	= 0x000000000;     // get large icon
			public const uint SHGFI_SMALLICON         	= 0x000000001;     // get small icon
			public const uint SHGFI_OPENICON          	= 0x000000002;     // get open icon
			public const uint SHGFI_SHELLICONSIZE     	= 0x000000004;     // get shell size icon
			public const uint SHGFI_PIDL              	= 0x000000008;     // pszPath is a pidl
			public const uint SHGFI_USEFILEATTRIBUTES 	= 0x000000010;     // use passed dwFileAttribute
			public const uint SHGFI_ADDOVERLAYS       	= 0x000000020;     // apply the appropriate overlays
			public const uint SHGFI_OVERLAYINDEX      	= 0x000000040;     // Get the index of the overlay

			public const uint FILE_ATTRIBUTE_DIRECTORY  = 0x00000010;  
			public const uint FILE_ATTRIBUTE_NORMAL     = 0x00000080;  

			[DllImport("Shell32.dll")]
			public static extern IntPtr SHGetFileInfo(
				string pszPath,
				uint dwFileAttributes,
				ref SHFILEINFO psfi,
				uint cbFileInfo,
				uint uFlags
				);
		}

		/// <summary>
		/// Wraps necessary functions imported from User32.dll. Code courtesy of MSDN Cold Rooster Consulting example.
		/// </summary>
		public class User32
		{
			/// <summary>
			/// Provides access to function required to delete handle. This method is used internally
			/// and is not required to be called separately.
			/// </summary>
			/// <param name="hIcon">Pointer to icon handle.</param>
			/// <returns>N/A</returns>
			[DllImport("User32.dll")]
			public static extern int DestroyIcon( IntPtr hIcon );
		}


		#endregion


		#region GetFolderIcon / GetFileIcon


		/// <summary>
		/// Two constants extracted from the FileInfoFlags, the only that are
		/// meaningfull for the user of this class.
		/// </summary>
		public enum SystemIconSize : int
		{
			Large = 0x000000000,
			Small = 0x000000001
		}
        
		/// <summary>
		/// Options to specify whether folders should be in the open or closed state.
		/// </summary>
		public enum FolderType
		{
			/// <summary>
			/// Specify open folder.
			/// </summary>
			Open = 0,
			/// <summary>
			/// Specify closed folder.
			/// </summary>
			Closed = 1
		}

		/// <summary>
		/// Returns an icon for a given file - indicated by the name parameter.
		/// </summary>
		/// <param name="name">Pathname for file.</param>
		/// <param name="size">Large or small</param>
		/// <param name="linkOverlay">Whether to include the link icon</param>
		/// <returns>System.Drawing.Icon</returns>
		public static System.Drawing.Icon GetFileIcon(string name, SystemIconSize size, bool linkOverlay)
		{
			Shell32.SHFILEINFO shfi = new Shell32.SHFILEINFO();
			uint flags = Shell32.SHGFI_ICON | Shell32.SHGFI_USEFILEATTRIBUTES;

			if (true == linkOverlay) flags += Shell32.SHGFI_LINKOVERLAY;

			/* Check the size specified for return. */
			if (SystemIconSize.Small == size)
			{
				flags += Shell32.SHGFI_SMALLICON ;
			} 
			else 
			{
				flags += Shell32.SHGFI_LARGEICON ;
			}

			Shell32.SHGetFileInfo(	name, 
				Shell32.FILE_ATTRIBUTE_NORMAL, 
				ref shfi, 
				(uint) System.Runtime.InteropServices.Marshal.SizeOf(shfi), 
				flags );

			if (shfi.hIcon != IntPtr.Zero)
			{
				// Copy (clone) the returned icon to a new object, thus allowing us to clean-up properly
				System.Drawing.Icon icon = (System.Drawing.Icon)System.Drawing.Icon.FromHandle(shfi.hIcon).Clone();
				User32.DestroyIcon(shfi.hIcon);		// Cleanup
				return icon;
			}
			else
				return null;
		}

		/// <summary>
		/// Used to access system folder icons.
		/// </summary>
		/// <param name="size">Specify large or small icons.</param>
		/// <param name="folderType">Specify open or closed FolderType.</param>
		/// <returns>System.Drawing.Icon</returns>
		public static System.Drawing.Icon GetFolderIcon( SystemIconSize size, FolderType folderType )
		{
			// Need to add size check, although errors generated at present!
			uint flags = Shell32.SHGFI_ICON | Shell32.SHGFI_USEFILEATTRIBUTES;

			if (FolderType.Open == folderType)
			{
				flags += Shell32.SHGFI_OPENICON;
			}
			
			if (SystemIconSize.Small == size)
			{
				flags += Shell32.SHGFI_SMALLICON;
			} 
			else 
			{
				flags += Shell32.SHGFI_LARGEICON;
			}

			// Get the folder icon
			Shell32.SHFILEINFO shfi = new Shell32.SHFILEINFO();
			Shell32.SHGetFileInfo(	"dummy",  // Windows 7 needs a string passed to this regardless. Previous versions of windows could handle null
				Shell32.FILE_ATTRIBUTE_DIRECTORY, 
				ref shfi, 
				(uint) System.Runtime.InteropServices.Marshal.SizeOf(shfi), 
				flags );

			if (shfi.hIcon != IntPtr.Zero)
			{
				System.Drawing.Icon.FromHandle(shfi.hIcon);	// Load the icon from an HICON handle

				// Now clone the icon, so that it can be successfully stored in an ImageList
				System.Drawing.Icon icon = (System.Drawing.Icon)System.Drawing.Icon.FromHandle(shfi.hIcon).Clone();

				User32.DestroyIcon(shfi.hIcon);		// Cleanup
				return icon;
			}
			else
				return null;
		}	

		#endregion


/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


		#region IconFromExtension methods (sourced elsewhere from above code)


		#region Custom exceptions class

		public class IconNotFoundException : Exception
		{
			public IconNotFoundException( string fileName, int index )
				: base( string.Format( "Icon with Id = {0} wasn't found in file {1}", index, fileName ) )
			{
			}
		}

		public class UnableToExtractIconsException : Exception
		{
			public UnableToExtractIconsException( string fileName, int firstIconIndex, int iconCount )
				: base( string.Format( "Tryed to extract {2} icons starting from the one with id {1} from the \"{0}\" file but failed", fileName, firstIconIndex, iconCount ) )
			{
			}
		}

		#endregion

		#region DllImports

		/// <summary>
		/// Contains information about a file object. 
		/// </summary>
		struct SHFILEINFO
		{
			/// <summary>
			/// Handle to the icon that represents the file. You are responsible for
			/// destroying this handle with DestroyIcon when you no longer need it. 
			/// </summary>
			public IntPtr hIcon;

			/// <summary>
			/// Index of the icon image within the system image list.
			/// </summary>
			public IntPtr iIcon;

			/// <summary>
			/// Array of values that indicates the attributes of the file object.
			/// For information about these values, see the IShellFolder::GetAttributesOf
			/// method.
			/// </summary>
			public uint dwAttributes;

			/// <summary>
			/// String that contains the name of the file as it appears in the Microsoft
			/// Windows Shell, or the path and file name of the file that contains the
			/// icon representing the file.
			/// </summary>
			[MarshalAs( UnmanagedType.ByValTStr, SizeConst = 260 )]
			public string szDisplayName;

			/// <summary>
			/// String that describes the type of file.
			/// </summary>
			[MarshalAs( UnmanagedType.ByValTStr, SizeConst = 80 )]
			public string szTypeName;
		};

		[Flags]
		enum FileInfoFlags : int
		{
			/// <summary>
			/// Retrieve the handle to the icon that represents the file and the index 
			/// of the icon within the system image list. The handle is copied to the 
			/// hIcon member of the structure specified by psfi, and the index is copied 
			/// to the iIcon member.
			/// </summary>
			SHGFI_ICON = 0x000000100,
			/// <summary>
			/// Indicates that the function should not attempt to access the file 
			/// specified by pszPath. Rather, it should act as if the file specified by 
			/// pszPath exists with the file attributes passed in dwFileAttributes.
			/// </summary>
			SHGFI_USEFILEATTRIBUTES = 0x000000010
		}

		/// <summary>
		///     Creates an array of handles to large or small icons extracted from
		///     the specified executable file, dynamic-link library (DLL), or icon
		///     file. 
		/// </summary>
		/// <param name="lpszFile">
		///     Name of an executable file, DLL, or icon file from which icons will
		///     be extracted.
		/// </param>
		/// <param name="nIconIndex">
		///     <para>
		///         Specifies the zero-based index of the first icon to extract. For
		///         example, if this value is zero, the function extracts the first
		///         icon in the specified file.
		///     </para>
		///     <para>
		///         If this value is ?1 and <paramref name="phiconLarge"/> and
		///         <paramref name="phiconSmall"/> are both NULL, the function returns
		///         the total number of icons in the specified file. If the file is an
		///         executable file or DLL, the return value is the number of
		///         RT_GROUP_ICON resources. If the file is an .ico file, the return
		///         value is 1. 
		///     </para>
		///     <para>
		///         Windows 95/98/Me, Windows NT 4.0 and later: If this value is a 
		///         negative number and either <paramref name="phiconLarge"/> or 
		///         <paramref name="phiconSmall"/> is not NULL, the function begins by
		///         extracting the icon whose resource identifier is equal to the
		///         absolute value of <paramref name="nIconIndex"/>. For example, use -3
		///         to extract the icon whose resource identifier is 3. 
		///     </para>
		/// </param>
		/// <param name="phIconLarge">
		///     An array of icon handles that receives handles to the large icons
		///     extracted from the file. If this parameter is NULL, no large icons
		///     are extracted from the file.
		/// </param>
		/// <param name="phIconSmall">
		///     An array of icon handles that receives handles to the small icons
		///     extracted from the file. If this parameter is NULL, no small icons
		///     are extracted from the file. 
		/// </param>
		/// <param name="nIcons">
		///     Specifies the number of icons to extract from the file. 
		/// </param>
		/// <returns>
		///     If the <paramref name="nIconIndex"/> parameter is -1, the
		///     <paramref name="phIconLarge"/> parameter is NULL, and the
		///     <paramref name="phiconSmall"/> parameter is NULL, then the return
		///     value is the number of icons contained in the specified file.
		///     Otherwise, the return value is the number of icons successfully
		///     extracted from the file. 
		/// </returns>
		[DllImport( "Shell32.dll", CharSet = CharSet.Auto )]
		extern static int ExtractIconEx(
				[MarshalAs( UnmanagedType.LPTStr )] 
            string lpszFile,
				int nIconIndex,
				IntPtr[] phIconLarge,
				IntPtr[] phIconSmall,
				int nIcons );

		[DllImport( "Shell32.dll", CharSet = CharSet.Auto )]
		extern static IntPtr SHGetFileInfo(
				string pszPath,
				int dwFileAttributes,
                ref SHFILEINFO psfi,
				int cbFileInfo,
				FileInfoFlags uFlags );

		#endregion


		/// <summary>
		/// Get the number of icons in the specified file.
		/// </summary>
		/// <param name="fileName">Full path of the file to look for.</param>
		/// <returns></returns>
		static int GetIconsCountInFile( string fileName )
		{
			return ExtractIconEx( fileName, -1, null, null, 0 );
		}

		#region ExtractIcon-like functions

		public static void ExtractEx( string fileName, List<Icon> largeIcons,
				List<Icon> smallIcons, int firstIconIndex, int iconCount )
		{
			/*
			 * Memory allocations
			 */

			IntPtr[] smallIconsPtrs = null;
			IntPtr[] largeIconsPtrs = null;

			if ( smallIcons != null )
			{
				smallIconsPtrs = new IntPtr[ iconCount ];
			}
			if ( largeIcons != null )
			{
				largeIconsPtrs = new IntPtr[ iconCount ];
			}

			/*
			 * Call to native Win32 API
			 */

			int apiResult = ExtractIconEx( fileName, firstIconIndex, largeIconsPtrs, smallIconsPtrs, iconCount );
			if ( apiResult != iconCount )
			{
				throw new UnableToExtractIconsException( fileName, firstIconIndex, iconCount );
			}

			/*
			 * Fill lists
			 */

			if ( smallIcons != null )
			{
				smallIcons.Clear();
				foreach ( IntPtr actualIconPtr in smallIconsPtrs )
				{
					smallIcons.Add( Icon.FromHandle( actualIconPtr ) );
				}
			}
			if ( largeIcons != null )
			{
				largeIcons.Clear();
				foreach ( IntPtr actualIconPtr in largeIconsPtrs )
				{
					largeIcons.Add( Icon.FromHandle( actualIconPtr ) );
				}
			}
		}

		public static List<Icon> ExtractEx( string fileName, SystemIconSize size,
				int firstIconIndex, int iconCount )
		{
			List<Icon> iconList = new List<Icon>();

			switch ( size )
			{
				case SystemIconSize.Large:
					ExtractEx( fileName, iconList, null, firstIconIndex, iconCount );
					break;

				case SystemIconSize.Small:
					ExtractEx( fileName, null, iconList, firstIconIndex, iconCount );
					break;

				default:
					throw new ArgumentOutOfRangeException( "size" );
			}

			return iconList;
		}

		public static void Extract( string fileName, List<Icon> largeIcons, List<Icon> smallIcons )
		{
			int iconCount = GetIconsCountInFile( fileName );
			ExtractEx( fileName, largeIcons, smallIcons, 0, iconCount );
		}

		public static List<Icon> Extract( string fileName, SystemIconSize size )
		{
			int iconCount = GetIconsCountInFile( fileName );
			return ExtractEx( fileName, size, 0, iconCount );
		}

		public static Icon ExtractOne( string fileName, int index, SystemIconSize size )
		{
			try
			{
				List<Icon> iconList = ExtractEx( fileName, size, index, 1 );
				return iconList[ 0 ];
			}
			catch ( UnableToExtractIconsException )
			{
				throw new IconNotFoundException( fileName, index );
			}
		}

		public static void ExtractOne( string fileName, int index,
				out Icon largeIcon, out Icon smallIcon )
		{
			List<Icon> smallIconList = new List<Icon>();
			List<Icon> largeIconList = new List<Icon>();
			try
			{
				ExtractEx( fileName, largeIconList, smallIconList, index, 1 );
				largeIcon = largeIconList[ 0 ];
				smallIcon = smallIconList[ 0 ];
			}
			catch ( UnableToExtractIconsException )
			{
				throw new IconNotFoundException( fileName, index );
			}
		}

		#endregion

		//this will look throw the registry 
		//to find if the Extension have an icon.
		public static Icon IconFromExtension( string extension,
																						SystemIconSize size )
		{
			// Add the '.' to the extension if needed
			if ( extension[ 0 ] != '.' ) extension = '.' + extension;

			//opens the registry for the wanted key.
			RegistryKey Root = Registry.ClassesRoot;
			RegistryKey ExtensionKey = Root.OpenSubKey( extension );
			ExtensionKey.GetValueNames();
			RegistryKey ApplicationKey = Root.OpenSubKey( ExtensionKey.GetValue( "" ).ToString() );

			//gets the name of the file that have the icon.
			string IconLocation = ApplicationKey.OpenSubKey( "DefaultIcon" ).GetValue( "" ).ToString();
			string[] IconPath = IconLocation.Split( ',' );

			if ( IconPath[ 1 ] == null ) IconPath[ 1 ] = "0";
			IntPtr[] Large = new IntPtr[ 1 ], Small = new IntPtr[ 1 ];

			//extracts the icon from the file.
			ExtractIconEx( IconPath[ 0 ],
					Convert.ToInt16( IconPath[ 1 ] ), Large, Small, 1 );
            if (size == SystemIconSize.Large)
            {
                return Large[0] == IntPtr.Zero ? null : Icon.FromHandle(Large[0]);
            }
            else
            {
                return Small[0] == IntPtr.Zero ? null : Icon.FromHandle(Small[0]);
            }
		}

		public static Icon IconFromExtensionShell( string extension, SystemIconSize size )
		{
			//add '.' if nessesry
			if ( extension[ 0 ] != '.' ) extension = '.' + extension;

			//temp struct for getting file shell info
			SHFILEINFO fileInfo = new SHFILEINFO();

			SHGetFileInfo(
					extension,
					0,
					ref fileInfo,
					Marshal.SizeOf( fileInfo ),
					FileInfoFlags.SHGFI_ICON | FileInfoFlags.SHGFI_USEFILEATTRIBUTES | (FileInfoFlags)size );

			return fileInfo.hIcon == IntPtr.Zero ? null : Icon.FromHandle( fileInfo.hIcon );
		}

		public static Icon IconFromResource( string resourceName )
		{
			Assembly assembly = Assembly.GetCallingAssembly();

			return new Icon( assembly.GetManifestResourceStream( resourceName ) );
		}

		/// <summary>
		/// Parse strings in registry who contains the name of the icon and
		/// the index of the icon an return both parts.
		/// </summary>
		/// <param name="regString">The full string in the form "path,index" as found in registry.</param>
		/// <param name="fileName">The "path" part of the string.</param>
		/// <param name="index">The "index" part of the string.</param>
		public static void ExtractInformationsFromRegistryString(
				string regString, out string fileName, out int index )
		{
			if ( regString == null )
			{
				throw new ArgumentNullException( "regString" );
			}
			if ( regString.Length == 0 )
			{
				throw new ArgumentException( "The string should not be empty.", "regString" );
			}

			index = 0;
			string[] strArr = regString.Replace( "\"", "" ).Split( ',' );
			fileName = strArr[ 0 ].Trim();
			if ( strArr.Length > 1 )
			{
				int.TryParse( strArr[ 1 ].Trim(), out index );
			}
		}

		public static Icon ExtractFromRegistryString( string regString, SystemIconSize size )
		{
			string fileName;
			int index;
			ExtractInformationsFromRegistryString( regString, out fileName, out index );
			return ExtractOne( fileName, index, size );
		}


		#endregion
	}
}


using System;
using System.Collections.Generic;
using System.Management;
using System.Text;
using System.Windows.Forms;
using System.Xml;

// Class to identify drives that are detected by class UsbDriveInfo

namespace PWLib.UsbDrive
{
	static class UsbPrivate
	{

		static string FormatDriveUniqueID( string volumeSerialNumber, string pnpDeviceId )
		{
			string uniqueId = volumeSerialNumber + "-" + pnpDeviceId;
			return uniqueId;
		}


		static ManagementObject GetDiskObjectFromLogical( ManagementObject logicalObj )
		{
			ManagementObject foundObj = null;

			foreach ( ManagementObject partitionObj in logicalObj.GetRelated( "Win32_DiskPartition" ) )
			{
				foreach ( ManagementObject diskObj in partitionObj.GetRelated( "Win32_DiskDrive" ) )
				{
					// There is a bug in WinVista that corrupts some of the fields
					// of the Win32_DiskDrive class if you instantiate the class via
					// its primary key and the device is a USB disk, so we must iterate through instead
					ManagementClass wmi = new ManagementClass( "Win32_DiskDrive" );
					foreach ( ManagementObject diskSearchObj in wmi.GetInstances() )
					{
						// loop thru all of the instances. This is silly, we shouldn't
						// have to loop thru them all, when we know which one we want.
						if ( diskSearchObj[ "DeviceID" ].ToString() == diskObj[ "DeviceID" ].ToString() )
						{
							foundObj = diskSearchObj;
						}
						else
							diskSearchObj.Dispose();
					}
					diskObj.Dispose();
				}
				partitionObj.Dispose();
			}

			if ( foundObj == null )
				throw new Exception( "Could not locate disk object from logical" );

			return foundObj;
		}


		static ManagementObject GetLogicalObjectFromDriveLetter( char driveLetter )
		{
			ManagementObject foundObj = null;
			ManagementObjectSearcher searcher = new ManagementObjectSearcher( "root\\CIMV2", "SELECT * FROM Win32_DiskDrive" );
			foreach ( ManagementObject diskObj in searcher.Get() )
			{
				foreach ( ManagementObject partitionObj in diskObj.GetRelated( "Win32_DiskPartition" ) )
				{
					foreach ( ManagementObject logicalObj in partitionObj.GetRelated( "Win32_LogicalDisk" ) )
					{
						if ( char.ToUpper( logicalObj[ "Name" ].ToString()[ 0 ] ) == char.ToUpper( driveLetter ) )
							foundObj = logicalObj;
						else
							logicalObj.Dispose();
					}
					partitionObj.Dispose();
				}
				diskObj.Dispose();
			}

			return foundObj;
		}


		public static UsbDriveInfo LoadUsbFromDriveLetter( char driveLetter )
		{
			const int maxAttempts = 5;
			try
			{
				if ( char.IsLetter( driveLetter ) )
				{
					for ( int attempts=0; attempts<maxAttempts; ++attempts ) 
					{
						try
						{
							ManagementObject logicalObj = GetLogicalObjectFromDriveLetter( driveLetter );
							logicalObj.Get();
							ManagementObject diskObj = GetDiskObjectFromLogical( logicalObj );

							string volumeName = logicalObj[ "VolumeName" ].ToString();
							object serialNumberObject = logicalObj[ "VolumeSerialNumber" ];
							string modelName = diskObj[ "Model" ].ToString();
							object pnpDeviceId = diskObj[ "PNPDeviceID" ];
							string uniqueId = FormatDriveUniqueID( serialNumberObject != null ? serialNumberObject.ToString() : "",
								pnpDeviceId != null ? pnpDeviceId.ToString() : "" );

							UsbDriveInfo driveInfo = UsbDriveList.Instance.GetUsbInfoByUniqueId( uniqueId );
							if ( driveInfo == null )
								driveInfo = UsbDriveList.Instance.CreateEmptyUsbInfo( modelName, volumeName, uniqueId );

							string sizeString = logicalObj[ "Size" ].ToString();
							System.UInt64 sizeBytes = 0;
							System.UInt64.TryParse( sizeString, out sizeBytes );
							driveInfo.__MarkActive( driveLetter, sizeBytes );

							diskObj.Dispose();
							logicalObj.Dispose();

							return driveInfo;
						}
						catch ( System.Exception )
						{
							// failed, try again until maxAttempts
							//UsbDriveList.__LogError( null, "LoadUsbFromDriveLetter attempt failed : " + (attempts + 1) + "/" + maxAttempts, e );
							System.Threading.Thread.Sleep( 200 );
						}
					}
				}
				else
					throw new Exception( "Drive letter is not valid " + driveLetter.ToString() );
			}
			catch ( System.Exception e )
			{
				UsbDriveList.__LogError( null, "UsbPrivate.LoadUsbFromDriveLetter", e );
				return null;
			}

			return null;
		}


		public static List<UsbDriveInfo> BuildUsbDiskList()
		{
			List<UsbDriveInfo> list = new List<UsbDriveInfo>();

			try
			{
				ManagementObjectSearcher searcher = new ManagementObjectSearcher( "root\\CIMV2", "SELECT * FROM Win32_DiskDrive" );

				foreach ( ManagementObject diskObj in searcher.Get() )
				{
					if ( diskObj[ "InterfaceType" ].ToString() == "USB" )
					{
						/* Contents of diskObj
						 * 
					
						foreach ( System.Management.PropertyData name in diskObj.Properties )
						{
							System.Diagnostics.Debug.WriteLine( name.Name + " : " + ( name.Value != null ? name.Value.ToString() : "" ) );
						}

	Availability : 
	BytesPerSector : 512
	Capabilities : System.UInt16[]
	CapabilityDescriptions : System.String[]
	Caption : Samsung S2 Portable USB Device
	CompressionMethod : 
	ConfigManagerErrorCode : 0
	ConfigManagerUserConfig : False
	CreationClassName : Win32_DiskDrive
	DefaultBlockSize : 
	Description : Disk drive
	DeviceID : \\.\PHYSICALDRIVE2
	ErrorCleared : 
	ErrorDescription : 
	ErrorMethodology : 
	FirmwareRevision : 
	Index : 2
	InstallDate : 
	InterfaceType : USB
	LastErrorCode : 
	Manufacturer : (Standard disk drives)
	MaxBlockSize : 
	MaxMediaSize : 
	MediaLoaded : True
	MediaType : External hard disk media
	MinBlockSize : 
	Model : Samsung S2 Portable USB Device
	Name : \\.\PHYSICALDRIVE2
	NeedsCleaning : 
	NumberOfMediaSupported : 
	Partitions : 1
	PNPDeviceID : USBSTOR\DISK&VEN_SAMSUNG&PROD_S2_PORTABLE&REV_\00000011E09310500A1B&0
	PowerManagementCapabilities : 
	PowerManagementSupported : 
	SCSIBus : 
	SCSILogicalUnit : 
	SCSIPort : 
	SCSITargetId : 
	SectorsPerTrack : 63
	SerialNumber : 
	Signature : 3392227871
	Size : 1000202273280
	Status : OK
	StatusInfo : 
	SystemCreationClassName : Win32_ComputerSystem
	SystemName : SOFTWARE118
	TotalCylinders : 121601
	TotalHeads : 255
	TotalSectors : 1953520065
	TotalTracks : 31008255
	TracksPerCylinder : 255
					 
						 */

						foreach ( ManagementObject partitionObj in diskObj.GetRelated( "Win32_DiskPartition" ) )
						{
							/* contents of partitionObj
							 * 
							 Access : 
							Availability : 
							BlockSize : 512
							Bootable : True
							BootPartition : True
							Caption : Disk #2, Partition #0
							ConfigManagerErrorCode : 
							ConfigManagerUserConfig : 
							CreationClassName : Win32_DiskPartition
							Description : Installable File System
							DeviceID : Disk #2, Partition #0
							DiskIndex : 2
							ErrorCleared : 
							ErrorDescription : 
							ErrorMethodology : 
							HiddenSectors : 
							Index : 0
							InstallDate : 
							LastErrorCode : 
							Name : Disk #2, Partition #0
							NumberOfBlocks : 1953520001
							PNPDeviceID : 
							PowerManagementCapabilities : 
							PowerManagementSupported : 
							PrimaryPartition : True
							Purpose : 
							RewritePartition : 
							Size : 1000202240512
							StartingOffset : 32768
							Status : 
							StatusInfo : 
							SystemCreationClassName : Win32_ComputerSystem
							SystemName : SOFTWARE118
							Type : Installable File System
						
							 */

							foreach ( ManagementBaseObject logicalObj in partitionObj.GetRelated( "Win32_LogicalDisk" ) )
							{
								/* Contents of logicalObj
	Access : 0
	Availability : 
	BlockSize : 
	Caption : G:
	Compressed : False
	ConfigManagerErrorCode : 
	ConfigManagerUserConfig : 
	CreationClassName : Win32_LogicalDisk
	Description : Local Fixed Disk
	DeviceID : G:
	DriveType : 3
	ErrorCleared : 
	ErrorDescription : 
	ErrorMethodology : 
	FileSystem : NTFS
	FreeSpace : 424120320
	InstallDate : 
	LastErrorCode : 
	MaximumComponentLength : 255
	MediaType : 12
	Name : G:
	NumberOfBlocks : 
	PNPDeviceID : 
	PowerManagementCapabilities : 
	PowerManagementSupported : 
	ProviderName : 
	Purpose : 
	QuotasDisabled : True
	QuotasIncomplete : False
	QuotasRebuilding : False
	Size : 1000202240000
	Status : 
	StatusInfo : 
	SupportsDiskQuotas : True
	SupportsFileBasedCompression : True
	SystemCreationClassName : Win32_ComputerSystem
	SystemName : SOFTWARE118
	VolumeDirty : False
	VolumeName : PW
	VolumeSerialNumber : 34FA652F
								 */

								string modelName = diskObj[ "Model" ].ToString();
								string volumeName = logicalObj[ "VolumeName" ].ToString();
								object serialNumberObject = logicalObj[ "VolumeSerialNumber" ];
								object pnpDeviceId = diskObj[ "PNPDeviceID" ];
								string uniqueId = FormatDriveUniqueID( serialNumberObject != null ? serialNumberObject.ToString() : "",
									pnpDeviceId != null ? pnpDeviceId.ToString() : "" );

								UsbDriveInfo driveInfo = UsbDriveList.Instance.GetUsbInfoByUniqueId( uniqueId );
								if ( driveInfo == null )
									driveInfo = UsbDriveList.Instance.CreateEmptyUsbInfo( modelName, volumeName, uniqueId );

								string sizeString = logicalObj[ "Size" ].ToString();
								System.UInt64 sizeBytes = 0;
								System.UInt64.TryParse( sizeString, out sizeBytes );

								driveInfo.__MarkActive( logicalObj[ "Name" ].ToString()[ 0 ], sizeBytes );

								list.Add( driveInfo );
							}
						}
					}
				}
			}
			catch ( ManagementException )
			{
			}

			return list;
		}
	}


	public class UsbDriveInfo
	{
		volatile bool mIsLocked = false;
		public bool MediaLocked { get { return mIsLocked; } set { mIsLocked = value; } }

		string mModelName;
		public string ModelName { get { return mModelName; } }

		string mVolumeName;
		public string VolumeName { get { return mVolumeName; } }

		string mUniqueId;
		public string UniqueID { get { return mUniqueId; } }

		UsbDriveActive mDriveId = null;
		public UsbDriveActive DriveId { get { return mDriveId; } }

		public bool MediaAvailable { get { return mDriveId != null; } }

		public event EventHandler MediaAvailableChanged;


		public UsbDriveInfo( string modelName, string volumeName, string uniqueId )
		{
			mModelName = modelName;
			mVolumeName = volumeName;
			mUniqueId = uniqueId;
		}


		internal void __MarkActive( char driveLetter, System.UInt64 sizeBytes )
		{
			mDriveId = new UsbDriveActive( driveLetter, sizeBytes );
			if ( MediaAvailableChanged != null )
				MediaAvailableChanged( this, null );
		}


		internal void __MarkInactive()
		{
			mDriveId = null;
			if ( MediaAvailableChanged != null )
				MediaAvailableChanged( this, null );
		}


		public override bool Equals( object obj )
		{
			if ( obj != null && obj is UsbDriveInfo )
			{
				UsbDriveInfo lhs = (UsbDriveInfo)obj;
				return string.Compare( lhs.UniqueID, UniqueID, false ) == 0;
			}
			else
				return base.Equals( obj );
		}


		public override int GetHashCode()
		{
			return UniqueID.GetHashCode();
		}



		public static UsbDriveInfo BuildFromXml( XmlNode parentNode )
		{
			string uniqueId = PWLib.XmlHelp.DirtyString( PWLib.XmlHelp.GetAttribute( parentNode, "uniqueid", "" ) );
			string modelName = PWLib.XmlHelp.DirtyString( PWLib.XmlHelp.GetAttribute( parentNode, "modelname", "" ) );
			string volumeName = PWLib.XmlHelp.DirtyString( PWLib.XmlHelp.GetAttribute( parentNode, "volumename", "" ) );

			// see if usb device is currently plugged in, otherwise return new usbdriveinfo object which is marked inactive
			UsbDriveInfo foundDriveInfo = UsbDriveList.Instance.GetUsbInfoByUniqueId( uniqueId );
			if ( foundDriveInfo != null )
				return foundDriveInfo;
			else
				return UsbDriveList.Instance.CreateEmptyUsbInfo( modelName, volumeName, uniqueId );
		}


		public void OutputToXml( XmlTextWriter xmlWriter )
		{
			xmlWriter.WriteAttributeString( "uniqueid", PWLib.XmlHelp.CleanString( mUniqueId ) );
			xmlWriter.WriteAttributeString( "modelname", PWLib.XmlHelp.CleanString( mModelName ) );
			xmlWriter.WriteAttributeString( "volumename", PWLib.XmlHelp.CleanString( mVolumeName ) );
		}
	}


	public class UsbDriveActive
	{
		char mDriveLetter;
		System.UInt64 mSizeBytes = 0;

		public char DriveLetter
		{ get { return mDriveLetter; } }

		public string DriveRootDirectory
		{ get { return mDriveLetter + @":\"; } }

		public System.UInt64 SizeBytes { get { return mSizeBytes; } }

		public string SizeString
		{ get { return PWLib.Platform.Windows.Misc.FormatSizeString( SizeBytes ); } }


		public UsbDriveActive( char driveLetter, System.UInt64 sizeBytes )
		{
			mDriveLetter = char.ToUpper( driveLetter );
			mSizeBytes = sizeBytes;
		}


		public override string ToString()
		{ return DriveRootDirectory; }

		public override bool Equals( object obj )
		{
			if ( obj is UsbDriveActive )
			{
				UsbDriveActive rhs = (UsbDriveActive)obj;
				return rhs.mDriveLetter == this.mDriveLetter;
			}
			else
				return false;
		}

		public override int GetHashCode()
		{
			return (int)mDriveLetter * base.GetHashCode();
		}


	}


	public class UsbDriveList : IDisposable
	{
		Control mControl;
		DriveDetector mDriveDetector;

		List<UsbDriveInfo> mDriveInfoList = new List<UsbDriveInfo>();
		public List<UsbDriveInfo> DriveList { get { return mDriveInfoList; } }

		static UsbDriveList mInstance = null;
		public static UsbDriveList Instance { get { return mInstance; } }

		public delegate void DeviceEventHandler( Object sender, UsbDriveInfo drive );
		public delegate void DeviceQueryEventHandler(Object sender, UsbDriveInfo drive, ref bool allowRemoval);
		public delegate void ErrorHandler( Object sender, string source, Exception e );

		public static event EventHandler OnCreate;
		public static event ErrorHandler OnError;

		public event DeviceEventHandler DeviceInserted;
		public event DeviceEventHandler DeviceRemoved;
		public event DeviceQueryEventHandler DeviceQueryRemove;


		public List<UsbDriveInfo> BuildActiveDriveList()
		{
			List<UsbDriveInfo> list = new List<UsbDriveInfo>();
			foreach ( UsbDriveInfo driveInfo in mDriveInfoList )
			{
				if ( driveInfo.MediaAvailable )
					list.Add( driveInfo );
			}
			return list;
		}


		public static void CreateInstance( Control control )
		{
			System.Diagnostics.Debug.Assert( mInstance == null );
			mInstance = new UsbDriveList( control );
			mInstance.Init();
			if ( OnCreate != null )
				OnCreate( mInstance, null );
		}


		internal static void __LogError( object sender, string source, Exception e )
		{
			if ( OnError != null )
				OnError( sender, source, e );
		}


		UsbDriveList( Control control )
		{
			mControl = control;
			mDriveDetector = new DriveDetector( control );
			mDriveDetector.DeviceInserted += new DriveInsertedEventHandler( mDriveDetector_DeviceInserted );
			mDriveDetector.DeviceRemoved += new DriveRemovedEventHandler( mDriveDetector_DeviceRemoved );
			mDriveDetector.QueryRemove += new DriveRemoveQueryEventHandler( mDriveDetector_QueryRemove );
		}

		void Init()
		{
			mDriveInfoList = UsbPrivate.BuildUsbDiskList();
			foreach ( UsbDriveInfo driveInfo in mDriveInfoList )
			{
				mDriveDetector.HookQueryRemove( driveInfo.DriveId.DriveLetter );
			}
		}


		public void Dispose()
		{
			mDriveDetector.Dispose();
		}


		void mDriveDetector_QueryRemove( object sender, char driveLetter, out bool allowRemoval )
		{
			allowRemoval = true;
			try
			{
				UsbDriveInfo driveInfo = GetUsbInfoByDriveLetter( driveLetter );
				if ( driveInfo != null )
				{
					allowRemoval = !driveInfo.MediaLocked;
					if ( DeviceQueryRemove != null )
						DeviceQueryRemove( this, driveInfo, ref allowRemoval );
				}
			}
			catch ( System.Exception e )
			{
				__LogError( this, "QueryRemove", e );
			}
		}


		void mDriveDetector_DeviceInserted( object sender, char driveLetter )
		{
			System.Threading.ThreadPool.QueueUserWorkItem( new System.Threading.WaitCallback( WinThread_mDriveDetector_DeviceInserted ), driveLetter );
		}


		void WinThread_mDriveDetector_DeviceInserted( object driveLetterObject )
		{
			try
			{
				char driveLetter = ( char )driveLetterObject;
				UsbDriveInfo driveInfo = UsbPrivate.LoadUsbFromDriveLetter( driveLetter );
				if ( DeviceInserted != null && driveInfo != null )
					DeviceInserted( this, driveInfo );
			}
			catch ( System.Exception e )
			{
				__LogError( this, "DeviceInserted failed", e );
			}
		}


		void mDriveDetector_DeviceRemoved( object sender, char driveLetter )
		{
			System.Threading.ThreadPool.QueueUserWorkItem( new System.Threading.WaitCallback( WinThread_mDriveDetector_DeviceRemoved ), driveLetter );
		}

		void WinThread_mDriveDetector_DeviceRemoved( object driveLetterObject )
		{
			try
			{
				char driveLetter = (char)driveLetterObject;
				UsbDriveInfo driveInfo = GetUsbInfoByDriveLetter( driveLetter );
				if ( driveInfo != null )
				{
					driveInfo.__MarkInactive();
					if ( DeviceRemoved != null )
						DeviceRemoved( this, driveInfo );
				}
			}
			catch ( Exception e )
			{
				__LogError( this, "DeviceRemoved", e );
			}
		}


		public UsbDriveInfo GetUsbInfoByUniqueId( string uniqueId )
		{
			return mDriveInfoList.Find( delegate( UsbDriveInfo driveInfo ) { return string.Compare( driveInfo.UniqueID, uniqueId, false ) == 0; } );
		}


		public UsbDriveInfo GetUsbInfoByDriveLetter( char driveLetter )
		{
			return mDriveInfoList.Find( delegate( UsbDriveInfo driveInfo ) { return driveInfo.DriveId != null ? driveInfo.DriveId.DriveLetter.Equals( driveLetter ) : false; } );
		}


		public UsbDriveInfo CreateEmptyUsbInfo( string modelName, string volumeName, string uniqueId )
		{
			UsbDriveInfo driveInfo = new UsbDriveInfo( modelName, volumeName, uniqueId );
			mDriveInfoList.Add( driveInfo );
			return driveInfo;
		}


		public bool WndProc( ref Message m )
		{
			return mDriveDetector.WndProc( ref m );
		}

	}

}

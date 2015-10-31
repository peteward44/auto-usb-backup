using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32.SafeHandles;

// Drive Detector: Detects the insertion & removal of removable disk volumes (USB drives etc)


namespace PWLib.UsbDrive
{
	public delegate void DriveInsertedEventHandler( Object sender, char driveLetter );
	public delegate void DriveRemovedEventHandler( Object sender, char driveLetter );
	public delegate void DriveRemoveQueryEventHandler( Object sender, char driveLetter, out bool allowRemoval );

  
  public class DriveDetector : IDisposable 
  {
		private class RemoveQueryHook : IDisposable
		{
			char mDriveLetter;
			IntPtr mRegisterDeviceHandle = IntPtr.Zero;

			public char DriveLetter { get { return mDriveLetter; } }


			public override string ToString()
			{
				return mDriveLetter.ToString();
			}


			public override int GetHashCode()
			{
				return mRegisterDeviceHandle.GetHashCode();
			}


			public override bool Equals( object obj )
			{
				if ( obj is RemoveQueryHook )
				{
					RemoveQueryHook driveObj = ( RemoveQueryHook )obj;
					return driveObj.mRegisterDeviceHandle.Equals( this.mRegisterDeviceHandle );
				}
				else if ( obj is IntPtr )
				{
					IntPtr devHandle = (IntPtr)obj;
					return devHandle.Equals( this.mRegisterDeviceHandle );
				}
				else
					return base.Equals( obj );
			}


			public RemoveQueryHook( char driveLetter, IntPtr windowHandle )
			{
				mDriveLetter = driveLetter;

				DEV_BROADCAST_DEVICEINTERFACE data = new DEV_BROADCAST_DEVICEINTERFACE();
				data.dbcc_devicetype = DBT_DEVTYP_DEVICEINTERFACE;
				data.dbcc_name = '\0';
				// This GUID is for all USB serial host PnP drivers, but you can replace it 
				// with any valid device class guid.
				data.dbcc_classguid = new Guid( 0x25dbce51, 0x6c8f, 0x4a72, 0x8a, 0x6d, 0xb5, 0x4c, 0x2b, 0x4f, 0xc8, 0x35 );
				int size = Marshal.SizeOf( data );
				data.dbcc_size = (uint)size;

				IntPtr buffer = Marshal.AllocHGlobal( size );
				try
				{
					Marshal.StructureToPtr( data, buffer, true );
					mRegisterDeviceHandle = Native.RegisterDeviceNotification( windowHandle, buffer, 0 );
					if ( mRegisterDeviceHandle == IntPtr.Zero )
						throw new Exception( "RegisterDeviceNotification() failed on drive '" + mDriveLetter.ToString() + "'" );
				}
				finally
				{
					Marshal.FreeHGlobal( buffer );
				}
			}


			~RemoveQueryHook()
			{
				Dispose();
			}


			public void Dispose()
			{
				try
				{
					if ( mRegisterDeviceHandle != IntPtr.Zero )
						Native.UnregisterDeviceNotification( mRegisterDeviceHandle );
					mRegisterDeviceHandle = IntPtr.Zero;
				}
				catch ( System.Exception e )
				{
					UsbDriveList.__LogError( this, "HookedDrive.Dispose failed", e );
				}
			}
		}


		private RemoveQueryHook FindHookedDrive( IntPtr registerDeviceHandle )
		{
			return mHookedDrives.Find( delegate( RemoveQueryHook drive ) { return drive.Equals( registerDeviceHandle ); } );
		}


		private RemoveQueryHook FindHookedDrive( char driveLetter )
		{
			return mHookedDrives.Find( delegate( RemoveQueryHook drive ) { return drive.DriveLetter.Equals( driveLetter ); } );
		}


		private void RemoveHookedDrive( RemoveQueryHook hookedDrive )
		{
			if ( hookedDrive != null )
			{
				mHookedDrives.Remove( hookedDrive );
				hookedDrive.Dispose();
			}
		}


		class EventObject
		{
			public enum EventType { Insert, Remove };

			public EventObject( EventType type, char driveLetter )
			{
				mEventType = type;
				mDriveLetter = driveLetter;
			}

			EventType mEventType;
			char mDriveLetter;

			public EventType Event { get { return mEventType; } }
			public char DriveLetter { get { return mDriveLetter; } }
		}

		List<RemoveQueryHook> mHookedDrives = new List<RemoveQueryHook>();
		Control mControl = null;
	  IntPtr mControlHandle = IntPtr.Zero;

		System.Threading.Thread mEventHandlerThread;
		volatile bool mThreadRunning = true;
		Queue<EventObject> mEventQueue = new Queue<EventObject>();
		System.Threading.Mutex mEventHandlerMutex = new System.Threading.Mutex();


		public event DriveInsertedEventHandler DeviceInserted;
		public event DriveRemovedEventHandler DeviceRemoved;
		public event DriveRemoveQueryEventHandler QueryRemove;

		// Insert / remove threads must be executed on a different thread as they are called within WndProc().
		// System.Windows.Forms.Invoke also wont work as we are within WndProc at the time.
		// QueryRemove does not use the thread as it requires an immediate response for the application to allow removal, just
		// dont call any COM stuff when responding to a QueryRemove
		public DriveDetector( Control control )
		{
			mControl = control;
			mControlHandle = mControl.Handle;
			mEventHandlerThread = new System.Threading.Thread( new System.Threading.ThreadStart( OnEventHandlerThreadStart ) );
			mEventHandlerThread.Start();
		}


		~DriveDetector()
		{
			Dispose();
		}


		public void HookQueryRemove( char driveLetter )
	  {
			try
			{
				if ( FindHookedDrive( driveLetter ) == null )
				{
					RemoveQueryHook hookedDrive = new RemoveQueryHook( driveLetter, mControlHandle );
					mHookedDrives.Add( hookedDrive );
				}
			}
			catch ( System.Exception )
			{
			}
	  }


		void OnEventHandlerThreadStart()
		{
			while ( mThreadRunning )
			{
				try
				{
					lock ( mEventHandlerMutex )
					{
						while ( mEventQueue.Count > 0 )
						{
							EventObject eventObject = mEventQueue.Dequeue();
							switch ( eventObject.Event )
							{
								case EventObject.EventType.Insert:
									{
										HookQueryRemove( eventObject.DriveLetter );
										if ( DeviceInserted != null )
										{
											if ( DeviceInserted != null )
												DeviceInserted( this, eventObject.DriveLetter );
										}
									}
									break;
								case EventObject.EventType.Remove:
									{
										RemoveHookedDrive( FindHookedDrive( eventObject.DriveLetter ) );
										if ( DeviceRemoved != null )
											DeviceRemoved( this, eventObject.DriveLetter );
									}
									break;
							}
						}
					}
				}
				catch ( System.Exception e )
				{
					UsbDriveList.__LogError( this, "OnEventHandlerThreadStart", e );
				}

				System.Threading.Thread.Sleep( 500 );
			}
		}


		public void Dispose()
		{
			try
			{
				mThreadRunning = false;

				lock ( mEventHandlerMutex )
				{
					foreach ( RemoveQueryHook drive in mHookedDrives )
					{
						drive.Dispose();
					}
					mHookedDrives.Clear();
				}
			}
			catch ( System.Exception e )
			{
				UsbDriveList.__LogError( this, "DriveDetector.Dispose", e );
			}
		}



		private void HandleDeviceInserted( char driveLetter )
		{
			try
			{
				lock ( mEventHandlerMutex )
				{
					mEventQueue.Enqueue( new EventObject( EventObject.EventType.Insert, driveLetter ) );
				}
			}
			catch ( System.Exception e )
			{
				UsbDriveList.__LogError( this, "DriveDetector.HandleDeviceInserted", e );
			}
		}


		private void HandleDeviceRemoved( char driveLetter )
		{
			try
			{
				lock ( mEventHandlerMutex )
				{
					mEventQueue.Enqueue( new EventObject( EventObject.EventType.Remove, driveLetter ) );
				}
			}
			catch ( System.Exception e )
			{
				UsbDriveList.__LogError( this, "DriveDetector.HandleDeviceRemoved", e );
			}
		}


		private bool HandleDeviceRemoveQuery( char driveLetter, ref Message m )
		{
			bool callBaseWndProc = true;

			try
			{
				if ( QueryRemove != null )
				{
					bool allowRemoval = true;
					if ( QueryRemove != null )
						QueryRemove( this, driveLetter, out allowRemoval );

					if ( !allowRemoval )
					{
						callBaseWndProc = false;
						m.Result = (IntPtr)BROADCAST_QUERY_DENY;
					}
				}
			}
			catch ( System.Exception e )
			{
				UsbDriveList.__LogError( this, "DriveDetector.HandleDeviceRemoveQueryd", e );
			}

			return callBaseWndProc;
		}


		public static char DriveMaskToLetter(int mask)
		{
			char letter;
			string drives = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
			// 1 = A
			// 2 = B
			// 4 = C...
			int cnt = 0;
			int pom = mask / 2;     
			while (pom != 0)
			{
				// while there is any bit set in the mask
				// shift it to the right...                
				pom = pom / 2;
				cnt++;
			}

			if ( cnt < drives.Length )
				letter = drives[ cnt ];
			else
				throw new Exception( "Could not determine drive letter from mask" );

			return letter;
		}


		#region WindowProc


		public bool WndProc( ref Message m )
		{
			bool callBaseWndProc = true;
			try
			{
				if ( m.Msg == WM_DEVICECHANGE )
				{
					int wParam = m.WParam.ToInt32();

					if ( wParam == DBT_DEVICEARRIVAL || wParam == DBT_DEVICEREMOVECOMPLETE || wParam == DBT_DEVICEQUERYREMOVE )
					{
						DEV_BROADCAST_HDR baseStructure = (DEV_BROADCAST_HDR)Marshal.PtrToStructure( m.LParam, typeof( DEV_BROADCAST_HDR ) );

						switch ( baseStructure.dbch_devicetype )
						{
							case DBT_DEVTYP_VOLUME:
								{
									DEV_BROADCAST_VOLUME vol = (DEV_BROADCAST_VOLUME)Marshal.PtrToStructure( m.LParam, typeof( DEV_BROADCAST_VOLUME ) );
									char driveLetter = DriveMaskToLetter( vol.dbcv_unitmask );

									switch ( wParam )
									{
										case DBT_DEVICEARRIVAL:
											HandleDeviceInserted( driveLetter );
											break;
										case DBT_DEVICEREMOVECOMPLETE:
											HandleDeviceRemoved( driveLetter );
											break;
									}
								}
								break;

							case DBT_DEVTYP_HANDLE:
								{
									DEV_BROADCAST_HANDLE handle = (DEV_BROADCAST_HANDLE)Marshal.PtrToStructure( m.LParam, typeof( DEV_BROADCAST_HANDLE ) );
									RemoveQueryHook hookedDrive = null;
									lock ( mEventHandlerMutex )
									{
										hookedDrive = FindHookedDrive( handle.dbch_hdevnotify );
									}

									if ( hookedDrive != null )
									{
										switch ( wParam )
										{
											case DBT_DEVICEQUERYREMOVE:
												callBaseWndProc = HandleDeviceRemoveQuery( hookedDrive.DriveLetter, ref m );
												break;
										}
									}
								}
								break;
						}
					}
				}
			}
			catch ( System.Exception e )
			{
				UsbDriveList.__LogError( this, "DriveDetector.WndProc", e );
			}
			return callBaseWndProc;
		}


		#endregion


		#region Native Win32 API


		// Win32 constants
		private const int BROADCAST_QUERY_DENY = 0x424D5144;

		// Windows message
		private const int WM_DEVICECHANGE = 0x0219;

		// Windows message WPARAM member values on WM_DEVICECHANGE
		private const int DBT_DEVICEARRIVAL = 0x8000; // system detected a new device
		private const int DBT_DEVICEQUERYREMOVE = 0x8001;   // Preparing to remove (any program can disable the removal)
		private const int DBT_DEVICEREMOVECOMPLETE = 0x8004; // removed 

		private const int DBT_DEVTYP_VOLUME = 0x00000002; // drive type is logical volume
		private const int DBT_DEVTYP_HANDLE = 0x00000006; // drive type is handle
		private const int DBT_DEVTYP_DEVICEINTERFACE = 0x00000005; // drive type is 'class of devices'


		private class Native
		{
			//   HDEVNOTIFY RegisterDeviceNotification(HANDLE hRecipient,LPVOID NotificationFilter,DWORD Flags);
			[DllImport("user32.dll", CharSet = CharSet.Auto)]
			public static extern IntPtr RegisterDeviceNotification(IntPtr hRecipient, IntPtr NotificationFilter, uint Flags);

			[DllImport("user32.dll", CharSet = CharSet.Auto)]
			public static extern uint UnregisterDeviceNotification(IntPtr hHandle);

			// CreateFile  - MSDN
			const uint GENERIC_READ = 0x80000000;
			const uint OPEN_EXISTING = 3;
			const uint FILE_SHARE_READ = 0x00000001;
			const uint FILE_SHARE_WRITE = 0x00000002;
			const uint FILE_ATTRIBUTE_NORMAL = 128;
			const uint FILE_FLAG_BACKUP_SEMANTICS = 0x02000000;
			static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

			// should be "static extern unsafe"
			[DllImport("kernel32", SetLastError = true)]
			static extern IntPtr CreateFile(
						string FileName,                    // file name
						uint DesiredAccess,                 // access mode
						uint ShareMode,                     // share mode
						uint SecurityAttributes,            // Security Attributes
						uint CreationDisposition,           // how to create
						uint FlagsAndAttributes,            // file attributes
						int hTemplateFile                   // handle to template file
						);

			[DllImport("kernel32", SetLastError = true)]
			static extern bool CloseHandle( IntPtr hObject );

			/// <summary>
			/// Opens a directory, returns it's handle or zero.
			/// </summary>
			/// <param name="dirPath">path to the directory, e.g. "C:\\dir"</param>
			/// <returns>handle to the directory. Close it with CloseHandle().</returns>
			static public IntPtr OpenDirectory(string dirPath)
			{
				// open the existing file for reading          
				IntPtr handle = CreateFile(
					dirPath,
					GENERIC_READ,
					FILE_SHARE_READ | FILE_SHARE_WRITE,
					0,
					OPEN_EXISTING,
					FILE_FLAG_BACKUP_SEMANTICS | FILE_ATTRIBUTE_NORMAL,
					0);

				return handle == INVALID_HANDLE_VALUE ? IntPtr.Zero : handle;
			}

			public static bool CloseDirectoryHandle(IntPtr handle)
			{
				return CloseHandle(handle);
			}
		}

		// Base structure, all below structures start with this one and it's type is defined by
		// the dbch_devicetype member
		[StructLayout( LayoutKind.Sequential )]
		public struct DEV_BROADCAST_HDR
		{
			public int dbch_size;
			public int dbch_devicetype;
			public int dbch_reserved;
		}

		// Structure with information for RegisterDeviceNotification.
		[StructLayout(LayoutKind.Sequential)]
		public struct DEV_BROADCAST_HANDLE
		{
			public int dbch_size;
			public int dbch_devicetype;
			public int dbch_reserved;
			public IntPtr dbch_handle;
			public IntPtr dbch_hdevnotify;
			public Guid dbch_eventguid;
			public long dbch_nameoffset;
			public byte dbch_data1; 
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct DEV_BROADCAST_DEVICEINTERFACE
		{
			public UInt32 dbcc_size;
			public UInt32 dbcc_devicetype;
			public UInt32 dbcc_reserved;
			public Guid dbcc_classguid;
			public char dbcc_name;
		}

		// Struct for parameters of the WM_DEVICECHANGE message
		[StructLayout(LayoutKind.Sequential)]
		public struct DEV_BROADCAST_VOLUME
		{
			public int dbcv_size;
			public int dbcv_devicetype;
			public int dbcv_reserved;
			public int dbcv_unitmask;
			public short dbcv_flags;
		}

#endregion

	}
}

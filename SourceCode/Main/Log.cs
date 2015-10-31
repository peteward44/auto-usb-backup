using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceProcess;

namespace AutoUSBBackup
{
	[System.Flags]
	public enum LogType
	{
		AppLog = 1,
		TextLog = 2,
		DebugLog = 4,
		Error = 8,
		TextLogVerbose = 16,
		All = AppLog | TextLog | DebugLog,
	}

	public delegate void LogLineDelegate( string line );

	public class Log
	{
		static System.IO.StreamWriter mFileLog = null;
		static string mCurrentFilename = "";
		static bool sVerboseEnabled = false;

		public static event LogLineDelegate LogLineEvent;


		public static void Init( bool enableVerboseLogging )
		{
			sVerboseEnabled = enableVerboseLogging;
			ReloadLogFile();
			PWLib.FileSyncLib.FileSync.OnError += new PWLib.FileSyncLib.FileSync.ErrorHandler( FileSync_OnError );
			PWLib.FileSyncLib.FileSync.OnLog += new PWLib.FileSyncLib.FileSync.LogHandler( FileSync_OnLog );
		}


		static void FileSync_OnLog( object sender, string source )
		{
			WriteLine( LogType.TextLogVerbose, source );
		}

		static void FileSync_OnError( object sender, string source, Exception e )
		{
			WriteException( source, e );
		}


		static void DeleteOldLogFiles( TimeSpan deleteOlderThanThis )
		{
			DateTime threshold = DateTime.Now - deleteOlderThanThis;

			foreach ( string fullpath in PWLib.Platform.Windows.Directory.GetFiles( Config.Active.LogFileDirectory ) )
			{
				try
				{
					string file = PWLib.Platform.Windows.Path.GetFileNameWithoutExtension( fullpath );
					if ( file.Length == 8 )
					{
						int year = int.Parse( file.Substring( 0, 4 ) );
						int month = int.Parse( file.Substring( 4, 2 ) );
						int day = int.Parse( file.Substring( 6, 2 ) );
						DateTime dt = new DateTime( year, month, day );
						if ( dt < threshold )
						{
							PWLib.Platform.Windows.File.Delete( fullpath );
						}
					}
				}
				catch ( System.Exception )
				{
				}
			}
		}


		static void ReloadLogFile()
		{
			DateTime dt = DateTime.Now;
			string filename = string.Format( "{0:d4}{1:d2}{2:d2}.log", dt.Year, dt.Month, dt.Day );
			if ( filename != mCurrentFilename )
			{
				mCurrentFilename = filename;
				if ( mFileLog != null )
					mFileLog.Close();
				mFileLog = new System.IO.StreamWriter( Config.Active.LogFileDirectory + PWLib.Platform.Windows.Path.DirectorySeparatorChar + filename, true );

				DeleteOldLogFiles( new TimeSpan( 30, 0, 0, 0, 0 ) );
			}
		}


		public static void WriteLine( LogType logType, string line )
		{
			try
			{
				LogType textLogVerboseFlag = sVerboseEnabled ? LogType.TextLogVerbose : (LogType)0;
				DateTime now = DateTime.Now;
				string dateLine = string.Format( "[{0} {1:d2}{2:d2}{3:d2}] {4}", now.ToShortDateString(), now.Hour, now.Minute, now.Second, line );
				
				if ( ( logType & ( LogType.AppLog | LogType.Error ) ) > 0 )
					GUI.MainForm.Instance.LogBox.WriteLine( now, line, ( ( logType & LogType.Error ) > 0 ) ? System.Drawing.Color.Red : System.Drawing.Color.Black );

				if ( (logType & ( LogType.DebugLog | LogType.Error | textLogVerboseFlag ) ) > 0 )
					System.Diagnostics.Debug.WriteLine( dateLine );

				if ( (logType & ( LogType.TextLog | LogType.Error | textLogVerboseFlag ) ) > 0 )
				{
					ReloadLogFile();

					if ( mFileLog != null )
					{
						mFileLog.WriteLine( dateLine );
						mFileLog.Flush();
					}
				}

				if ( LogLineEvent != null )
					LogLineEvent( line );
			}
			catch ( System.Exception )
			{
			}
		}

		public static void WriteException( string line, Exception e )
		{
			WriteException( line, e, true );
		}

		public static void WriteException( string line, Exception e, bool outputToGUILog )
		{
			WriteLine( outputToGUILog ? LogType.Error : ( LogType.TextLog | LogType.DebugLog ), e.Message + ": " + line );
			WriteLine( LogType.TextLog | LogType.DebugLog, e.StackTrace );
		}


		public static void Close()
		{
			mFileLog.Close();
			mFileLog = null;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Text;


namespace AutoUSBBackup
{
	public class VolumeMonitor
	{
		System.IO.FileSystemWatcher mWatcher;


		public VolumeMonitor( string path )
		{
			mWatcher = new System.IO.FileSystemWatcher( path );

			mWatcher.NotifyFilter = System.IO.NotifyFilters.FileName | System.IO.NotifyFilters.LastWrite | System.IO.NotifyFilters.DirectoryName;

			mWatcher.Changed += new System.IO.FileSystemEventHandler( mWatcher_Changed );
			mWatcher.Created += new System.IO.FileSystemEventHandler( mWatcher_Created );
			mWatcher.Deleted += new System.IO.FileSystemEventHandler( mWatcher_Deleted );
			mWatcher.Renamed += new System.IO.RenamedEventHandler( mWatcher_Renamed );
			mWatcher.Error += new System.IO.ErrorEventHandler( mWatcher_Error );

			mWatcher.IncludeSubdirectories = true;
			mWatcher.EnableRaisingEvents = true;
		}


		void mWatcher_Error( object sender, System.IO.ErrorEventArgs e )
		{
			System.Diagnostics.Debug.WriteLine( "Watcher error: " + e.GetException().Message );
		}


		void mWatcher_Renamed( object sender, System.IO.RenamedEventArgs e )
		{
			System.Diagnostics.Debug.WriteLine( "File renamed: '" + e.OldName + "' to '" + e.Name + "'" );
		}


		void mWatcher_Deleted( object sender, System.IO.FileSystemEventArgs e )
		{
			System.Diagnostics.Debug.WriteLine( "File deleted: '" + e.Name + "'" );
		}


		void mWatcher_Created( object sender, System.IO.FileSystemEventArgs e )
		{
			System.Diagnostics.Debug.WriteLine( "File created: '" + e.Name + "'" );
		}


		void mWatcher_Changed( object sender, System.IO.FileSystemEventArgs e )
		{
			System.Diagnostics.Debug.WriteLine( "File changed: '" + e.Name + "'" );
		}
	}
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using Microsoft.Win32;

namespace AutoUSBBackup
{
	[RunInstaller( true )]
	public partial class ProjectInstaller : Installer
	{
		public ProjectInstaller()
		{
			InitializeComponent();
		}

		protected override void OnAfterInstall( System.Collections.IDictionary savedState )
		{
			base.OnAfterInstall( savedState );
			try
			{
#if INSTALL_AS_SERVICE
				SetInteractWithDesktopFlag( ServiceObject.ObjectRef.ServiceName );		
#else
				//System.IO.StreamWriter sw = new System.IO.StreamWriter( @"C:\log.txt" );
				//foreach ( string kvp in base.Context.Parameters.Keys )
				//{
				//  sw.WriteLine( kvp.ToString() + " : " + base.Context.Parameters[kvp].ToString() );
				//}
				//sw.Close();

				//object assemblyPath = base.Context.Parameters[ "assemblypath" ];
				//if ( assemblyPath != null )
				//  SetRunOnStartup( "AutoUSBBackup", ( string )assemblyPath, !IsAllUsersInstall() );
#endif
			}
			catch ( System.Exception )
			{
			}
		}


		bool IsAllUsersInstall()
		{
			// An ALLUSERS property value of 1 specifies the per-machine installation context.
			// An ALLUSERS property value of an empty string ("") specifies the per-user installation context.

			// In the custom action data, we have mapped the parameter 'AllUsers' to ALLUSERS.
			// (By setting CustomActionData in the custom action viewer to '/AllUsers=[ALLUSERS]' )
			string s = base.Context.Parameters[ "AllUsers" ];

			if ( s == null )
				return true;
			else if ( s == string.Empty )
				return false;
			else
				return true;
		}


		void SetInteractWithDesktopFlag( string serviceName )
		{
			RegistryKey ckey = Registry.LocalMachine.OpenSubKey( @"SYSTEM\CurrentControlSet\Services\" + serviceName, true );

			if ( ckey != null )
			{
				int oldValue = ckey.GetValue( "Type" ) != null ? ( ( int )ckey.GetValue( "Type" ) ) : 0;
				ckey.SetValue( "Type", ( oldValue | 256 ) );
			}
		}


		void SetRunOnStartup( string appName, string appPath, bool currentUserOnly )
		{
			RegistryKey parent = currentUserOnly ? Registry.CurrentUser : Registry.LocalMachine;
			RegistryKey rkApp = parent.OpenSubKey( "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", Microsoft.Win32.RegistryKeyPermissionCheck.ReadWriteSubTree );

			if ( rkApp != null )
				rkApp.SetValue( appName, appPath );
		}

	}
}
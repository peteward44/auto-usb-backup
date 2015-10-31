using System;
using System.Collections.Generic;
using System.Text;

namespace PWLib.FileSyncLib
{
	public class FileSync
	{
		public delegate void LogHandler( object sender, string source );

		public static event LogHandler OnLog;

		public static void __Log( object parent, string message )
		{
			if ( OnLog != null )
				OnLog( parent, message );
		}


		public delegate void ErrorHandler( object sender, string source, Exception e );

		public static event ErrorHandler OnError;

		public static void __LogError( object parent, string message, Exception e )
		{
			if ( OnError != null )
				OnError( parent, message, e );
		}
	}
}

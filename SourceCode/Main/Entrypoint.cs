using System; 
using System.Collections.Generic;
using System.ServiceProcess;

namespace AutoUSBBackup
{
	static class Entrypoint
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main( string[] args )
		{
			try
			{
				GUI.MainForm.StartApp( true, args );
			}
			catch ( System.Exception )
			{
			}
			finally
			{
				GUI.MainForm.StopApp();
			}
		}
	}
}
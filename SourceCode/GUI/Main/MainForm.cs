using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using PWLib.FileSyncLib;


namespace AutoUSBBackup.GUI
{

	public partial class MainForm : Form
	{
		static MainForm sInstance = null;
		public static MainForm Instance { get { return sInstance; } }

		SpineThread mSpineThread = null;
		Thread mShutdownThread;
		MainFormEventController mEventController;

		bool mClosing = false;
		bool mSystemShutdown = false;
		bool mHideOnLoad = true;


		public Spine Spine { get { return mSpineThread != null ? mSpineThread.Spine : null; } }
		public MainFormEventController EventController { get { return mEventController; } }
		public UserControlSwitcher ControlSwitcher { get { return mUserControlSwitcher; } }

		public Shared.LogTextBox LogBox { get { return ( ( WelcomeControl )mUserControlSwitcher.GetUserControl( FormControlType.Welcome ) ).LogBox; } }


		public static void StartApp( bool block, string[] args )
		{
			try
			{
				bool hideOnLoad = false;
				bool verboseLogging = false;

				foreach ( string arg in args )
				{
					string lowerArg = arg.ToLower();
					if ( lowerArg == "/hide" )
						hideOnLoad = true;
					else if ( lowerArg == "/verbose" )
						verboseLogging = true;
				}

				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault( false );

				GUI.MainForm m = new GUI.MainForm( hideOnLoad );
				Config.CreateInstance();
				PWLib.UsbDrive.UsbDriveList.CreateInstance( m );

				m.ControlSwitcher.SwitchUserControl( FormControlType.Welcome, FormControlSwitchType.Start );

				Log.Init( verboseLogging );
				Log.WriteLine( LogType.TextLog, "******************* Starting application *******************" );

				m.StartSpineThread();

				if ( block )
					Application.Run( m );
				else
					m.Show();
			}
			catch ( System.Exception ex )
			{
				Log.WriteException( "Fatal exception caught", ex );
			}
		}


		public static void StopApp()
		{
			try
			{
				PWLib.UsbDrive.UsbDriveList.Instance.Dispose();

				Log.WriteLine( LogType.TextLog, "******************* Exiting application *******************" );
				Log.Close();
			}
			catch ( System.Exception )
			{
			}
		}


		public new void Dispose()
		{
			CloseApplication();
			base.Dispose();
		}


		public MainForm( bool hideOnLoad )
		{
			System.Diagnostics.Debug.Assert( sInstance == null );
			sInstance = this;
			mShutdownThread = new Thread( new ThreadStart( ShutdownThread ) );

			mHideOnLoad = hideOnLoad;

			InitializeComponent();

			CreateControl();

			Shown += new EventHandler( MainForm_Shown );

			mSpineThread = new SpineThread();
			VolumeDescriptorList.CreateInstance( mSpineThread.EventController );

			mEventController = new MainFormEventController( this, mProgressBar, mTrayIcon );
			mUserControlSwitcher.BuildUserControlList();
		}


		public void StartSpineThread()
		{
			mEventController.HookSpineEvents( mSpineThread.EventController );

			mSpineThread.Start();
		}


		protected override void WndProc( ref Message m )
		{
			bool callBaseWndProc = true;

			//		const int WM_ENDSESSION = 0x16;
			const int WM_QUERYENDSESSION = 0x11;

			if ( m.Msg.Equals( WM_QUERYENDSESSION ) )
				mSystemShutdown = true;

			if ( mSpineThread != null )
				callBaseWndProc = mSpineThread.WndProc( ref m );

			if ( callBaseWndProc )
				base.WndProc( ref m );
		}


		#region Closing / showing / hiding form methods


		void ShutdownThread()
		{
			while ( mSpineThread != null && !mSpineThread.Disposed )
			{
				Thread.Sleep( 100 );
			}
			if ( this.Created )
				this.Invoke( new ThreadStart( delegate() { Close(); } ) );
		}


		public void CloseApplication()
		{
			if ( !mClosing )
			{
				Config.Active.Save();

				if ( mSpineThread != null )
					mSpineThread.CancelOperation();

				ShowForm( false );
				mClosing = true;
				if ( mSpineThread != null )
					mSpineThread.Dispose();

				mTrayIcon.Dispose();

				mShutdownThread.Start();
			}
		}


		public void ShowForm( bool show )
		{
			if ( show )
			{
				Show();
				Activate();
				BringToFront();
				WindowState = FormWindowState.Normal;
			}
			else
			{
				Hide();
				WindowState = FormWindowState.Minimized;
			}
		}


		private void MainForm_FormClosing( object sender, FormClosingEventArgs e )
		{
			if ( mSystemShutdown )
			{
				CloseApplication();
				e.Cancel = false;
				ShowForm( false );
			}
			else if ( !mClosing )
			{
				e.Cancel = true;
				ShowForm( false );
			}
		}


		#endregion


		#region Methods called by inside the GUI (mainly from the UserControls describing the GUI)


		public void RemoveVolume( VolumeDescriptor desc, bool removeAllData )
		{
			mSpineThread.RemoveVolume( desc, removeAllData );
		}


		public bool VolumeNameExists( string name )
		{
			return VolumeDescriptorList.Instance.VolumeNameExists( name );
		}


		public string TakeNameAndMakeItUnique( string defaultName )
		{
			string name = defaultName;

			for ( int attempts = 1; true; ++attempts )
			{
				if ( VolumeNameExists( name ) )
				{
					name = defaultName + " (" + attempts + ")";
				}
				else if ( attempts >= 1000 )
				{
					name = "";
				}
				else
					break;
			}

			return name;
		}


		public void RestoreVolume( VolumeDescriptor vid, VolumeSnapshotRevision revision, string onDiskPath )
		{
			mSpineThread.RestoreVolume( vid, revision, onDiskPath );
		}


		public void TransferVolume( VolumeDescriptor vid, PWLib.UsbDrive.UsbDriveInfo newDrive )
		{
			mSpineThread.TransferVolume( vid, newDrive );
		}


		public void SetDefaultStatusText()
		{
			SetStatusText("Ready");
		}


		public void SetTitleText( string text, int percentComplete )
		{
			if ( text.Length > 0 && percentComplete >= 0 )
				this.Text = text + "... [" + percentComplete.ToString() + "%]";
			else
				this.Text = "AutoUSBBackup";
		}


		public void SetStatusText(string text)
		{
			mStatusLabel.Text = text;
		}


		public void CancelOperation()
		{
			mSpineThread.CancelOperation();
		}


		public void FormatUsbDrive( PWLib.UsbDrive.UsbDriveInfo drive )
		{
			mSpineThread.FormatUsbDrive( drive );
		}


		public void ForceFullBackup()
		{
			mSpineThread.ForceFullBackup();
		}


		public void BackupVolume( VolumeDescriptor vdesc )
		{
			if ( vdesc.Volume != null )
				mSpineThread.ForceBackup( vdesc.Volume );
		}

		#endregion


		#region Tray icon events

		private void mTrayIcon_DoubleClick( object sender, EventArgs e )
		{
			ShowForm( true );
		}

		private void showToolStripMenuItem_Click( object sender, EventArgs e )
		{
			mTrayIcon_DoubleClick( sender, e );
		}

		private void exitToolStripMenuItem_Click( object sender, EventArgs e )
		{
			CloseApplication();
		}

		#endregion


		void MainForm_Shown( object sender, EventArgs e )
		{
			if ( mHideOnLoad )
			{
				ShowForm( false );
			}
			RecalculateProgressBarSize();
		}


		private void MainForm_Resize(object sender, EventArgs e)
		{
			if ( FormWindowState.Minimized == WindowState )
			{
				Hide(); // leave only the sys tray icon
			}
			RecalculateProgressBarSize();
		}

		private void mStatusLabel_TextChanged(object sender, EventArgs e)
		{
			RecalculateProgressBarSize();
		}


		void RecalculateProgressBarSize()
		{
			mProgressBar.Width = this.Size.Width - this.mStatusLabel.Width - 50;
		}


	}
}
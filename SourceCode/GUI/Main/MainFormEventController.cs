using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using PWLib.FileSyncLib;



namespace AutoUSBBackup.GUI
{
	public class MainFormEventController
	{
		Control mInvokeControl;
		ToolStripProgressBar mProgressBar;
		NotifyIcon mNotifyIcon;

		public event EventHandler SpineInitialised;

		public event VolumeBackupHandler BackupStarted;
		public event VolumeBackupHandler BackupFinished;
		public event VolumeRestoreHandler RestoreStarted;
		public event VolumeRestoreHandler RestoreFinished;

		public event EventHandler FormatUsbFinished;

		public event VolumeDescriptorChangedHandler VolumeNeedsRefreshing;


		public MainFormEventController( Control invokeControl, ToolStripProgressBar progressBar, NotifyIcon notifyIcon )
		{
			mInvokeControl = invokeControl;
			mProgressBar = progressBar;
			mNotifyIcon = notifyIcon;
		}


		public void HookSpineEvents( VolumeEventController eventController )
		{
			eventController.VolumeDescriptorActiveStateChanged += new VolumeDescriptorChangedHandler( eventController_VolumeDescriptorActiveStateChanged );
			eventController.VolumeSourceActiveStateChanged += new VolumeSourceActiveChangedHandler( eventController_VolumeSourceActiveStateChanged );

			eventController.BackupStarted += new VolumeBackupHandler( Spine_BackupStarted );
			eventController.BackupFinished += new VolumeBackupHandler( Spine_BackupFinished );
			eventController.BackupProgress += new VolumeBackupProgressHandler( Spine_BackupFileFinished );

			eventController.RestoreStarted += new VolumeRestoreHandler( spine_RestoreInitStarted );
			eventController.RestoreFinished += new VolumeRestoreHandler( Spine_RestoreFinished );
			eventController.RestoreProgress += new VolumeRestoreProgressHandler( spine_RestoreFileFinished );

			eventController.ApplicationInitialised += new EventHandler( Spine_SpineInitialised );

			eventController.FormatUsbFinished += new EventHandler( Spine_FormatUsbFinished );

			MainForm.Instance.SetDefaultStatusText();
		}

		void eventController_VolumeSourceActiveStateChanged( Volume volume, bool isActive )
		{
			mInvokeControl.Invoke( new VolumeDescriptorChangedHandler( eventController_VolumeDescriptorActiveStateChanged_WinThread ), volume.Descriptor, isActive );
		}

		void eventController_VolumeDescriptorActiveStateChanged( VolumeDescriptor volume, bool isActive )
		{
			mInvokeControl.Invoke( new VolumeDescriptorChangedHandler( eventController_VolumeDescriptorActiveStateChanged_WinThread ), volume, isActive );
		}

		void eventController_VolumeDescriptorActiveStateChanged_WinThread( VolumeDescriptor volume, bool isActive )
		{
			if ( VolumeNeedsRefreshing != null )
				VolumeNeedsRefreshing( volume, isActive );
		}


		void ShowBalloonTip( string text )
		{
			if ( MainForm.Instance.WindowState == FormWindowState.Minimized )
			{
				mNotifyIcon.BalloonTipText = text;
				mNotifyIcon.BalloonTipTitle = "AutoBackup";
				mNotifyIcon.ShowBalloonTip( 5000 );
			}
		}



		#region Spine events



		#region Restore events

		void spine_RestoreFileFinished( Volume volume, VolumeSnapshot snapshot, ulong fileNum, ulong fileCount, ulong bytesTranferred, ulong totalBytes )
		{
			mInvokeControl.Invoke( new VolumeBackupProgressHandler( spine_RestoreFileFinishedWinThread ), volume, snapshot, fileNum, fileCount, bytesTranferred, totalBytes );
		}

		void spine_RestoreFileFinishedWinThread( Volume volume, VolumeSnapshot snapshot, ulong fileNum, ulong fileCount, ulong bytesTranferred, ulong totalBytes )
		{
			while ( totalBytes > int.MaxValue )
			{
				totalBytes /= 10;
				bytesTranferred /= 10;
			}

			mProgressBar.Maximum = ( int )totalBytes + 1;
			mProgressBar.Value = ( int )bytesTranferred;

			int percentComplete = (int)(((float)bytesTranferred / (float)(totalBytes + 1)) * 100.0f );
			MainForm.Instance.SetTitleText( "Restoring", percentComplete );
			Log.WriteLine( LogType.DebugLog | LogType.TextLog, "File: " + fileNum.ToString() + "/" + fileCount.ToString() + " Progress: " + bytesTranferred.ToString() + "/" + totalBytes.ToString() );
		}


		void spine_RestoreInitStarted( Volume volume, VolumeSnapshot snapshot )
		{
			mInvokeControl.Invoke( new VolumeRestoreHandler( spine_RestoreInitStartedWinThread ), volume, snapshot );
		}

		void spine_RestoreInitStartedWinThread( Volume volume, VolumeSnapshot snapshot )
		{
//			ShowBalloonTip( "Restore of " + volume.Descriptor.Identifier.Name + " started" );
			MainForm.Instance.SetStatusText("Restoring...");
			MainForm.Instance.SetTitleText( "Restoring", 0 );
			if ( RestoreStarted != null )
				RestoreStarted( volume, snapshot );
		}

		void Spine_RestoreFinished( Volume volume, VolumeSnapshot snapshot )
		{
			mInvokeControl.Invoke( new VolumeRestoreHandler( Spine_RestoreFinishedWinThread ), volume, snapshot );
		}

		void Spine_RestoreFinishedWinThread( Volume volume, VolumeSnapshot snapshot )
		{
			ShowBalloonTip( "Restore of " + volume.Descriptor.VolumeName + " finished" );
			mProgressBar.Value = 0;
			MainForm.Instance.SetDefaultStatusText();
			MainForm.Instance.SetTitleText( "", 0 );
			Log.WriteLine( LogType.AppLog | LogType.TextLog | LogType.DebugLog, "Restoration of '" + volume.Descriptor.VolumeName + "' complete" );
			if (RestoreFinished != null)
				RestoreFinished( volume, snapshot );
		}

		#endregion



		#region Backup events


		void Spine_BackupFileFinished( Volume volume, VolumeSnapshot snapshot, ulong fileNum, ulong fileCount, ulong bytesTranferred, ulong totalBytes )
		{
			mInvokeControl.Invoke( new VolumeBackupProgressHandler( Spine_BackupFileFinishedWinThread ), volume, snapshot, fileNum, fileCount, bytesTranferred, totalBytes );
		}

		void Spine_BackupFileFinishedWinThread( Volume volume, VolumeSnapshot snapshot, ulong fileNum, ulong fileCount, ulong bytesTranferred, ulong totalBytes )
		{
			try
			{
				while ( totalBytes >= int.MaxValue )
				{
					totalBytes /= 10;
					bytesTranferred /= 10;

				}

				mProgressBar.Maximum = ( int )totalBytes + 1;
				mProgressBar.Value = ( int )bytesTranferred;

				int percentComplete = (int)(((float)bytesTranferred / (float)(totalBytes + 1)) * 100.0f);
				MainForm.Instance.SetTitleText( "Backing up", percentComplete );
			}
			catch ( System.Exception )
			{
				throw;
			}
		}


		void Spine_BackupFinished( Volume volume, VolumeSnapshot snapshot, bool snapshotHasBeenSaved, bool firstBackupOnLoad )
		{
			mInvokeControl.Invoke( new VolumeBackupHandler( Spine_BackupFinishedWinThread ), volume, snapshot, snapshotHasBeenSaved, firstBackupOnLoad );
		}

		void Spine_BackupStarted( Volume volume, VolumeSnapshot snapshot, bool snapshotHasBeenSaved, bool firstBackupOnLoad )
		{
			mInvokeControl.Invoke( new VolumeBackupHandler( Spine_BackupStartedWinThread ), volume, snapshot, snapshotHasBeenSaved, firstBackupOnLoad );
		}


		void Spine_BackupFinishedWinThread( Volume volume, VolumeSnapshot snapshot, bool snapshotHasBeenSaved, bool firstBackupOnLoad )
		{
			if ( !firstBackupOnLoad && snapshotHasBeenSaved )
				ShowBalloonTip( "Backup of " + volume.Descriptor.VolumeName + " finished" );
			MainForm.Instance.SetDefaultStatusText();
			mProgressBar.Value = 0;
			MainForm.Instance.SetTitleText( "", 0 );
			if ( snapshotHasBeenSaved )
				Log.WriteLine( LogType.AppLog | LogType.TextLog | LogType.DebugLog, "Backup of '" + volume.Descriptor.VolumeName + "' complete" );
			if ( BackupFinished != null )
				BackupFinished( volume, snapshot, snapshotHasBeenSaved, firstBackupOnLoad );
		}

		void Spine_BackupStartedWinThread( Volume volume, VolumeSnapshot snapshot, bool snapshotHasBeenSaved, bool firstBackupOnLoad )
		{
			MainForm.Instance.SetStatusText( "Backing up..." );
			if ( BackupStarted != null )
				BackupStarted( volume, snapshot, snapshotHasBeenSaved, firstBackupOnLoad );
			mProgressBar.Value = 0;
			MainForm.Instance.SetTitleText( "Backing up", 0 );
//			if ( !firstBackupOnLoad )
//				ShowBalloonTip( "Backup of " + volume.Descriptor.Identifier.Name + " started" );
		}


		#endregion


		#region Format USB events


		void Spine_FormatUsbFinished( object sender, EventArgs e )
		{
			mInvokeControl.Invoke( new EventHandler( Spine_FormatUsbFinishedWinThread ), sender, e );
		}

		void Spine_FormatUsbFinishedWinThread( object sender, EventArgs e )
		{
			MainForm.Instance.SetDefaultStatusText();
			if ( FormatUsbFinished != null )
				FormatUsbFinished( sender, e );
		}

		#endregion


		#region Spine events


		void Spine_SpineInitialised( object sender, EventArgs e )
		{
			mInvokeControl.Invoke( new EventHandler( Spine_SpineInitialisedWinThread ), sender, e );
		}

		void Spine_SpineInitialisedWinThread( object sender, EventArgs e )
		{
			MainForm.Instance.SetDefaultStatusText();
			if ( SpineInitialised != null )
				SpineInitialised( sender, e );
		}


		#endregion


		#endregion


	}
}


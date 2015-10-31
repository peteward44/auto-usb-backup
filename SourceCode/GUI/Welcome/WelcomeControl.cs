using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using PWLib.FileSyncLib;



namespace AutoUSBBackup.GUI
{
	public partial class WelcomeControl : UserControl, FormUserControl
	{
		public Shared.LogTextBox LogBox { get { return mTreeView.LogBox; } }


		public WelcomeControl()
		{
			InitializeComponent();

			MainForm.Instance.EventController.SpineInitialised += new EventHandler( Spine_SpineInitialised );
			MainForm.Instance.EventController.VolumeNeedsRefreshing += new VolumeDescriptorChangedHandler( EventController_VolumeDescriptorActiveChanged );
			MainForm.Instance.EventController.BackupStarted += new VolumeBackupHandler( Spine_BackupStartedWinThread );
			MainForm.Instance.EventController.BackupFinished += new VolumeBackupHandler( Spine_BackupFinishedWinThread );
			MainForm.Instance.EventController.RestoreStarted += new VolumeRestoreHandler( Spine_RestoreInitStartedWinThread );
			MainForm.Instance.EventController.RestoreFinished += new VolumeRestoreHandler( Spine_RestoreFinishedWinThread );

			mTreeView.AddNewVolumeMenuClicked += new AutoUSBBackup.GUI.Shared.WelcomeListView.VolumeEventDelegate( mTreeView_AddNewVolumeMenuClicked );
			mTreeView.RemoveVolumeMenuClicked += new AutoUSBBackup.GUI.Shared.WelcomeListView.VolumeEventDelegate( mTreeView_RemoveVolumeMenuClicked );
			mTreeView.RestoreVolumeMenuClicked += new AutoUSBBackup.GUI.Shared.WelcomeListView.VolumeEventDelegate( mTreeView_RestoreVolumeMenuClicked );
			mTreeView.TransferVolumeMenuClicked += new AutoUSBBackup.GUI.Shared.WelcomeListView.VolumeEventDelegate( mTreeView_TransferVolumeMenuClicked );
		}

		void EventController_VolumeDescriptorActiveChanged( VolumeDescriptor volume, bool isActive )
		{
			mTreeView.Reset();
		}

		void mTreeView_TransferVolumeMenuClicked( VolumeDescriptor vid )
		{
			MainForm.Instance.ControlSwitcher.SwitchUserControl( FormControlType.TransferVolume1, FormControlSwitchType.Start, vid );
		}

		void mTreeView_RestoreVolumeMenuClicked( VolumeDescriptor vid )
		{
			MainForm.Instance.ControlSwitcher.SwitchUserControl( FormControlType.RestoreVolume1, FormControlSwitchType.Start, vid );
		}

		void mTreeView_RemoveVolumeMenuClicked( VolumeDescriptor vid )
		{
			RemoveVolumeForm removeVolumeForm = new RemoveVolumeForm( vid.VolumeName );

			DialogResult dr = removeVolumeForm.ShowDialog( this );
			if ( dr == DialogResult.Yes )
			{
				MainForm.Instance.RemoveVolume( vid, removeVolumeForm.DeleteAllData );
			}
		}

		void mTreeView_AddNewVolumeMenuClicked( VolumeDescriptor vid )
		{
			MainForm.Instance.ControlSwitcher.SwitchUserControl( FormControlType.AddVolume2, FormControlSwitchType.Start );
		}


		void Spine_RestoreInitStartedWinThread( Volume volume, VolumeSnapshot snapshot )
		{
			mTreeView.LockInterface( true );
		}


		void Spine_RestoreFinishedWinThread( Volume volume, VolumeSnapshot snapshot )
		{
			mTreeView.LockInterface( false );
		}


		void Spine_BackupStartedWinThread( Volume volume, VolumeSnapshot snapshot, bool snapshotHasBeenSaved, bool firstBackupOnLoad )
		{
			mTreeView.SetVolumeAsBusy( volume.Descriptor, true );
		}

		void Spine_BackupFinishedWinThread( Volume volume, VolumeSnapshot snapshot, bool snapshotHasBeenSaved, bool firstBackupOnLoad )
		{
			mTreeView.SetVolumeAsBusy( volume.Descriptor, false );
		}

		void Spine_SpineInitialised( object sender, EventArgs e )
		{
			mTreeView.Reset();
		}


		public void OnControlHide( FormControlSwitchType switchType, object userObject )
		{
		}

		public void OnControlShow( FormControlSwitchType switchType, object userObject )
		{
			mTreeView.Reset();
		}

		public Control GetControl() { return this; }

		private void mTreeView_Load( object sender, EventArgs e )
		{

		}
	}
}

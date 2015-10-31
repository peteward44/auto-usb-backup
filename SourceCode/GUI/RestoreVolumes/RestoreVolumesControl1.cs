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
	public partial class RestoreVolumesControl1 : UserControl, FormUserControl
	{
		RestoreVolumesControlObject mObject = null;
		RevisionListObject mSelectedObject = null;

		class RevisionListObject
		{
			VolumeSnapshotRevision mRevision;
			string mDateString;
			string mFuzzyString;
			public VolumeSnapshotRevision Revision { get { return mRevision; } }

			public string Name { get { return mRevision.ToString(); } }
			public string DateString { get { return mDateString; } }

			public string FuzzyTimeString { get { return mFuzzyString; } }

			public RevisionListObject( VolumeSnapshotRevision revision )
			{
				mRevision = revision;
				DateTime dt = mRevision.CreationTime;
				mDateString = dt.ToLongDateString() + " " + dt.ToLongTimeString();
				mFuzzyString = PWLib.Platform.Windows.Misc.FormatFuzzyDateString( dt );
			}

			public override bool Equals( object obj )
			{
				if ( obj != null && obj is RevisionListObject )
				{
					bool eq = ( (RevisionListObject)obj ).mRevision.Equals( mRevision );
					return eq;
				}
				return false;
			}

			public override int GetHashCode()
			{
				return mRevision.GetHashCode();
			}
		}


		public RestoreVolumesControl1()
		{
			InitializeComponent();

			mRevisionListView.ShowGroups = false;
			mRevisionListView.SelectedIndexChanged += new EventHandler( mRevisionListView_SelectedIndexChanged );

			mRevisionListView.CheckStateGetter = new BrightIdeasSoftware.CheckStateGetterDelegate( CheckStateGetterMethod );
			mRevisionListView.CheckStatePutter = new BrightIdeasSoftware.CheckStatePutterDelegate( CheckStateSetterMethod );
		}
		

		CheckState CheckStateGetterMethod( object x )
		{
			System.Diagnostics.Debug.Assert( x is RevisionListObject );
			return x.Equals( mSelectedObject ) ? CheckState.Checked : CheckState.Unchecked;
		}


		CheckState CheckStateSetterMethod( object x, CheckState checkState )
		{
			RevisionListObject oldObject = mSelectedObject;
			mSelectedObject = checkState == CheckState.Checked ? (RevisionListObject)x : null;

			if ( mRevisionListView.SelectedObject == null || !mRevisionListView.SelectedObject.Equals( mSelectedObject ) )
			{
				mRevisionListView.Focus();
				mRevisionListView.SelectedObject = mSelectedObject;
			}

			mRevisionListView.RefreshObject( oldObject );
			mRevisionListView.RefreshObject( mSelectedObject );

			mButtonNext.Enabled = mSelectedObject != null;
			
			return checkState;
		}


		void mRevisionListView_SelectedIndexChanged( object sender, EventArgs e )
		{
			object modelObj = mRevisionListView.SelectedObject;
			if ( modelObj != null && !modelObj.Equals( mSelectedObject ) )
				mRevisionListView.CheckObject( modelObj );
		}


		public void OnControlHide( FormControlSwitchType switchType, object userObject )
		{
		}

		public void OnControlShow( FormControlSwitchType switchType, object userObject )
		{
			if ( switchType == FormControlSwitchType.Start )
			{
				mObject = new RestoreVolumesControlObject();
				System.Diagnostics.Debug.Assert( userObject is VolumeDescriptor && userObject != null );
				mObject.mVolumeId = ( VolumeDescriptor )userObject;
				Init();
			}
		}


		public Control GetControl() { return this; }


		private void mButtonBack_Click(object sender, EventArgs e)
		{
			MainForm.Instance.ControlSwitcher.SwitchUserControl(FormControlType.Welcome, FormControlSwitchType.Finish );
		}

		private void mButtonNext_Click(object sender, EventArgs e)
		{
			mObject.mRevision = mSelectedObject.Revision;
			MainForm.Instance.ControlSwitcher.SwitchUserControl( FormControlType.RestoreVolume2, FormControlSwitchType.Next, mObject );
		}


		void Init()
		{
			mSelectedObject = null;
			mButtonNext.Enabled = false;

			mRevisionListView.ClearObjects();
			List<RevisionListObject> revisionList = new List<RevisionListObject>();

			// Set text of object we wish to restore
			mRestoreFileNameLabel.Text = mObject.mVolumeId.VolumeName;

			// Restoring a whole volume - just put all the revisions for this volume in the box
			List<VolumeSnapshotRevision> revisionHistoryList = mObject.mVolumeId.Volume.Database.GetRevisionHistory();
			revisionHistoryList.Reverse(); // add them in chronological order, with the newest revision at the top
			foreach ( VolumeSnapshotRevision revision in revisionHistoryList )
			{
				revisionList.Add( new RevisionListObject( revision ) );
			}

			if ( revisionList.Count == 0 )
			{
				// Shouldn't happen - no revisions found
				MessageBox.Show( this, "No revisions found for selected volume", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
				MainForm.Instance.ControlSwitcher.SwitchUserControl( FormControlType.Welcome, FormControlSwitchType.Finish );
			}
			else
			{
				mRevisionListView.SetObjects( revisionList );
				mRevisionListView.CheckObject( revisionList[ 0 ] ); // set newest as default option
			}
		}
	}
}

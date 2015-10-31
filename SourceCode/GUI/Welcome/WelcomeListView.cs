using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;
using PWLib.FileSyncLib;



namespace AutoUSBBackup.GUI.Shared
{
	/// <summary>
	/// Has a tree view to the left and a file list view to the right. Can select volumes in both like in explorer. Used in the file restoration page.
	/// </summary>
	public partial class WelcomeListView : UserControl
	{

		#region Internal objects for tree view


		public abstract class IInternalObject : CheckedModelItem, IFileImageModelObject
		{
			public IInternalObject( IInternalObject parent )
				: base( parent )
			{
			}

			public abstract string Name { get; }


			public abstract FileImageListViewHelper.IconUsage IconUsage { get; }
			public abstract int UserSpecifiedImageListIndex { get; }
			public abstract string FileExtension { get; }

			public abstract VolumeDescriptor VolumeDescriptor { get; }

            public abstract string FilePath { get; }
            public abstract bool IsDirectory { get; }
		}


		public class InternalRootObject : IInternalObject
		{
			bool mIsDirty = false;
			int mIconIndex, mBusyIconIndex;

			VolumeDescriptor mVolumeDescriptor;
			string mAlternativeName = "";
			bool mIsBusy = false;

			// Busy flag indicates volume is backing up / restoring so no operation can be performed on it
			public bool IsBusy { get { return mIsBusy; } set { mIsBusy = value; } }

			// Dirty flag indicates the cached children objects are out of date and need rebuilding.
			public bool Dirty { get { return mIsDirty; } set { mIsDirty = value; } }


			public InternalRootObject( VolumeDescriptor vid, int iconIndex, int busyIconIndex, string alternativeName )
				: base( null )
			{
				mVolumeDescriptor = vid;
				mIconIndex = iconIndex;
				mBusyIconIndex = busyIconIndex;
				mAlternativeName = alternativeName;
			}


			public override bool Equals( object obj )
			{
				if ( obj is InternalRootObject )
				{
					InternalRootObject lvo = (InternalRootObject)obj;
					return lvo.mVolumeDescriptor.Equals( this.mVolumeDescriptor );
				}
				return false;
			}

			public override int GetHashCode()
			{
				return mVolumeDescriptor.GetHashCode();
			}

			public override int CompareTo( object obj )
			{
				if ( obj is WelcomeListView )
				{
					InternalRootObject lvo = (InternalRootObject)obj;
					return string.Compare( lvo.mVolumeDescriptor.VolumeName, this.mVolumeDescriptor.VolumeName, true );
				}
				else
					return 1;
			}

			public override string Name { get { return mVolumeDescriptor.VolumeName; } }

			public override FileImageListViewHelper.IconUsage IconUsage
			{
				get
				{
					return FileImageListViewHelper.IconUsage.UserSpecified;
				}
			}

			public override int UserSpecifiedImageListIndex
			{
				get
				{
					return mIsBusy ? mBusyIconIndex : mIconIndex;
				}
			}

			public override string FileExtension { get { return ""; } }

			public override VolumeDescriptor VolumeDescriptor { get { return mVolumeDescriptor; } }

      public override string FilePath { get { return "\\"; } }
      public override bool IsDirectory { get { return true; } }
		}


		#endregion


		ImageList mImageList = new ImageList();
		ImageList mFileImageList = new ImageList();

		FileImageListViewHelper mFileImageListTreeViewHelper;

		public delegate void VolumeEventDelegate( VolumeDescriptor vid );

		public event VolumeEventDelegate RestoreVolumeMenuClicked;
		public event VolumeEventDelegate AddNewVolumeMenuClicked;
		public event VolumeEventDelegate RemoveVolumeMenuClicked;
		public event VolumeEventDelegate TransferVolumeMenuClicked;

		// Icon for root branch object
		Bitmap mIcon = null, mBusyIcon = null;
		int mIconIndex = -1, mBusyIconIndex = -1;
		int mNullIconIndex = -1;
		Bitmap mNullIcon = null;
		bool mInterfaceLocked = false;
		bool mInterfaceLockedOverride = false;

		int mFolderIconIndex = -1, mFolderBusyIconIndex = -1;
		Bitmap mFolderIcon, mFolderBusyIcon;

		List<InternalRootObject> mObjectList = new List<InternalRootObject>();
		InternalRootObject mSelectedObject = null;

		BrightIdeasSoftware.OLVColumn mStatusColumn = new BrightIdeasSoftware.OLVColumn();
		BrightIdeasSoftware.OLVColumn mLastBackupColumn = new BrightIdeasSoftware.OLVColumn();
		BrightIdeasSoftware.OLVColumn mSizeOnDiskColumn = new BrightIdeasSoftware.OLVColumn();
		BrightIdeasSoftware.OLVColumn mRevisionsAvailableColumn = new BrightIdeasSoftware.OLVColumn();
		BrightIdeasSoftware.OLVColumn mPathColumn = new BrightIdeasSoftware.OLVColumn();

		public LogTextBox LogBox { get { return mLogTextBox; } }


		void CreateIcon( System.Reflection.Assembly assem, string name, out int index, out Bitmap bitmap )
		{
			index = -1;
			bitmap = null;
			try
			{
				if ( assem != null )
				{
					Stream busyIconStream = assem.GetManifestResourceStream( name );
					if ( busyIconStream != null )
					{
						bitmap = new Bitmap( busyIconStream );
						index = mFileImageListTreeViewHelper.AddUserSpecifiedImage( bitmap );
					}
				}
			}
			catch ( Exception )
			{
			}
		}


		public WelcomeListView()
		{
			InitializeComponent();

			MainForm.Instance.EventController.VolumeNeedsRefreshing += new VolumeDescriptorChangedHandler( EventController_VolumeDescriptorActiveChanged );

			this.mVolumeListView.EmptyListMsg = "Press 'Add' to set up a new backup volume";

            mFileImageListTreeViewHelper = new FileImageListViewHelper(mColumn, mImageList);

			mColumn.AspectName = "Name";

			mStatusColumn.Text = "Status";
			mStatusColumn.Width = 100;
			mStatusColumn.TextAlign = HorizontalAlignment.Left;
			mStatusColumn.AspectGetter = delegate( object modelObject )
			{
				InternalRootObject rootObject = (( InternalRootObject )modelObject );
				return rootObject.VolumeDescriptor.StateString;
			};

			mLastBackupColumn.Text = "Last backup";
			mLastBackupColumn.Width = 100;
			mLastBackupColumn.TextAlign = HorizontalAlignment.Center;
			mLastBackupColumn.AspectGetter = delegate( object modelObject )
			{
				InternalRootObject rootObject = ( ( InternalRootObject )modelObject );
				if ( rootObject.VolumeDescriptor != null && rootObject.VolumeDescriptor.IsAvailable )
				{
					if ( rootObject.VolumeDescriptor.Volume != null && rootObject.VolumeDescriptor.Volume.BackupInProgress )
						return "In Progress";
					else
						return rootObject.VolumeDescriptor.LastAttemptedBackupFuzzyString;
				}
				else
					return "";
			};

			mPathColumn.Text = "Path";
			mPathColumn.Width = 180;
			mPathColumn.TextAlign = HorizontalAlignment.Left;
			mPathColumn.AspectGetter = delegate( object modelObject )
			{
				InternalRootObject rootObject = ( ( InternalRootObject )modelObject );
				if ( rootObject.VolumeDescriptor != null && rootObject.VolumeDescriptor.IsAvailable )
				{
					return rootObject.VolumeDescriptor.Volume.Source.GetOnDiskPath( "" );
				}
				else
					return "";
			};

			mSizeOnDiskColumn.Text = "Size on disk";
			mSizeOnDiskColumn.Width = 80;
			
			mSizeOnDiskColumn.TextAlign = HorizontalAlignment.Center;
			mSizeOnDiskColumn.AspectGetter = delegate( object modelObject )
			{
				InternalRootObject rootObject = ( ( InternalRootObject )modelObject );
				return rootObject.VolumeDescriptor != null && rootObject.VolumeDescriptor.IsAvailable ? rootObject.VolumeDescriptor.TotalDatabaseSizeString : "";
			};

			mRevisionsAvailableColumn.Text = "Revisions";
			mRevisionsAvailableColumn.Width = 60;
			mRevisionsAvailableColumn.TextAlign = HorizontalAlignment.Center;
			mRevisionsAvailableColumn.AspectGetter = delegate( object modelObject )
			{
				InternalRootObject rootObject = ( ( InternalRootObject )modelObject );
				return rootObject.VolumeDescriptor != null && rootObject.VolumeDescriptor.IsAvailable ? rootObject.VolumeDescriptor.Volume.Database.GetRevisionHistory().Count.ToString() : "";
			};

			mVolumeListView.Columns.Add( mStatusColumn );
			mVolumeListView.Columns.Add( mLastBackupColumn );
			mVolumeListView.Columns.Add( mPathColumn );
			mVolumeListView.Columns.Add( mSizeOnDiskColumn );
			mVolumeListView.Columns.Add( mRevisionsAvailableColumn );

			mVolumeListView.SelectedIndexChanged += new EventHandler( mVolumeListView_SelectedIndexChanged );

			mVolumeListView.CustomSorter = new BrightIdeasSoftware.SortDelegate( CustomSortHandler ); 

			System.Reflection.Assembly assem = System.Reflection.Assembly.GetEntryAssembly();

//			System.Diagnostics.Debug.WriteLine( string.Join( ", ", assem.GetManifestResourceNames() ) );

			CreateIcon( assem, "AutoUSBBackup.Resources.pendrive32x32.png", out mIconIndex, out mIcon );
			CreateIcon( assem, "AutoUSBBackup.Resources.pendrivebusy32x32.png", out mBusyIconIndex, out mBusyIcon );
			CreateIcon( assem, "AutoUSBBackup.Resources.localfolder32x32.png", out mFolderIconIndex, out mFolderIcon );
			CreateIcon( assem, "AutoUSBBackup.Resources.localfolderbusy32x32.png", out mFolderBusyIconIndex, out mFolderBusyIcon );
			CreateIcon( assem, "", out mNullIconIndex, out mNullIcon );

			mVolumeListView.LargeImageList = mImageList;
			mVolumeListView.SmallImageList = mImageList;
			mVolumeListView.ShowImagesOnSubItems = true;

			mButtonOptions.Visible = false;

			MainForm.Instance.EventController.BackupFinished += new VolumeBackupHandler( Spine_BackupFinished );

			mVolumeListView.CheckStateGetter = new BrightIdeasSoftware.CheckStateGetterDelegate( CheckStateGetterMethod );
			mVolumeListView.CheckStatePutter = new BrightIdeasSoftware.CheckStatePutterDelegate( CheckStateSetterMethod );
		}


		CheckState CheckStateGetterMethod( object x )
		{
			System.Diagnostics.Debug.Assert( x is InternalRootObject );
			return x.Equals( mSelectedObject ) ? CheckState.Checked : CheckState.Unchecked;
		}


		CheckState CheckStateSetterMethod( object x, CheckState checkState )
		{
			InternalRootObject oldObject = mSelectedObject;
			mSelectedObject = checkState == CheckState.Checked ? ( InternalRootObject )x : null;

			if ( mVolumeListView.SelectedObject == null || !mVolumeListView.SelectedObject.Equals( mSelectedObject ) )
			{
				mVolumeListView.Focus();
				mVolumeListView.SelectedObject = mSelectedObject;
			}

			mVolumeListView.RefreshObject( oldObject );
			mVolumeListView.RefreshObject( mSelectedObject );

			UpdateButtonStates();

			return checkState;
		}



		void mVolumeListView_SelectedIndexChanged( object sender, EventArgs e )
		{
			object modelObj = mVolumeListView.SelectedObject;
			if ( modelObj != null && !modelObj.Equals( mSelectedObject ) )
				mVolumeListView.CheckObject( modelObj );
		}


		void CustomSortHandler( BrightIdeasSoftware.OLVColumn column, SortOrder order )
		{
			if ( column.Equals( mSizeOnDiskColumn ) )
			{
				mVolumeListView.ListViewItemSorter = new WelcomeListViewCustomSorter( order,
					delegate( bool ascending, WelcomeListView.InternalRootObject rootObjectX, WelcomeListView.InternalRootObject rootObjectY )
					{
						long x = ( long )( rootObjectX.VolumeDescriptor != null ? rootObjectX.VolumeDescriptor.TotalDatabaseSize : 0 );
						long y = ( long )( rootObjectY.VolumeDescriptor != null ? rootObjectY.VolumeDescriptor.TotalDatabaseSize : 0 );
						long diff = ( ascending ? x - y : y - x );
						if ( diff == 0 )
							return 0;
						else
							return diff > 0 ? 1 : -1;
					} );
			}
			else if ( column.Equals( mLastBackupColumn ) )
			{
				mVolumeListView.ListViewItemSorter = new WelcomeListViewCustomSorter( order,
					delegate( bool ascending, WelcomeListView.InternalRootObject rootObjectX, WelcomeListView.InternalRootObject rootObjectY )
					{
						long x = ( rootObjectX.VolumeDescriptor != null ? rootObjectX.VolumeDescriptor.LastAttemptedBackup.Ticks : 0 );
						long y = ( rootObjectY.VolumeDescriptor != null ? rootObjectY.VolumeDescriptor.LastAttemptedBackup.Ticks : 0 );
						long diff = ( ascending ? x - y : y - x );
						if ( diff == 0 )
							return 0;
						else
							return diff > 0 ? 1 : -1;
					} );
			}
			else if ( column.Equals( mRevisionsAvailableColumn ) )
			{
				mVolumeListView.ListViewItemSorter = new WelcomeListViewCustomSorter( order,
						delegate( bool ascending, WelcomeListView.InternalRootObject rootObjectX, WelcomeListView.InternalRootObject rootObjectY )
						{
							int x = ( rootObjectX.VolumeDescriptor.Volume != null ? rootObjectX.VolumeDescriptor.Volume.Database.GetRevisionHistory().Count : 0 );
							int y = ( rootObjectY.VolumeDescriptor.Volume != null ? rootObjectY.VolumeDescriptor.Volume.Database.GetRevisionHistory().Count : 0 );
							int diff = ( ascending ? x - y : y - x );
							if ( diff == 0 )
								return 0;
							else
								return diff > 0 ? 1 : -1;
						} );
			}
			else
				mVolumeListView.ListViewItemSorter = new BrightIdeasSoftware.ColumnComparer( column, order );
		}


		void EventController_VolumeDescriptorActiveChanged( VolumeDescriptor volume, bool isActive )
		{
			Reset();
		}


		void Spine_BackupFinished( Volume volume, VolumeSnapshot snapshot, bool snapshotHasBeenSaved, bool firstBackupOnLoad )
		{
			mVolumeListView.Refresh();
		}


		InternalRootObject FindRootObject( VolumeDescriptor vid )
		{
			foreach ( InternalRootObject rootObj in this.mVolumeListView.Objects )
			{
				if ( rootObj.VolumeDescriptor.IsAvailable && rootObj.VolumeDescriptor.Equals( vid ) )
					return rootObj;
			}
			return null;
		}


		public void SetVolumeAsBusy( VolumeDescriptor vid, bool busy )
		{
			InternalRootObject rootObj = FindRootObject( vid );
			if ( rootObj != null )
			{
				if ( rootObj.IsBusy && !busy )
					rootObj.Dirty = true; // operation has just finished, contents may have changed
				rootObj.IsBusy = busy;
				rootObj.IsChecked = false;

//				Log.WriteLine( "Setting " + rootObj.VolumeIdentifier.Name + " to " + ( busy ? "busy" : "not busy" ) );
				this.mVolumeListView.RefreshObject( rootObj );
			}

			LockInterface( mInterfaceLockedOverride );
		}


		public void LockInterface( bool lockIt )
		{
			bool lockInterface = false;
			foreach ( InternalRootObject obj in this.mVolumeListView.Objects )
			{
				if ( obj.IsBusy )
				{
					lockInterface = true;
					break;
				}
			}

			mInterfaceLockedOverride = lockIt;
			mInterfaceLocked = lockInterface || mInterfaceLockedOverride;

			if ( mSelectedObject != null )
				mVolumeListView.UncheckObject( mSelectedObject );
			mSelectedObject = null;
			mVolumeListView_SelectedIndexChanged( null, null );

			mBackupAllNowButton.Enabled = !mInterfaceLocked;
			mAddVolumeButton.Enabled = !mInterfaceLocked;
		}


		void UpdateButtonStates()
		{
			bool loaded = mSelectedObject != null ? mSelectedObject.VolumeDescriptor.Volume != null : false;
			bool enabled = mSelectedObject != null && !mInterfaceLocked;
			mRemoveVolumeButton.Enabled = enabled && loaded;
			mRestoreVolumeButton.Enabled = enabled && loaded;
		}


		public void Reset()
		{
			mObjectList.Clear();

			// Add active volumes
			foreach ( VolumeDescriptor desc in VolumeDescriptorList.Instance.Descriptors )
			{
				if ( desc.IsAvailable )
				{
					bool isUsbDevice = desc.Volume.Source.VolumeType == VolumeType.UsbDevice;
					mObjectList.Add( new InternalRootObject( desc, isUsbDevice ? mIconIndex : mFolderIconIndex, isUsbDevice ? mBusyIconIndex : mFolderBusyIconIndex, "" ) );
				}
				else
				{
					mObjectList.Add( new InternalRootObject( desc, mNullIconIndex, mNullIconIndex, desc.VolumeName ) );
				}
			}

			mVolumeListView.SetObjects( mObjectList );

			if ( mSelectedObject != null )
				mVolumeListView.UncheckObject( mSelectedObject );
			mSelectedObject = null;
			mVolumeListView_SelectedIndexChanged( null, null );

			mVolumeListView.Refresh();
		}


		private void RestoreVolumeEvent()
		{
			if ( mSelectedObject != null )
			{
				if ( RestoreVolumeMenuClicked != null )
					RestoreVolumeMenuClicked( mSelectedObject.VolumeDescriptor );
			}

			Reset();
		}


		private void AddVolumeEvent()
		{
			if ( AddNewVolumeMenuClicked != null )
				AddNewVolumeMenuClicked( null );

			Reset();
		}


		private void RemoveVolumeEvent()
		{
			if ( mSelectedObject != null )
			{
				if ( RemoveVolumeMenuClicked != null )
					RemoveVolumeMenuClicked( mSelectedObject.VolumeDescriptor );
			}

			Reset();
		}


		private void TransferVolumeEvent()
		{
			if ( mSelectedObject != null )
			{
				if ( TransferVolumeMenuClicked != null )
					TransferVolumeMenuClicked( mSelectedObject.VolumeDescriptor );
			}
		}


		private void BackupAllNowEvent()
		{
			MainForm.Instance.ForceFullBackup();
		}


		private void restoreVolumeToolStripMenuItem_Click( object sender, EventArgs e )
		{
			RestoreVolumeEvent();
		}

		private void addNewVolumeToolStripMenuItem_Click( object sender, EventArgs e )
		{
			AddVolumeEvent();
		}

		private void removeVolumeToolStripMenuItem_Click( object sender, EventArgs e )
		{
			RemoveVolumeEvent();
		}

		private void transferVolumeToolStripMenuItem_Click( object sender, EventArgs e )
		{
			TransferVolumeEvent();
		}

		private void mAddVolumeButton_Click( object sender, EventArgs e )
		{
			AddVolumeEvent();
		}

		private void mRestoreVolumeButton_Click( object sender, EventArgs e )
		{
			RestoreVolumeEvent();
		}

		private void mRemoveVolumeButton_Click( object sender, EventArgs e )
		{
			RemoveVolumeEvent();
		}

		private void mButtonTransfer_Click( object sender, EventArgs e )
		{
			TransferVolumeEvent();
		}


		private void mButtonOptions_Click( object sender, EventArgs e )
		{

		}

		private void mBackupAllNowButton_Click( object sender, EventArgs e )
		{
			BackupAllNowEvent();
		}

		private void mVolumeContextMenu_Opening( object sender, CancelEventArgs e )
		{
			bool enabled = mSelectedObject != null;
			restoreVolumeToolStripMenuItem.Enabled = enabled;
			removeVolumeToolStripMenuItem.Enabled = enabled;
			openLocationInExplorer.Enabled = enabled && mSelectedObject.VolumeDescriptor.IsAvailable && mSelectedObject.VolumeDescriptor.Volume != null && mSelectedObject.VolumeDescriptor.Volume.Source.MediaAvailable && PWLib.Platform.Windows.Directory.Exists( mSelectedObject.VolumeDescriptor.Volume.Source.GetOnDiskPath( "\\" ) );

			string dirPath = PWLib.Platform.Windows.Path.GetStemName( mSelectedObject.VolumeDescriptor.VolumeFilename );
			openBackupLocationInExplorer.Enabled = enabled && PWLib.Platform.Windows.Directory.Exists( dirPath );
			//transferVolumeToolStripMenuItem.Enabled = enabled;
		}


		void OpenExplorerWindow( string path )
		{
			if ( PWLib.Platform.Windows.Directory.Exists( path ) )
				System.Diagnostics.Process.Start( path );
		}

		
		private void openLocationInExplorer_Click( object sender, EventArgs e )
		{
			if ( mSelectedObject != null && mSelectedObject.VolumeDescriptor.Volume.Source.MediaAvailable )
				OpenExplorerWindow( mSelectedObject.VolumeDescriptor.Volume.Source.GetOnDiskPath( "\\" ) );
		}

		private void openBackupLocationInExplorer_Click( object sender, EventArgs e )
		{
			OpenExplorerWindow( PWLib.Platform.Windows.Path.GetStemName( mSelectedObject.VolumeDescriptor.VolumeFilename ) );
		}

	}








	public delegate int GoCompareDelegate( bool ascending, WelcomeListView.InternalRootObject rootObjectX, WelcomeListView.InternalRootObject rootObjectY );

	public class WelcomeListViewCustomSorter : IComparer
	{
		SortOrder mSortOrder;
		GoCompareDelegate mGoCompare;

		public WelcomeListViewCustomSorter( SortOrder sortOrder, GoCompareDelegate goCompare )
		{
			System.Diagnostics.Debug.Assert( goCompare != null );

			mSortOrder = sortOrder;
			mGoCompare = goCompare;
		}



		int IComparer.Compare( Object lhs, Object rhs )
		{
			System.Diagnostics.Debug.Assert( lhs is BrightIdeasSoftware.OLVListItem && rhs is BrightIdeasSoftware.OLVListItem );

			BrightIdeasSoftware.OLVListItem xolv = ( BrightIdeasSoftware.OLVListItem )lhs;
			BrightIdeasSoftware.OLVListItem yolv = ( BrightIdeasSoftware.OLVListItem )rhs;

			System.Diagnostics.Debug.Assert( xolv.RowObject is WelcomeListView.InternalRootObject && yolv.RowObject is WelcomeListView.InternalRootObject );

			WelcomeListView.InternalRootObject rootObjectx = ( ( WelcomeListView.InternalRootObject )xolv.RowObject );
			WelcomeListView.InternalRootObject rootObjecty = ( ( WelcomeListView.InternalRootObject )yolv.RowObject );

			return mGoCompare( mSortOrder == SortOrder.Ascending, rootObjectx, rootObjecty );
		}
	}


}

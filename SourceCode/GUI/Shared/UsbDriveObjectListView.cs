using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace AutoUSBBackup.GUI.Shared
{
	/// <summary>
	/// Has a tree view to the left and a file list view to the right. Can select volumes in both like in explorer. Used in the file restoration page.
	/// </summary>
	public partial class UsbDriveObjectListView : UserControl
	{
		ImageList mImageList = new ImageList();
		public event EventHandler CheckedObjectChanged;
		object mCheckedItem = null;

		private BrightIdeasSoftware.OLVColumn mModelNameColumn;
		private List<BrightIdeasSoftware.OLVColumn> mColumns = new List<BrightIdeasSoftware.OLVColumn>();

		public BrightIdeasSoftware.ObjectListView InternalListView { get { return mTreeView; } }

		public delegate bool ItemVerifierDelegate( PWLib.UsbDrive.UsbDriveInfo driveInfo );
		public ItemVerifierDelegate DynamicItemVerifier = null;


		public PWLib.UsbDrive.UsbDriveInfo SelectedDriveObject
		{
			get
			{
				if ( mCheckedItem != null && mCheckedItem is PWLib.UsbDrive.UsbDriveInfo )
					return ( ( PWLib.UsbDrive.UsbDriveInfo )mCheckedItem );
				else
					return null;
			}
		}


		// Monitors active usb drives and displays them, alternatively displays list of usb devices already provided
		public UsbDriveObjectListView()
		{
			InitializeComponent();

			CreateCommonControls();

			BrightIdeasSoftware.OLVColumn volumeName = new BrightIdeasSoftware.OLVColumn();

			volumeName.AspectName = "DriveId.DriveRootDirectory";
			volumeName.Text = "Drive";
			volumeName.Width = 40;
			mTreeView.Columns.Add( volumeName );
			mColumns.Add( volumeName );

			BrightIdeasSoftware.OLVColumn sizeColumn = new BrightIdeasSoftware.OLVColumn();
			sizeColumn.AspectName = "DriveId.SizeString";
			sizeColumn.Text = "Size";
			sizeColumn.Width = 80;
			mTreeView.Columns.Add( sizeColumn );
			mColumns.Add( sizeColumn );

			if ( PWLib.UsbDrive.UsbDriveList.Instance != null )
				UsbDriveList_OnCreate( null, null );
			else
				PWLib.UsbDrive.UsbDriveList.OnCreate += new EventHandler( UsbDriveList_OnCreate );

			RefreshDynamicList();
		}


		void UsbDriveList_OnCreate( object sender, EventArgs e )
		{
			PWLib.UsbDrive.UsbDriveList.Instance.DeviceInserted += new PWLib.UsbDrive.UsbDriveList.DeviceEventHandler( Instance_DeviceInserted );
			PWLib.UsbDrive.UsbDriveList.Instance.DeviceRemoved += new PWLib.UsbDrive.UsbDriveList.DeviceEventHandler( Instance_DeviceRemoved );
		}


		UsbDriveObjectListView( List<PWLib.UsbDrive.UsbDriveInfo> driveInfoList )
		{
			InitializeComponent();

			CreateCommonControls();

			BrightIdeasSoftware.OLVColumn volumeName = new BrightIdeasSoftware.OLVColumn();

			volumeName.AspectName = "VolumeName";
			volumeName.Text = "Volume";
			volumeName.Width = 150;
			mTreeView.Columns.Add( volumeName );

			mColumns.Add( volumeName );

			RefreshStaticList( driveInfoList );
		}


		void CreateCommonControls()
		{
			mTreeView.SmallImageList = mImageList;
			mTreeView.ItemChecked += new ItemCheckedEventHandler( mTreeView_ItemChecked );

			mTreeView.CheckStateGetter = new BrightIdeasSoftware.CheckStateGetterDelegate( CheckStateGetterMethod );
			mTreeView.CheckStatePutter = new BrightIdeasSoftware.CheckStatePutterDelegate( CheckStateSetterMethod );

			mModelNameColumn = new BrightIdeasSoftware.OLVColumn();
			mModelNameColumn.AspectName = "ModelName";
			mModelNameColumn.AspectName = "ModelName";
			mModelNameColumn.Text = "Name";
			mModelNameColumn.Width = 300;
			mModelNameColumn.FillsFreeSpace = true;

			mTreeView.Columns.Add( mModelNameColumn );
		}


		public void RefreshStaticList( List<PWLib.UsbDrive.UsbDriveInfo> driveInfoList )
		{
			mCheckedItem = null;
			mTreeView.Refresh();
			mTreeView.SetObjects( driveInfoList );
		}


		public void RefreshDynamicList()
		{
			mCheckedItem = null;
			mTreeView.Refresh();

			if ( PWLib.UsbDrive.UsbDriveList.Instance != null )
			{
				List<PWLib.UsbDrive.UsbDriveInfo> driveInfoList = PWLib.UsbDrive.UsbDriveList.Instance.BuildActiveDriveList();

				if ( DynamicItemVerifier != null )
				{
					List<PWLib.UsbDrive.UsbDriveInfo> newDriveInfoList = new List<PWLib.UsbDrive.UsbDriveInfo>();
					foreach ( PWLib.UsbDrive.UsbDriveInfo driveInfo in driveInfoList )
					{
						if ( DynamicItemVerifier( driveInfo ) )
							newDriveInfoList.Add( driveInfo );
					}
					mTreeView.SetObjects( newDriveInfoList );
				}
				else
					mTreeView.SetObjects( driveInfoList );
			}
		}


		void Instance_DeviceInserted( object sender, PWLib.UsbDrive.UsbDriveInfo drive )
		{
			List<PWLib.UsbDrive.UsbDriveInfo> driveInfo = PWLib.UsbDrive.UsbDriveList.Instance.BuildActiveDriveList();
			mTreeView.SetObjects( driveInfo );
		}

		void Instance_DeviceRemoved( object sender, PWLib.UsbDrive.UsbDriveInfo drive )
		{
			List<PWLib.UsbDrive.UsbDriveInfo> driveInfo = PWLib.UsbDrive.UsbDriveList.Instance.BuildActiveDriveList();
			mTreeView.SetObjects( driveInfo );
		}


		CheckState CheckStateGetterMethod( object x )
		{
			if ( mCheckedItem != null && x != null )
				return ( x.Equals( mCheckedItem ) ? CheckState.Checked : CheckState.Unchecked );
			return CheckState.Unchecked;
		}


		CheckState CheckStateSetterMethod( object x, CheckState checkState )
		{
			if ( checkState == CheckState.Checked )
			{
				object oldCheckedItem = mCheckedItem;
				mCheckedItem = x;
				mTreeView.RefreshObject( oldCheckedItem );
			}
			else
			{
				mCheckedItem = null;
				mTreeView.RefreshObject( x );
			}
			return checkState;
		}


		void mTreeView_ItemChecked( object sender, ItemCheckedEventArgs e )
		{
			if ( CheckedObjectChanged != null )
				CheckedObjectChanged( this, null );
		}

	}
}

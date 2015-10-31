using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace AutoUSBBackup.GUI
{
	// Add volume from already existing .vol file
	public partial class AddNewVolumeControl2c : UserControl, FormUserControl
	{
		AddNewVolumeControlObject mObject = null;

		public AddNewVolumeControl2c()
		{
			InitializeComponent();

			mLocalFolderLabel.Text = "";
		}


		string GetVolumeName( string filename )
		{
			string name = PWLib.Platform.Windows.Path.GetFileNameWithoutExtension( filename );
			return MainForm.Instance.TakeNameAndMakeItUnique( name );
		}


		private void mButtonBack_Click( object sender, EventArgs e )
		{
			MainForm.Instance.ControlSwitcher.SwitchUserControl( FormControlType.AddVolume2, FormControlSwitchType.Back );
		}

		private void mButtonNext_Click( object sender, EventArgs e )
		{
			try
			{
				VolumeDescriptorList.Instance.LoadDescriptor( GetVolumeName( mLocalFolderLabel.Text ), mLocalFolderLabel.Text );
				MainForm.Instance.ControlSwitcher.SwitchUserControl( FormControlType.Welcome, FormControlSwitchType.Finish );
			}
			catch ( Exception ex )
			{
				MessageBox.Show( this, "Error loading existing volume " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
		}


		public void OnControlHide( FormControlSwitchType switchType, object userObject )
		{
		}

		public void OnControlShow( FormControlSwitchType switchType, object userObject )
		{
			if (switchType == FormControlSwitchType.Next)
			{
				mObject = (AddNewVolumeControlObject)userObject;
				mButtonNext.Enabled = false;
				mLocalFolderLabel.Text = "";
			}
		}

		public Control GetControl() { return this; }

		bool DoesVolumeFilenameExist( string filename )
		{
			lock ( VolumeDescriptorList.Instance.Descriptors )
			{
				foreach ( VolumeDescriptor desc in VolumeDescriptorList.Instance.Descriptors )
				{
					if ( string.Compare( desc.VolumeFilename, filename, true ) == 0 )
						return true;
				}
			}
			return false;
		}


		private void mButtonBrowse_Click( object sender, EventArgs e )
		{
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.CheckFileExists = true;
			ofd.CheckPathExists = true;
			ofd.Filter = "Volume files|*.vol|All Files|*.*";
			ofd.Title = "Open volume file...";
			if ( ofd.ShowDialog() == DialogResult.OK )
			{
				// make sure volume file isn't already here
				string filename = ofd.FileName;
				if ( DoesVolumeFilenameExist( filename ) )
				{
					MessageBox.Show( this, "Volume filename '" + filename + "' already exists in volume list!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation );
					return;
				}

				mLocalFolderLabel.Text = filename;
				mButtonNext.Enabled = true;
			}
		}
	}
}

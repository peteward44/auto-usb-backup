using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace AutoUSBBackup.GUI
{
	public enum FormControlType
	{
		Welcome,
		AddVolume1, AddVolume2, AddVolume2a, AddVolume2b, AddVolume2c, AddVolume3, AddVolume4,
		RestoreVolume1, RestoreVolume2,
		TransferVolume1, TransferVolume2, TransferVolume3
	}


	public partial class UserControlSwitcher : UserControl
	{
		FormUserControl mDisplayedUserControl = null;
		Dictionary<FormControlType, FormUserControl> mUserControlList = new Dictionary<FormControlType, FormUserControl>();


		public UserControlSwitcher()
		{
			InitializeComponent();
		}



		public void BuildUserControlList()
		{
			mUserControlList.Add( FormControlType.Welcome, InitUserControl( new WelcomeControl(), "WelcomeControl" ) );
			mUserControlList.Add( FormControlType.AddVolume1, InitUserControl( new AddNewVolumeControl1(), "AddNewVolumeControl1" ) );
			mUserControlList.Add( FormControlType.AddVolume2, InitUserControl( new AddNewVolumeControl2(), "AddNewVolumeControl2" ) );
			mUserControlList.Add( FormControlType.AddVolume2a, InitUserControl( new AddNewVolumeControl2a(), "AddNewVolumeControl2a" ) );
			mUserControlList.Add( FormControlType.AddVolume2b, InitUserControl( new AddNewVolumeControl2b(), "AddNewVolumeControl2b" ) );
			mUserControlList.Add( FormControlType.AddVolume2c, InitUserControl( new AddNewVolumeControl2c(), "AddNewVolumeControl2c" ) );
			mUserControlList.Add( FormControlType.AddVolume3, InitUserControl( new AddNewVolumeControl3(), "AddNewVolumeControl3" ) );
			mUserControlList.Add( FormControlType.AddVolume4, InitUserControl( new AddNewVolumeControl4(), "AddNewVolumeControl4" ) );
			mUserControlList.Add( FormControlType.RestoreVolume1, InitUserControl( new RestoreVolumesControl1(), "RestoreVolumesControl1" ) );
			mUserControlList.Add( FormControlType.RestoreVolume2, InitUserControl( new RestoreVolumesControl2(), "RestoreVolumesControl2" ) );
			mUserControlList.Add( FormControlType.TransferVolume1, InitUserControl( new TransferVolumeControl1(), "TransferVolumeControl1" ) );
			mUserControlList.Add( FormControlType.TransferVolume2, InitUserControl( new TransferVolumeControl2(), "TransferVolumeControl2" ) );
			mUserControlList.Add( FormControlType.TransferVolume3, InitUserControl( new TransferVolumeControl3(), "TransferVolumeControl3" ) );
		}


		FormUserControl InitUserControl( FormUserControl formControl, string name )
		{
			Control userControl = formControl.GetControl();
			userControl.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
			userControl.Location = new System.Drawing.Point( 0, 0 );
			userControl.Size = new System.Drawing.Size( 494, 341 );
			userControl.Dock = DockStyle.Fill;
			userControl.Name = name;
			userControl.TabIndex = 0;
			userControl.Hide();
			this.Controls.Add( userControl );
			return formControl;
		}


		public void SwitchUserControl( FormControlType controlType, FormControlSwitchType switchType )
		{
			SwitchUserControl( controlType, switchType, null );
		}


		public void SwitchUserControl( FormControlType controlType, FormControlSwitchType switchType, object userObject )
		{
			if ( mDisplayedUserControl != null )
			{
				mDisplayedUserControl.OnControlHide( switchType, userObject );
				mDisplayedUserControl.GetControl().Hide();
			}

			if ( mUserControlList.ContainsKey( controlType ) )
				mDisplayedUserControl = mUserControlList[ controlType ];
			else
				mDisplayedUserControl = null;

			if ( mDisplayedUserControl != null )
			{
				mDisplayedUserControl.GetControl().Show();
				mDisplayedUserControl.OnControlShow( switchType, userObject );
			}
		}


		public FormUserControl GetUserControl( FormControlType controlType )
		{
			if ( mUserControlList.ContainsKey( controlType ) )
				return mUserControlList[ controlType ];
			else
				return null;
		}

	}
}

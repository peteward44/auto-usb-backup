using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace AutoUSBBackup.GUI
{
	public partial class AddNewVolumeControl1 : UserControl, FormUserControl
	{
		AddNewVolumeControlObject mObject;


		public AddNewVolumeControl1()
		{
			InitializeComponent();

			UpdateErrorLabelAndButton();
		}

		private void textBox1_TextChanged( object sender, EventArgs e )
		{
			UpdateErrorLabelAndButton();
		}


		void UpdateErrorLabelAndButton()
		{
			mErrorLabel.Text = "";
			mButtonNext.Enabled = false;

			string name = mTextBox.Text;
			if ( name.Length > 0 )
			{
				// check if name already exists
				if ( MainForm.Instance.VolumeNameExists( name ) )
				{
					mErrorLabel.Text = "Volume '" + name + "' already exists";
				}
				else
					mButtonNext.Enabled = true;
			}
		}

		private void mButtonNext_Click( object sender, EventArgs e )
		{
			mObject.Name = mTextBox.Text;
			MainForm.Instance.ControlSwitcher.SwitchUserControl( FormControlType.AddVolume3, FormControlSwitchType.Next, mObject );
		}

		private void mButtonBack_Click( object sender, EventArgs e )
		{
			MainForm.Instance.ControlSwitcher.SwitchUserControl( FormControlType.AddVolume2a, FormControlSwitchType.Back );
		}


		public void OnControlHide( FormControlSwitchType switchType, object userObject )
		{
		}

		public void OnControlShow( FormControlSwitchType switchType, object userObject )
		{
			if (switchType == FormControlSwitchType.Next)
			{
				mObject = (AddNewVolumeControlObject)userObject;
				mTextBox.Text = MainForm.Instance.TakeNameAndMakeItUnique( "New Volume" );
				UpdateErrorLabelAndButton();
			}
		}

		public Control GetControl() { return this; }


	}
}

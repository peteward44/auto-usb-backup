using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace AutoUSBBackup.GUI
{
	public enum FormControlSwitchType
	{
		Start,
		Back,
		Next,
		Finish
	}


	public interface FormUserControl
	{
		void OnControlHide( FormControlSwitchType switchType, object userObject );
		void OnControlShow( FormControlSwitchType switchType, object userObject );

		Control GetControl();
	}
}

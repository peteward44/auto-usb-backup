using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace AutoUSBBackup.GUI.Shared
{
	public partial class LogTextBox : UserControl
	{
		class LogItem
		{
			DateTime mDateTime;
			public string DateString { get { return mDateTime.ToString(); } }

			string mText;
			public string Text { get { return mText; } }

			Color mForeColor, mBackColor;
			public Color ForeColor { get { return mForeColor; } }
			public Color BackColor { get { return mBackColor; } }

			public LogItem( DateTime dt, string text, Color foreColour, Color backColour )
			{
				mDateTime = dt;
				mText = text;
				mForeColor = foreColour;
				mBackColor = backColour;
			}

		}


		List<LogItem> mLogItemList = new List<LogItem>();


		public LogTextBox()
		{
			InitializeComponent();

			this.mTextBox.FormatRow += new EventHandler<BrightIdeasSoftware.FormatRowEventArgs>(mTextBox_FormatRow);
		}


		void mTextBox_FormatRow( object sender, BrightIdeasSoftware.FormatRowEventArgs e )
		{
			LogItem logItem = ( LogItem )e.Model;
			if ( logItem != null )
			{
				e.Item.ForeColor = logItem.ForeColor;
				e.Item.BackColor = logItem.BackColor;
			}
		}


		public void WriteLine( DateTime dt, string line )
		{
			WriteLine( dt, line, Color.Black );
		}

		public void WriteLine( DateTime dt, string line, Color foreColour )
		{
			WriteLine( dt, line, foreColour, Color.Transparent );
		}

		delegate void AnonDelegate();

		public void WriteLine( DateTime dt, string line, Color foreColour, Color backColour )
		{
			mTextBox.Invoke( (AnonDelegate)delegate()
			{
				mLogItemList.Add( new LogItem( dt, line, foreColour, backColour ) );
				mTextBox.SetObjects( mLogItemList );
				mTextBox.EnsureVisible( mTextBox.Items.Count - 1 );
			}
			);
		}


		public void ClearText()
		{
			mTextBox.ClearObjects();
		}
	}
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;


namespace PWLib.Platform.Windows
{
	public class Misc
	{
		[StructLayout(LayoutKind.Sequential)]
		private struct RECT
		{
			public int left;
			public int top;
			public int right;
			public int bottom;
		}

		[DllImport("user32.dll")]
		private static extern bool GetWindowRect(HandleRef hWnd, [In, Out] ref RECT rect);

		[DllImport("user32.dll")]
		private static extern IntPtr GetForegroundWindow();


		public static bool IsForegroundFullScreen()
		{
			return IsForegroundFullScreen(null);
		}


		public static bool IsForegroundFullScreen(Screen screen)
		{
			if (screen == null)
			{
				screen = Screen.PrimaryScreen;
			}
			RECT rect = new RECT();
			GetWindowRect(new HandleRef(null, GetForegroundWindow()), ref rect);
			return new Rectangle(rect.left, rect.top, rect.right - rect.left, rect.bottom - rect.top).Contains(screen.Bounds);
		}


		public static string FormatSizeString( System.UInt64 sizeBytes )
		{
			string[] suffixes = new string[] { "B", "KB", "MB", "GB", "TB" };
			const ulong denominator = 1024;
			ulong comparator = denominator;
			ulong divider = 1;

			foreach ( string suffix in suffixes )
			{
				if ( sizeBytes < comparator )
					return ( (double)sizeBytes / (double)divider ).ToString( "F" ) + suffix;
				divider = comparator;
				comparator *= denominator;
			}

			return sizeBytes.ToString();
		}




		#region Fuzzy date formatting

		static bool IsSameDay( DateTime d1, DateTime d2 )
		{
			return d1.Day == d2.Day && d1.Month == d2.Month && d1.Year == d2.Year;
		}

		static bool IsSameWeek( DateTime d1, DateTime d2 )
		{
			TimeSpan ts = d2 - d1;
			return ts.Days < 7;
		}

		static bool IsSameMonth( DateTime d1, DateTime d2 )
		{
			return d1.Month == d2.Month && d1.Year == d2.Year;
		}


		static int GetDaysSinceMonthBefore( DateTime dt, int numberOfMonthsBefore )
		{
			int year = dt.Year;
			int month = dt.Month;
			int totalDays = 0;

			for ( int monthIndex = 0; monthIndex < numberOfMonthsBefore; ++monthIndex )
			{
				month--;
				if ( month <= 0 )
				{
					month = 12;
					year--;
				}
				totalDays += DateTime.DaysInMonth( year, month );
			}

			return totalDays;
		}


		public static string FormatFuzzyDateString( DateTime dt )
		{
			if ( dt.Ticks == 0 )
				return "Never";

			DateTime now = DateTime.Now;// +( new TimeSpan( 340, 0, 0, 0 ) );
			for ( int i = 0; i < 7; ++i )
			{
				DateTime dayToTest = now - ( new TimeSpan( i, 0, 0, 0 ) );
				if ( IsSameDay( dt, dayToTest ) )
				{
					switch ( i )
					{
						case 0:
							return "Today at " + dt.ToShortTimeString();
						case 1:
							return "Yesterday";
						default:
							return i + " days ago";
					}
				}
			}

			for ( int i = 0; i < 3; ++i )
			{
				DateTime weekToTest = now - ( new TimeSpan( ( int )now.DayOfWeek, 0, 0, 0 ) ) - ( new TimeSpan( i * 7, 0, 0, 0 ) );
				if ( IsSameWeek( dt, weekToTest ) )
				{
					switch ( i )
					{
						case 0:
							return "Last week";
						default:
							return ( i + 1 ) + " weeks ago";
					}
				}
			}

			for ( int i = 0; i < 11; ++i )
			{
				DateTime startOfMonth = now - ( new TimeSpan( now.Day - 1, 0, 0, 0 ) );
				DateTime toTest = ( now - ( new TimeSpan( now.Day - 1, 0, 0, 0 ) ) ) - ( new TimeSpan( GetDaysSinceMonthBefore( startOfMonth, i + 1 ), 0, 0, 0 ) );
				if ( IsSameMonth( dt, toTest ) )
				{
					switch ( i )
					{
						case 0:
							return "Last month";
						default:
							return ( i + 1 ) + " months ago";
					}
				}
			}

			return "Over a year ago";
		}

		#endregion
	}
}

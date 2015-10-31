using System;
using System.Collections.Generic;
using System.Text;
using PWLib.FileSyncLib;
using System.Xml;


namespace AutoUSBBackup
{
	public class VolumeDescriptor
	{
		ulong mTotalDatabaseSize = 0;

		public ulong TotalDatabaseSize { get { return mTotalDatabaseSize; } set { mTotalDatabaseSize = value; } }
		public string TotalDatabaseSizeString { get { return PWLib.Platform.Windows.Misc.FormatSizeString( mTotalDatabaseSize ); } }


		bool mIsAvailable = false;
		int mRevisionsToKeep = 1;
		TimeSpan mTimePeriodToMonitor;
		DateTime mLastAttemptedBackup; // last time a backup was checked for, not necessarily the last time a snapshot was created
		string mFuzzyLastAttemptedBackupString;

		VolumeEventController mEventController;
		string mVolumeFilename = "", mName = "";

		public VolumeEventController EventController { get { return mEventController; } }
		public string VolumeFilename { get { return mVolumeFilename; } }
		public string VolumeName { get { return mName; } }

		public bool IsAvailable { get { return mIsAvailable; } }

		public int RevisionsToKeep { get { return mRevisionsToKeep; } set { mRevisionsToKeep = value; } }
		public TimeSpan TimePeriodToMonitor { get { return mTimePeriodToMonitor; } set { mTimePeriodToMonitor = value; mRevisionsToKeep = -1; } }

		public DateTime LastAttemptedBackup
		{
			get { return mLastAttemptedBackup; }
			set { mLastAttemptedBackup = value; mFuzzyLastAttemptedBackupString = PWLib.Platform.Windows.Misc.FormatFuzzyDateString( mLastAttemptedBackup ); }
		}
		public string LastAttemptedBackupFuzzyString { get { return mFuzzyLastAttemptedBackupString; } }


		Volume mVolume = null;
		public Volume Volume { get { return mVolume; } }

		public string StateString
		{
			get
			{
				if ( mVolume != null && IsAvailable )
				{
					if ( mVolume.Source.MediaAvailable )
						return mVolume.Busy ? "Busy" : "Monitoring";
					else
						return "Inactive";
				}
				else
					return "Vol file missing";
			}
		}


		public bool ReadyForBackup
		{
			get
			{
				return mVolume != null && IsAvailable && mVolume.Source.MediaAvailable && mVolume.Archive.IsAvailable && !mVolume.Busy;
			}
		}


		public bool ReadyForRestore
		{
			get
			{
				return mVolume != null && IsAvailable && mVolume.Archive.IsAvailable && !mVolume.Busy;
			}
		}



		void mVolume_OutputToXmlRequired( Volume volume )
		{
			SaveVolumeData();
		}


		void ConnectToVolumeEvents( Volume volume )
		{
			volume.OutputToXmlRequired += new AutoUSBBackup.Volume.OutputToXmlRequiredDelegate( mVolume_OutputToXmlRequired );
		}


		Volume CreateVolume()
		{
			if ( PWLib.Platform.Windows.File.Exists( mVolumeFilename ) )
			{
				XmlDocument xmlDoc = new XmlDocument();
				xmlDoc.Load( mVolumeFilename );
				XmlNode mainNode = PWLib.XmlHelp.GetFirstChildWithName( xmlDoc, "Volume" );

				mName = PWLib.XmlHelp.DirtyString( PWLib.XmlHelp.GetAttribute( mainNode, "name", "" ) );
				mRevisionsToKeep = PWLib.XmlHelp.GetAttributeInt( mainNode, "revisionsToKeep", 1 );
				long timePeriodTicks = 0;
				long.TryParse( PWLib.XmlHelp.GetAttribute( mainNode, "timePeriodToMonitor", "" ), out timePeriodTicks );
				mTimePeriodToMonitor = TimeSpan.FromTicks( timePeriodTicks );

				long lastAttemptedBackupTicks = 0;
				long.TryParse( PWLib.XmlHelp.GetAttribute( mainNode, "lastattemptedbackup", "" ), out lastAttemptedBackupTicks );
				LastAttemptedBackup = new DateTime( lastAttemptedBackupTicks );

				mTotalDatabaseSize = 0;
				ulong.TryParse( PWLib.XmlHelp.GetAttribute( mainNode, "totaldatabasesize", "" ), out mTotalDatabaseSize );

				Volume volume = new Volume( mEventController, this, mainNode );
				ConnectToVolumeEvents( volume );
				mIsAvailable = true;
				return volume;
			}
			else
				return null;
		}


		public VolumeDescriptor( VolumeEventController eventController, string volName, string volFilename )
		{
			mEventController = eventController;
			mName = volName;
			mVolumeFilename = volFilename;

			mVolume = CreateVolume();
		}


		public VolumeDescriptor( VolumeEventController eventController, string volName, string volFilename, VolumeSource source, BaseArchive archive )
		{
			mEventController = eventController;
			mName = volName;
			mVolumeFilename = volFilename;

			mVolume = new Volume( mEventController, this, source, archive );
			ConnectToVolumeEvents( mVolume );
			mIsAvailable = true;
			SaveVolumeData();
		}



		public void SaveVolumeData()
		{
			//try
			//{
				if ( mVolume != null )
				{
					string dirPath = PWLib.Platform.Windows.Path.GetStemName( mVolumeFilename );
					if ( !PWLib.Platform.Windows.Directory.Exists( dirPath ) )
						PWLib.Platform.Windows.Directory.CreateDirectory( dirPath );
					XmlTextWriter xmlWriter = new XmlTextWriter( mVolumeFilename, Encoding.Unicode );
					xmlWriter.Formatting = Formatting.Indented;
					xmlWriter.WriteStartDocument();
					xmlWriter.WriteStartElement( "Volume" );

					xmlWriter.WriteAttributeString( "name", PWLib.XmlHelp.CleanString( mName ) );
					xmlWriter.WriteAttributeString( "revisionsToKeep", mRevisionsToKeep.ToString() );
					xmlWriter.WriteAttributeString( "timePeriodToMonitor", mTimePeriodToMonitor.Ticks.ToString() );
					xmlWriter.WriteAttributeString( "lastattemptedbackup", mLastAttemptedBackup.Ticks.ToString() );
					xmlWriter.WriteAttributeString( "totaldatabasesize", mTotalDatabaseSize.ToString() );

					mVolume.OutputToXml( xmlWriter );

					xmlWriter.WriteEndElement();
					xmlWriter.Close();
				}
			//}
			//catch ( System.Exception e )
			//{
			//}
		}


		public static VolumeDescriptor LoadFromXml( VolumeEventController eventController, string volName, string volFilename )
		{
			return new VolumeDescriptor( eventController, volName, volFilename );
		}



		public void CheckAvailability()
		{
			bool fileExists = PWLib.Platform.Windows.File.Exists( mVolumeFilename );
			if ( fileExists && !mIsAvailable )
			{
				if ( mVolume != null )
					mVolume.Dispose();
				mVolume = CreateVolume();
				mIsAvailable = true;
				mEventController.InvokeVolumeDescriptorActiveStateChanged( this, true );
			}
			else if ( !fileExists && mIsAvailable )
			{
				mIsAvailable = false;
				mEventController.InvokeVolumeDescriptorActiveStateChanged( this, false );
			}
		}


		public override bool Equals( object obj )
		{
			if ( obj is VolumeDescriptor )
			{
				VolumeDescriptor vd = (VolumeDescriptor)obj;
				return string.Compare( vd.mName, this.mName, false ) == 0;
			}
			else
				return false;
		}

		public override int GetHashCode()
		{
			return mName.GetHashCode();
		}
	}
}



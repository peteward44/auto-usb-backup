using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;


namespace AutoUSBBackup
{

	public class Config
	{
#if DEBUG
        static readonly string ConfigXmlFile = @"c:\autousb\config.xml";
#else
        static readonly string ConfigXmlFile = System.Windows.Forms.Application.CommonAppDataPath + PWLib.Platform.Windows.Path.DirectorySeparatorChar + "config.xml";
#endif

		static Config sActive = null;

		public static Config Active { get { return sActive; } }

		public static void CreateInstance()
		{
			System.Diagnostics.Debug.Assert( sActive == null );
			sActive = new Config();
			sActive.Load();
		}


		bool SaveConfigFile( string filePath )
		{
			try
			{
				string dirPath = PWLib.Platform.Windows.Path.GetStemName( filePath );
				if ( !PWLib.Platform.Windows.Directory.Exists( dirPath ) )
					PWLib.Platform.Windows.Directory.CreateDirectory( dirPath );
				XmlTextWriter configWriter = new XmlTextWriter( filePath, Encoding.Unicode );
				configWriter.Formatting = Formatting.Indented;
				configWriter.WriteStartDocument();
				configWriter.WriteStartElement( "AutoUSBBackup" );
				configWriter.WriteStartElement( "config" );

				configWriter.WriteElementString( "rootdatadirectory", mDefaultRootDataDirectory );
				configWriter.WriteElementString( "workerthreadsleeptime", mWorkerThreadSleepTime.ToString() );
				configWriter.WriteElementString( "compressbackups", mCompressBackups.ToString() );

				configWriter.WriteStartElement( "volumes" );
				lock ( VolumeDescriptorList.Instance.Descriptors )
				{
					foreach ( VolumeDescriptor cvid in VolumeDescriptorList.Instance.Descriptors )
					{
						string volumeFilename = cvid.VolumeFilename;
						configWriter.WriteStartElement( "volume" );
						configWriter.WriteAttributeString( "name", cvid.VolumeName );
						configWriter.WriteString( volumeFilename );
						configWriter.WriteEndElement();
					}
				}
				configWriter.WriteEndElement();

				configWriter.WriteEndElement();
				configWriter.WriteFullEndElement();
				configWriter.Close();
			}
			catch ( System.Exception e )
			{
				Log.WriteException( "SaveConfigFile failed", e );
				return false;
			}

			Log.WriteLine( LogType.TextLog, "Config file saved, describing " + VolumeDescriptorList.Instance.Descriptors.Count + " volumes" );

			return true;
		}


		public void Save()
		{
			// had some problems with the config file wiping itself, to fix this we output the new config.xml to another location
			// and copy it back over the old one if it succeeded
			bool succeeded = false;
			int attempts = 0;
			const int maxAttempts = 10;
			string tempConfigFilename = System.IO.Path.GetTempFileName();
			do
			{
				succeeded = SaveConfigFile( tempConfigFilename );
				if ( !succeeded )
					System.Threading.Thread.Sleep( 300 );
				else
				{
					string dirPath = PWLib.Platform.Windows.Path.GetStemName( ConfigXmlFile );
					if ( !PWLib.Platform.Windows.Directory.Exists( dirPath ) )
						PWLib.Platform.Windows.Directory.CreateDirectory( dirPath );
					System.IO.File.Copy( tempConfigFilename, ConfigXmlFile, true );
					System.IO.File.Delete( tempConfigFilename );
				}
				attempts++;
			} while ( !succeeded && attempts < maxAttempts );
		}


		List<KeyValuePair<string, string>> LoadConfigFile()
		{
			XmlTextReader configReader = null;
			List<KeyValuePair<string, string>> loadedDescriptorList = new List<KeyValuePair<string, string>>();

			try
			{
				System.IO.FileStream fs = new System.IO.FileStream( ConfigXmlFile, System.IO.FileMode.Open, System.IO.FileAccess.Read );
				configReader = new XmlTextReader( fs );

				if ( !configReader.ReadToFollowing( "config" ) )
					throw new Exception( "config element not found" );

				mDefaultRootDataDirectory = PWLib.XmlHelp.ReadElementString( configReader, "rootdatadirectory", mDefaultRootDataDirectory );
				int.TryParse( PWLib.XmlHelp.ReadElementString( configReader, "workerthreadsleeptime", mWorkerThreadSleepTime.ToString() ), out mWorkerThreadSleepTime );
				bool.TryParse( PWLib.XmlHelp.ReadElementString( configReader, "compressbackups", mCompressBackups.ToString() ), out mCompressBackups );

				if ( !configReader.ReadToFollowing( "volumes" ) )
					return loadedDescriptorList;
				if ( !configReader.ReadToDescendant( "volume" ) )
					return loadedDescriptorList;

				do
				{
					string volFilename = "";
					string volName = "";

					configReader.MoveToFirstAttribute();
					do
					{
						if ( configReader.Name.ToLower() == "name" )
						{
							volName = configReader.Value;
							break;
						}
					} while ( configReader.MoveToNextAttribute() );

					XmlNodeType nodeType = configReader.MoveToContent();

					if ( nodeType == XmlNodeType.Element )
						volFilename = configReader.ReadString();

					if ( volName.Length > 0 && volFilename.Length > 0 )
						loadedDescriptorList.Add( new KeyValuePair<string, string>( volName, volFilename ) );

				} while ( configReader.ReadToNextSibling( "volume" ) );
			}
			finally
			{
				if ( configReader != null )
					configReader.Close();
			}
			return loadedDescriptorList;
		}


		public void Load()
		{
			try
			{
				List<KeyValuePair<string, string>> descriptorList = LoadConfigFile();

				VolumeDescriptorList.Instance.Clear();

				foreach ( KeyValuePair<string, string> pair in descriptorList )
				{
					VolumeDescriptorList.Instance.LoadDescriptor( pair.Key, pair.Value );
				}
			}
			catch ( Exception e )
			{
				Log.WriteException( "Load config file failed", e, false );
			}
		}


		public Config()
		{
#if DEBUG
			mDefaultRootDataDirectory = @"c:\autousb\data";
#else
			mDefaultRootDataDirectory = System.Windows.Forms.Application.CommonAppDataPath + PWLib.Platform.Windows.Path.DirectorySeparatorChar + "data";
#endif
			mLogFileDirectory = System.Windows.Forms.Application.CommonAppDataPath;
		}

		bool mCompressBackups = false;
		public bool CompressBackups { get { return mCompressBackups; } set { mCompressBackups = value; } }

		TimeSpan mBackupInterval = new TimeSpan( 0, 1, 0, 0, 0 ); // Default to every 1 hour
		public TimeSpan BackupInterval { get { return mBackupInterval; } }

		int mWorkerThreadSleepTime = 300;
		public int WorkerThreadSleepTime { get { return mWorkerThreadSleepTime; } }

		bool mStartMinimised = true;
		public bool StartMinimised { get { return mStartMinimised; } }


		#region Directories


		string mLogFileDirectory;
		public string LogFileDirectory
		{
			get
			{
				if ( !PWLib.Platform.Windows.Directory.Exists( mLogFileDirectory ) )
					PWLib.Platform.Windows.Directory.CreateDirectory( mLogFileDirectory );
				return mLogFileDirectory;
			}
		}


		string mDefaultRootDataDirectory;

		public string DefaultRootDataDirectory
		{
			get
			{
				if ( !PWLib.Platform.Windows.Directory.Exists( mDefaultRootDataDirectory ) )
					PWLib.Platform.Windows.Directory.CreateDirectory( mDefaultRootDataDirectory );
				return mDefaultRootDataDirectory;
			}
		}


		#endregion


	}
}

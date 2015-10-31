using System;
using System.Collections.Generic;
using System.Text;
using PWLib.FileSyncLib;


namespace AutoUSBBackup
{
	public class VolumeDescriptorList
	{
		VolumeEventController mEventController;
		List<VolumeDescriptor> mDescriptorList = new List<VolumeDescriptor>();
		public List<VolumeDescriptor> Descriptors { get { return mDescriptorList; } }

		static VolumeDescriptorList mInstance = null;

		public static void CreateInstance( VolumeEventController eventController )
		{
			if ( mInstance == null )
				mInstance = new VolumeDescriptorList( eventController );
		}

		public static VolumeDescriptorList Instance
		{
			get
			{
				if ( mInstance == null )
					throw new Exception( "VolumeDescriptorList not created" );
				return mInstance;
			}
		}


		public bool AnyBusyVolumes
		{
			get
			{
				lock ( mDescriptorList )
				{
					foreach ( VolumeDescriptor desc in mDescriptorList )
					{
						if ( desc.IsAvailable && desc.Volume.Busy )
							return true;
					}
				}
				return false;
			}
		}


		public VolumeDescriptorList( VolumeEventController eventController )
		{
			mEventController = eventController;
		}


		public void Clear()
		{
			lock ( mDescriptorList )
			{
				foreach ( VolumeDescriptor descriptor in mDescriptorList )
				{
					if ( descriptor.IsAvailable )
						descriptor.Volume.Dispose();
				}
				mDescriptorList.Clear();
			}
		}


		public VolumeDescriptor AddNewDescriptor( string volName, string volFilename, VolumeSource source, BaseArchive archive )
		{
			VolumeDescriptor vd = new VolumeDescriptor( mEventController, volName, volFilename, source, archive );
			lock ( mDescriptorList )
			{
				mDescriptorList.Add( vd );
			}
			Config.Active.Save();
			return vd;
		}

		public VolumeDescriptor LoadDescriptor( string volName, string volFilename )
		{
			VolumeDescriptor vd = VolumeDescriptor.LoadFromXml( mEventController, volName, volFilename );
			lock ( mDescriptorList )
			{
				mDescriptorList.Add( vd );
			}
			return vd;
		}


		public void RemoveDescriptor( VolumeDescriptor vd )
		{
			lock ( mDescriptorList )
			{
				if ( mDescriptorList.Contains( vd ) )
				{
					mDescriptorList.Remove( vd );
				}
			}
		}


		public void CheckAvailability()
		{
			List<VolumeDescriptor> copyList;
			lock ( mDescriptorList )
			{
				copyList = new List<VolumeDescriptor>( mDescriptorList );
			}
			foreach ( VolumeDescriptor descriptor in copyList )
			{
				descriptor.CheckAvailability();
			}
		}


		public void Dispose()
		{
			Clear();
		}


		public bool VolumeNameExists( string name )
		{
			lock ( mDescriptorList )
			{
				foreach ( VolumeDescriptor descriptor in mDescriptorList )
				{
					if ( string.Compare( descriptor.VolumeName, name, true ) == 0 )
						return true;
				}
			}
			return false;
		}

	}
}



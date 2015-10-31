using System;
using System.Collections.Generic;
using System.Text;


namespace PWLib.FileSyncLib
{
	public class VolumeSnapshotRevision : IComparable< VolumeSnapshotRevision >
	{
		long mId;


		public long Value { get { return mId; } }
		public DateTime CreationTime { get { return new DateTime( mId ); } }


		public override bool Equals( object obj )
		{
			if ( obj is VolumeSnapshotRevision )
			{
				VolumeSnapshotRevision vsr = (VolumeSnapshotRevision)obj;
				return mId == vsr.mId;
			}
			else
				return false;
		}

		public override int GetHashCode()
		{
			return mId.GetHashCode();
		}

		public override string ToString()
		{
			return mId.ToString();
		}

		public int CompareTo( object x )
		{
			if ( x is VolumeSnapshotRevision )
			{
				VolumeSnapshotRevision rhs = (VolumeSnapshotRevision)x;
				long diff = mId - rhs.mId;
				if ( diff > 0 )
					return 1;
				else if ( diff < 0 )
					return -1;
				else
					return 0;
			}
			else
				return 0;
		}

		int IComparable<VolumeSnapshotRevision>.CompareTo( VolumeSnapshotRevision rhs )
		{
			long diff = mId - rhs.mId;
			if ( diff > 0 )
				return 1;
			else if ( diff < 0 )
				return -1;
			else
				return 0;
		}



		private VolumeSnapshotRevision( long id )
		{
			mId = id;
		}


		public static VolumeSnapshotRevision CreateNew()
		{
			DateTime dt = DateTime.Now;
			return Create( dt.Ticks );
		}

		public static VolumeSnapshotRevision Create( long ticks )
		{
			return new VolumeSnapshotRevision( ticks );
		}

		public static VolumeSnapshotRevision Create( string tickString )
		{
			long ticks;
			if ( long.TryParse( tickString, out ticks ) )
				return Create( ticks );
			else
				return null;
		}
	}
}

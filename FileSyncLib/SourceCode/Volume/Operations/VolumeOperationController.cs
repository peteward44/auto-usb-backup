using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;


namespace PWLib.FileSyncLib
{

	internal class VolumeOperationController
	{
		List<VolumeOperationController> mChained = new List<VolumeOperationController>();
		volatile bool mCancelled = false;
		volatile bool mCancelledByUser = false;

		public bool Cancelled { get { return mCancelled || CheckChainedCancel(); } }
		public bool CancelledByUser { get { return mCancelledByUser || CheckChainedCancelByUser(); } }
		public bool CanContinue { get { return !Cancelled; } }


		public void Reset()
		{
			mCancelled = false;
			mCancelledByUser = false;
		}


		public void Cancel( bool userCancelled )
		{
			mCancelledByUser = userCancelled;
			mCancelled = true;
		}


		bool CheckChainedCancel()
		{
			foreach ( VolumeOperationController voc in mChained )
			{
				if ( voc.Cancelled )
					return true;
			}
			return false;
		}


		bool CheckChainedCancelByUser()
		{
			foreach ( VolumeOperationController voc in mChained )
			{
				if ( voc.CancelledByUser )
					return true;
			}
			return false;
		}


		// Any 'chained' VOCs when the cancel, this will also cancel
		public void Chain( VolumeOperationController otherOpController )
		{
			mChained.Add( otherOpController );
		}
	}
}



using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

// Base class for object model class for the ObjectListView
// library. This class automatically checks all children objects
// underneath the checked item, useful for file views


namespace AutoUSBBackup.GUI.Shared
{
	public abstract class CheckedModelItem : IComparable
	{
		CheckedModelItem mParent = null;
		bool mIsChecked = false;
		bool mIsCheckedViaInheritance = false;

		public CheckedModelItem Parent { get { return mParent; } }
		public bool IsChecked { get { return mIsChecked; } set { mIsChecked = value; } }

		public bool IsCheckedViaInheritance
		{
			get
			{
				return mIsCheckedViaInheritance;
			}
		}

		public CheckedModelItem( CheckedModelItem parent )
		{
			ResetParent( parent );
		}


		public void ResetParent(CheckedModelItem parent)
		{
			mIsChecked = false;
			mParent = parent;
			UpdateCheckedByInheritance();
		}


		public void UpdateCheckedByInheritance()
		{
			mIsCheckedViaInheritance = false;
			CheckedModelItem cmi = mParent;
			while (!mIsCheckedViaInheritance && cmi != null)
			{
				if (cmi.IsChecked)
				{
					mIsCheckedViaInheritance = true;
				}
				cmi = cmi.mParent;
			}
		}


		public abstract int CompareTo( object obj );

	}


	public class CheckedTreeViewHelper
	{
		BrightIdeasSoftware.TreeListView mTreeView;
		
		bool mAllowMultipleCheckedItems = true;
		public bool AllowMultipleCheckedItems { get { return mAllowMultipleCheckedItems; } set { mAllowMultipleCheckedItems = value; } }

		public delegate void CheckedItemDelegate( CheckedTreeViewHelper sender, CheckedModelItem checkedObject );

		public event CheckedItemDelegate SelectedObjectAdded;
		public event CheckedItemDelegate SelectedObjectRemoved;

		public delegate CheckState CheckedItemFilterDelegate( CheckedModelItem checkedObject, CheckState checkState );

		public CheckedItemFilterDelegate GetterFilter = null, SetterFilter = null;


		public CheckedTreeViewHelper( BrightIdeasSoftware.TreeListView treeView )
		{
			mTreeView = treeView;
			mTreeView.CheckStateGetter = new BrightIdeasSoftware.CheckStateGetterDelegate( CheckStateGetterMethod );
			mTreeView.CheckStatePutter = new BrightIdeasSoftware.CheckStatePutterDelegate( CheckStateSetterMethod );
		}


		CheckState CheckStateGetterMethod( object x )
		{
			System.Diagnostics.Debug.Assert( x is CheckedModelItem );

			CheckedModelItem o = (CheckedModelItem)x;
			CheckState cs;
			if ( o.IsChecked )
				cs = CheckState.Checked;
			else if ( o.IsCheckedViaInheritance )
				cs = CheckState.Indeterminate;
			else
				cs = CheckState.Unchecked;
			if ( GetterFilter != null )
				return GetterFilter( o, cs );
			else
				return cs;
		}


		CheckState CheckStateSetterMethod( object x, CheckState checkState )
		{
			System.Diagnostics.Debug.Assert( x is CheckedModelItem );

			bool check = checkState == CheckState.Checked;
			CheckState ret = CheckState.Unchecked;
			CheckedModelItem o = (CheckedModelItem)x;
			if ( !o.IsCheckedViaInheritance )
			{
				if ( check && !mAllowMultipleCheckedItems )
				{
					foreach ( CheckedModelItem lvo in mTreeView.CheckedObjects )
					{
						lvo.IsChecked = false;
						RefreshObjectAndChildrenRecurse( lvo );
					}
				}
				o.IsChecked = check;
				ret = check ? CheckState.Checked : CheckState.Unchecked;
				if ( check )
				{
					if ( SelectedObjectAdded != null )
						SelectedObjectAdded( this, o );
				}
				else
				{
					if ( SelectedObjectRemoved != null )
						SelectedObjectRemoved( this, o );
				}
			}
			else
			{
				ret = CheckState.Indeterminate;
			}
			RefreshObjectAndChildrenRecurse( o );
			return SetterFilter != null ? SetterFilter( o, ret ) : ret;
		}


		void RefreshObjectAndChildrenRecurse( CheckedModelItem lvo )
		{
			foreach ( CheckedModelItem child in mTreeView.GetChildren( lvo ) )
			{
				child.ResetParent( lvo );
				RefreshObjectAndChildrenRecurse( child );
			}
			mTreeView.RefreshObject( lvo );
		}
	}
}

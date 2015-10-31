using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;


namespace AutoUSBBackup.GUI.Shared
{
	public interface IFileImageModelObject
	{
		FileImageListViewHelper.IconUsage IconUsage { get; }
		int UserSpecifiedImageListIndex { get; }
		string FileExtension { get; }
	}


	public class FileImageListViewHelper
	{
		public enum IconUsage
		{
			UserSpecified,
			ByFileExtension,
			FolderOpen,
			FolderClosed,
			SystemDrive,
		}


		static Dictionary<string, Icon> sIconCache = new Dictionary<string, Icon>(); // file extensions icon cache
		Dictionary<string, int> mIndexMap = new Dictionary<string, int>(); // file extension mapping to index into image list

		BrightIdeasSoftware.OLVColumn mListViewColumn;
		ImageList mImageList;
		int mClosedFolderIndex = -1, mOpenFolderIndex = -1, mSystemDriveIndex = -1;

		
		public int AddUserSpecifiedImage( Image image )
		{
			mImageList.Images.Add( image );
			return mImageList.Images.Count - 1;
		}


		public FileImageListViewHelper( BrightIdeasSoftware.OLVColumn olv, ImageList imageList )
		{
			mListViewColumn = olv;
			mImageList = imageList;

            mListViewColumn.ImageGetter = new BrightIdeasSoftware.ImageGetterDelegate( ImageGetterMethod );

			mImageList.ColorDepth = ColorDepth.Depth32Bit;

            // Add folder icons and system drive icon
			Icon closedFolderIcon = PWLib.Platform.Windows.IconHelper.GetFolderIcon( PWLib.Platform.Windows.IconHelper.SystemIconSize.Small, PWLib.Platform.Windows.IconHelper.FolderType.Closed );
            if (closedFolderIcon != null)
            {
                mImageList.Images.Add(closedFolderIcon);
                mClosedFolderIndex = mImageList.Images.Count - 1;
            }

			Icon openFolderIcon = PWLib.Platform.Windows.IconHelper.GetFolderIcon( PWLib.Platform.Windows.IconHelper.SystemIconSize.Small, PWLib.Platform.Windows.IconHelper.FolderType.Open );
            if (openFolderIcon != null)
            {
                mImageList.Images.Add(openFolderIcon);
                mOpenFolderIndex = mImageList.Images.Count - 1;
            }

            // Try getting hard disk icon, if it doesn't exist fall back on the closed folder icon
            Icon deviceIcon = PWLib.Platform.Windows.IconHelper.GetFileIcon(System.IO.Path.GetPathRoot(Application.ExecutablePath), PWLib.Platform.Windows.IconHelper.SystemIconSize.Small, false);
            if (deviceIcon != null)
            {
                mImageList.Images.Add(deviceIcon);
                mSystemDriveIndex = mImageList.Images.Count - 1;
            }
            else
            {
                mSystemDriveIndex = mClosedFolderIndex;
            }
		}


		object ImageGetterMethod( object x )
		{
            if (x is IFileImageModelObject)
            {
                IFileImageModelObject br = (IFileImageModelObject)x;

                switch (br.IconUsage)
                {
                    case IconUsage.ByFileExtension:
                        {
                            // load system default icon for file type extension
                            string extension = br.FileExtension.ToLower();
                            int index = -1;
                            if (extension.Length > 0)
                            {
                                // already loaded into this list view's image map?
                                if (mIndexMap.ContainsKey(extension))
                                    index = mIndexMap[extension];
                                else
                                {
                                    // check application-wide icon cache for icon
                                    lock (sIconCache)
                                    {
                                        Icon icon = null;
                                        if (!sIconCache.ContainsKey(extension))
                                        {
											icon = PWLib.Platform.Windows.IconHelper.IconFromExtensionShell( extension, PWLib.Platform.Windows.IconHelper.SystemIconSize.Small );
                                            if ( icon != null )
                                                sIconCache.Add(extension, icon);
                                        }
                                        else
                                            icon = sIconCache[extension];

                                        if (icon != null)
                                        {
                                            mImageList.Images.Add(icon);
                                            index = mImageList.Images.Count - 1;
                                            mIndexMap.Add(extension, index);
                                        }
                                    }
                                }
                            }
                            return index;
                        }

                    case IconUsage.UserSpecified:
                        return br.UserSpecifiedImageListIndex;

                    case IconUsage.FolderClosed:
                        return mClosedFolderIndex;

                    case IconUsage.FolderOpen:
                        return mOpenFolderIndex;

                    case IconUsage.SystemDrive:
                        return mSystemDriveIndex;
                }
            }

            return -1;
		}
	}
}

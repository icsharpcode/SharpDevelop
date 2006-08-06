/*
 * Created by SharpDevelop.
 * User: Daniel Grunwald
 * Date: 18.07.2006
 * Time: 15:53
 */

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ICSharpCode.Core;

namespace LineCounterAddin
{
	// Helper class for improvement 1
	
	/// <summary>
	/// To minimize changes to LineCounterBrowser, we are
	/// using this helper class to load images from SharpDevelop's
	/// IconService into our image list on demand.
	/// </summary>
	public class ImageListHelper
	{
		ImageList imageList;
		Dictionary<string, int> dict = new Dictionary<string, int>();
		
		public ImageListHelper(ImageList imageList)
		{
			if (imageList == null)
				throw new ArgumentNullException("imageList");
			this.imageList = imageList;
		}
		
		public int GetIndex(string imageName)
		{
			int index;
			if (!dict.TryGetValue(imageName, out index)) {
				index = imageList.Images.Count;
				imageList.Images.Add(IconService.GetBitmap(imageName));
				dict[imageName] = index;
			}
			return index;
		}
	}
}

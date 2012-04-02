// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Media.Imaging;

namespace ICSharpCode.UnitTesting
{
	static class Images
	{
		static BitmapImage LoadBitmap(string name)
		{
			BitmapImage image = new BitmapImage(new Uri("pack://application:,,,/UnitTesting;component/Resources/" + name + ".png"));
			image.Freeze();
			return image;
		}
		
		public static readonly BitmapImage Grey = LoadBitmap("Grey");
		public static readonly BitmapImage Green = LoadBitmap("Green");
		public static readonly BitmapImage Red = LoadBitmap("Red");
		public static readonly BitmapImage Yellow = LoadBitmap("Yellow");
	}
}

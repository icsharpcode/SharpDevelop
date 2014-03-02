// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Drawing;
using System.Windows.Media;

namespace ICSharpCode.SharpDevelop.Templates
{
	public static class TemplateIconLoader
	{
		public static IImage GetImage(string iconName)
		{
			IImage image = GetFileImage(iconName);
			if (image != null) {
				return image;
			}
			return SD.ResourceService.GetImage(iconName);
		}
		
		static IImage GetFileImage(string iconName)
		{
			if (FileIconService.IsFileImage(iconName)) {
				return new FileImage(iconName);
			}
			return null;
		}
	}
	
	public class FileImage : IImage
	{
		string name;
		
		public FileImage(string name)
		{
			this.name = name;
		}
		
		public ImageSource ImageSource {
			get { return null; }
		}
		
		public Bitmap Bitmap {
			get { return FileIconService.GetBitmap(name); }
		}
		
		public Icon Icon {
			get { return null; }
		}
	}
}

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
using System.Windows.Media;
using ICSharpCode.SharpDevelop;

namespace ResourceEditor.Views
{
	/// <summary>
	/// Provides resource icons for different <see cref="ResourceEditor.ViewModels.ResourceItem"/> types.
	/// </summary>
	public class ResourceItemIcons
	{
		public ResourceItemIcons()
		{
			UnknownResourceIcon = SD.ResourceService.GetImageSource("Icons.16x16.ResourceEditor.obj");
			StringResourceIcon = SD.ResourceService.GetImageSource("Icons.16x16.ResourceEditor.string");
			BooleanResourceIcon = SD.ResourceService.GetImageSource("Icons.16x16.ResourceEditor.obj");
			BitmapResourceIcon = SD.ResourceService.GetImageSource("Icons.16x16.ResourceEditor.bmp");
			IconResourceIcon = SD.ResourceService.GetImageSource("Icons.16x16.ResourceEditor.icon");
			CursorResourceIcon = SD.ResourceService.GetImageSource("Icons.16x16.ResourceEditor.cursor");
			BinaryResourceIcon = SD.ResourceService.GetImageSource("Icons.16x16.ResourceEditor.bin");
		}
		
		public ImageSource UnknownResourceIcon {
			get;
			private set;
		}
		
		public ImageSource StringResourceIcon {
			get;
			private set;
		}
		
		public ImageSource BooleanResourceIcon {
			get;
			private set;
		}
		
		public ImageSource BitmapResourceIcon {
			get;
			private set;
		}
		
		public ImageSource IconResourceIcon {
			get;
			private set;
		}
		
		public ImageSource CursorResourceIcon {
			get;
			private set;
		}
		
		public ImageSource BinaryResourceIcon {
			get;
			private set;
		}
	}
}

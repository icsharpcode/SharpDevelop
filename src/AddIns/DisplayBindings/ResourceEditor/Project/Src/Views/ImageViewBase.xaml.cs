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
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ICSharpCode.SharpDevelop;
using ResourceEditor.ViewModels;

namespace ResourceEditor.Views
{
	/// <summary>
	/// Interaction logic for ImageViewBase.xaml
	/// </summary>
	public partial class ImageViewBase : UserControl, IResourceItemView
	{
		ResourceEditor.ViewModels.ResourceItem resourceItem;
		
		public ImageViewBase()
		{
			InitializeComponent();
			DataContext = this;
		}
		
		public static readonly DependencyProperty DisplayedImageProperty =
			DependencyProperty.Register("DisplayedImage", typeof(object), typeof(ImageViewBase),
				new FrameworkPropertyMetadata());
		
		public object DisplayedImage {
			get { return (object)GetValue(DisplayedImageProperty); }
			set { SetValue(DisplayedImageProperty, value); }
		}
		
		public static readonly DependencyProperty UpdateLinkTextProperty =
			DependencyProperty.Register("UpdateLinkText", typeof(string), typeof(ImageViewBase),
				new FrameworkPropertyMetadata());
		
		public string UpdateLinkText {
			get { return (string)GetValue(UpdateLinkTextProperty); }
			set { SetValue(UpdateLinkTextProperty, value); }
		}

		public FrameworkElement UIControl {
			get {
				return this;
			}
		}
		
		public ResourceEditor.ViewModels.ResourceItem ResourceItem {
			get {
				return resourceItem;
			}
			set {
				resourceItem = value;
				UpdateLinkText = "";
				if (resourceItem != null) {
					switch (resourceItem.ResourceType) {
						case ResourceItemEditorType.Bitmap:
							var gdiBitmap = resourceItem.ResourceValue as System.Drawing.Bitmap;
							if (gdiBitmap != null) {
								DisplayedImage = gdiBitmap.ToBitmapSource();
								UpdateLinkText = SD.ResourceService.GetString("ResourceEditor.BitmapView.UpdateBitmap");
							}
							break;
					}
				}
			}
		}
		
		void Button_Click(object sender, RoutedEventArgs e)
		{
			if (resourceItem != null) {
				if (resourceItem.UpdateFromFile()) {
					ResourceItem = resourceItem;
				}
			}
		}
	}
}
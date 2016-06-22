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
using System.IO;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.WpfDesign.PropertyGrid;

namespace ICSharpCode.WpfDesign.AddIn.ImageSourceEditor
{
	/// <summary>
	/// Editor to edit properties of type of ImageSource such as Windows.Icon or Image.Source
	/// </summary>
	[TypeEditor(typeof (ImageSource))]
	public partial class ImageSourceEditor
	{
		public ImageSourceEditor()
		{
			InitializeComponent();
		}
		
		private void ChooseImageClick(object sender, RoutedEventArgs e)
		{
			PropertyNode propNode = (PropertyNode)DataContext;
			
			string directory = null;
			var uriContext = propNode.Services.GetService<IUriContext>();
			if (uriContext != null && uriContext.BaseUri.IsFile) {
				directory = Path.GetDirectoryName(uriContext.BaseUri.LocalPath);
			}
			
			string fileName = propNode.Value as string;
			if (fileName != null && directory != null) {
				fileName = Path.Combine(directory, fileName);
			}
			ChooseImageDialog cid = new ChooseImageDialog();
			cid.SelectedFileName = fileName;
			cid.Owner = SD.Workbench.MainWindow;
			if (cid.ShowDialog() == true) {
				fileName = cid.SelectedFileName;
				if (fileName == null) {
					propNode.Reset();
				} else {
					propNode.Value = FileUtility.GetRelativePath(directory, fileName);
				}
			}
		}
	}
}

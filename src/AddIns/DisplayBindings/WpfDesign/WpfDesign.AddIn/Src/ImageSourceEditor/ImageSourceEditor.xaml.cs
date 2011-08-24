// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
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
			cid.Owner = WorkbenchSingleton.MainWindow;
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

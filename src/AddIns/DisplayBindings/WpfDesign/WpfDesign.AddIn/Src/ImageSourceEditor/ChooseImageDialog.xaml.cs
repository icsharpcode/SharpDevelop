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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Data;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;
using Microsoft.Win32;

namespace ICSharpCode.WpfDesign.AddIn.ImageSourceEditor
{
	/// <summary>
	/// Dialog which allows user to add images to the project
	/// </summary>
	public partial class ChooseImageDialog : Window
	{
		/// <summary>
		/// Contains the allowed extensions for image files.
		/// </summary>
		static readonly string[] Extension = {".jpg", ".bmp", ".png", ".gif", ".ico", ".dib", ".jpe", ".jpeg", ".tif", ".tiff"};
		
		private ObservableCollection<ImageData> _data = new ObservableCollection<ImageData>();
		
		public ChooseImageDialog()
		{
			InitializeComponent();
			
			// Get image file with allowed extensions
			AddImages(ProjectTools.RetrieveFiles(ChooseImageDialog.Extension));
			
			CollectionViewSource cvs = new CollectionViewSource();
			cvs.Source = _data;
			cvs.GroupDescriptions.Add(new PropertyGroupDescription("Directory"));
			cvs.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
			imgDisplay.ItemsSource = cvs.View;
		}
		
		List<ImageData> AddImages(IEnumerable<FileProjectItem> files)
		{
			List<ImageData> images = new List<ImageData>();
			foreach (FileProjectItem file in files) {
				ImageData image = new ImageData(file.FileName, Path.GetDirectoryName(file.VirtualName) + Path.DirectorySeparatorChar);
				images.Add(image);
				_data.Add(image);
			}
			return images;
		}
		
		#region Event Handlers
		private void AddClick(object sender, RoutedEventArgs e)
		{
			OpenFileDialog dialog = new OpenFileDialog();
			dialog.Filter = "Image Files|*" + String.Join(";*",Extension) + "|All Files|*.*";
			dialog.Multiselect = true;
			dialog.CheckFileExists = true;
			dialog.Title = "Choose Image";
			
			if (dialog.ShowDialog() == true) {
				string[] fileNames = dialog.FileNames;
				var files = ProjectTools.AddFiles(fileNames, ItemType.Resource);
				imgDisplay.SelectedItem = files.FirstOrDefault();
			}
		}
		
		private void OkClick(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
			Close();
		}
		
		private void ImageDisplayDoubleClick(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
			Close();
		}
		
		public string SelectedFileName {
			get {
				var image = (ImageData)imgDisplay.SelectedItem;
				if (image != null) {
					return image.FullName;
				} else {
					return null;
				}
			}
			set {
				imgDisplay.SelectedItem = _data.FirstOrDefault(d => FileUtility.IsEqualFileName(d.FullName, value));
			}
		}
		
		private void Cancel(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
			Close();
		}
		#endregion
	}
	
	public class ImageData
	{
		/// <summary>
		/// The directory (relative to project root) where the image is stored.
		/// </summary>
		public string Directory { get; private set; }
		
		/// <summary>
		/// The short file name.
		/// </summary>
		public string Name { get; private set; }
		
		/// <summary>
		/// The full file name.
		/// </summary>
		public string FullName { get; private set; }
		
		public ImageData(string fullName, string relDirectory)
		{
			this.FullName = fullName;
			this.Name = Path.GetFileName(fullName);
			this.Directory = relDirectory;
		}
	}
}

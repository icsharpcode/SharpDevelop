// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Kumar Devvrat"/>
//     <version>$Revision: $</version>
// </file>
using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;

using ICSharpCode.WpfDesign.PropertyGrid;

namespace ICSharpCode.WpfDesign.AddIn.ImageSourceEditor
{
	/// <summary>
	/// Dialog which allows user to add images to the project
	/// </summary>
	public partial class ChooseImageDialog : Window
	{
		/// <summary>
		/// Inner ListBox which displays the images
		/// </summary>
		private ListBox _imgDisplay;
		
		/// <summary>
		/// Contains the allowed extensions for image files.
		/// </summary>
		private static string[] Extension;
		
		private PropertyNode _node;
		private List<ImageData> _data;
		
		static ChooseImageDialog()
		{
			Extension = new String[]{".jpg", ".bmp", ".png", ".gif", ".ico", ".dib", ".jpe", ".jpeg", ".tif", ".tiff"};
		}
		
		public ChooseImageDialog(PropertyNode node)
		{
			this._node=node;
			this._data=new List<ImageData>();
			InitializeComponent();
		}
		
		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);
			List<KeyValuePair<string, string>> images=new List<KeyValuePair<string, string>>();
			
			/* Get image file with allowed extensions and group them with their directory */
			images=ProjectTools.RetrieveFiles(ChooseImageDialog.Extension);
			IEnumerable<IGrouping<string, string>> grouping = images.GroupBy(image => image.Key, image => image.Value);
			
			/* Set values for _data and bind to the ListBox */
			foreach(IGrouping<string, string> group in grouping){
				List<string> temp=new List<string>();
				foreach(var name in group){
					temp.Add(name);
				}
				_data.Add(new ImageData(group.Key + Path.DirectorySeparatorChar,temp));
			}
			
			Display.ItemsSource=_data;
			Display.SelectionChanged+=delegate { Display.SelectedItem=null; };
		}
		
		#region Event Handlers
		private void AddClick(object sender, RoutedEventArgs e)
		{
			OpenFileDialog dialog=new OpenFileDialog();
			dialog.Filter="Image Files | *" + String.Join(";*",Extension);
			dialog.Multiselect=true;
			dialog.CheckFileExists=true;
			dialog.Title="Choose Image";
			
			if(dialog.ShowDialog() == true){
				string []fileNames=dialog.FileNames;
				ProjectTools.AddFiles(fileNames);
				
				/* Add files to _data so that the ListBox is updated. Note that as Images are added to the project directory,
				 * images will be added in the _data with ImageData having header Path.DirectorySeparatorChar. */
				List<string> temp=new List<string>();
				foreach(var file in fileNames){
					temp.Add(Path.GetFileName(file));
				}
				if(fileNames.Length!=0)
					txURL.Text = fileNames.Last();
				_data.OrderBy(image => image.Header).ElementAt(0).Images.AddRange(fileNames);
			}
		}
		
		private void imgDisplayLoaded(object sender, RoutedEventArgs e)
		{
			_imgDisplay=new ListBox();
			_imgDisplay=sender as ListBox;
			_imgDisplay.MouseDoubleClick+=ImageDisplayDoubleClick;
		}
		
		private void imgDisplaySelectionChanged(object sender, RoutedEventArgs e)
		{
			_imgDisplay=sender as ListBox;
			if(_imgDisplay.SelectedItem!=null)
				txURL.Text=(string)_imgDisplay.SelectedItem;
		}
		
		private void OkClick(object sender, RoutedEventArgs e)
		{
			Save();
		}
		
		private void ImageDisplayDoubleClick(object sender, RoutedEventArgs e)
		{
			if(_imgDisplay.SelectedItem!=null){
				_node.Value=_imgDisplay.SelectedItem;
				Close();
			}
			
		}
		
		protected override void OnPreviewKeyUp(KeyEventArgs e)
		{
			base.OnPreviewKeyUp(e);
			if(e.Key==Key.Enter)
				Save();
		}
		
		private void Save()
		{
			Debug.Assert(_imgDisplay!=null);
			if(_imgDisplay.SelectedItem!=null){
				_node.Value=_imgDisplay.SelectedItem;
				Close();
			}else{
				if(File.Exists(txURL.Text) && uxOk.IsFocused){
					if(Extension.Contains(Path.GetExtension(txURL.Text)))
						_node.Value=Path.GetFullPath(txURL.Text);
					else
						MessageBox.Show(this, "The specified file is not a valid image file", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
					
					Close();
				}
				else{
					MessageBox.Show(this, "The specified file does not exist on the disk", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
		}
		
		private void Cancel(object sender, RoutedEventArgs e)
		{
			Close();
		}
		#endregion
	}
	
	public class ImageData
	{
		/// <summary>
		/// Stores the directory where <see cref="Images"/> are there.
		/// </summary>
		public string Header { get; set; }
		
		/// <summary>
		/// Contains the name of all images in <see cref="Header"/>.
		/// </summary>
		public List<string> Images { get; set; }
		
		public ImageData(string header,List<string> images)
		{
			this.Header=header;
			this.Images=images;
		}
	}
}
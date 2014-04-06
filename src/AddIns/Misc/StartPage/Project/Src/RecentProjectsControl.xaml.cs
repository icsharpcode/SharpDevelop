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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.StartPage
{
	/// <summary>
	/// Interaction logic for RecentProjectsControl.xaml
	/// </summary>
	public partial class RecentProjectsControl : UserControl
	{
		public RecentProjectsControl()
		{
			InitializeComponent();
			
			this.SetValueToExtension(HeaderProperty, new LocalizeExtension("StartPage.StartMenu.BarNameName"));
			BuildRecentProjectList();
		}
		
		public static readonly DependencyProperty HeaderProperty = HeaderedContentControl.HeaderProperty.AddOwner(typeof(RecentProjectsControl));
		
		public object Header {
			get { return GetValue(HeaderProperty); }
			set { SetValue(HeaderProperty, value); }
		}
		
		async void BuildRecentProjectList()
		{
			// When building the project list we access the .sln files (to see if they still exist).
			// Because those might be stored on a slow network drive, we do this on a background thread so that
			// SharpDevelop startup doesn't have to wait.
			var projectPaths = SD.FileService.RecentOpen.RecentProjects.ToArray();
			List<RecentOpenItem> items = new List<RecentOpenItem>();
			await Task.Run(
				delegate {
					foreach (FileName path in projectPaths) {
						Core.LoggingService.Debug("RecentProjectsControl: Looking up path '" + path + "'");
						FileInfo file = new FileInfo(path);
						if (file.Exists) {
							items.Add(
								new RecentOpenItem {
									Name = Path.GetFileNameWithoutExtension(path),
									LastModification = file.LastWriteTime.ToShortDateString(),
									Path = path
								});
						}
					}
				});
			if (items.Count > 0) {
				lastProjectsListView.ItemsSource = items;
				lastProjectsListView.Visibility = Visibility.Visible;
			}
		}
		
		class RecentOpenItem : INotifyPropertyChanged
		{
			public string Name { get; set; }
			public string LastModification { get; set; }
			public string Path { get; set; }
			
			event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged { add { } remove { } }
			
			public override string ToString()
			{
				return this.Name;
			}
		}
		
		void lastProjectsDoubleClick(object sender, RoutedEventArgs e)
		{
			RecentOpenItem item = (RecentOpenItem)lastProjectsListView.SelectedItem;
			if (item != null) {
				SD.ProjectService.OpenSolutionOrProject(FileName.Create(item.Path));
			}
		}
		
		void lastProjectsKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Return) {
				lastProjectsDoubleClick(null, null);
			}
		}
		
		void listViewHyperlinkClick(object sender, RoutedEventArgs e)
		{
			RecentOpenItem item = (RecentOpenItem)((Hyperlink)sender).Tag;
			SD.ProjectService.OpenSolutionOrProject(FileName.Create(item.Path));
		}
		
		void openSolutionClick(object sender, RoutedEventArgs e)
		{
			new ICSharpCode.SharpDevelop.Project.Commands.LoadSolution().Run();
		}
		
		void newSolutionClick(object sender, RoutedEventArgs e)
		{
			new ICSharpCode.SharpDevelop.Project.Commands.CreateNewSolution().Run();
		}
		
		void openContainingFolderClick(object sender, RoutedEventArgs e)
		{
			RecentOpenItem item = (RecentOpenItem)lastProjectsListView.SelectedItem;
			string folder = Path.GetDirectoryName(item.Path);
			Process.Start("explorer", "\"" + folder + "\"");
		}
		
		void removeRecentProjectClick(object sender, RoutedEventArgs e)
		{
			RecentOpenItem item = (RecentOpenItem)lastProjectsListView.SelectedItem;
			SD.FileService.RecentOpen.RemoveRecentProject(new FileName(item.Path));
			BuildRecentProjectList();
		}
	}
}

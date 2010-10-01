// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
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
		
		
		void BuildRecentProjectList()
		{
			// When building the project list we access the .sln files (to see if they still exist).
			// Because those might be stored on a slow network drive, we do this on a background thread so that
			// SharpDevelop startup doesn't have to wait.
			ThreadPool.QueueUserWorkItem(AsyncBuildRecentProjectList, FileService.RecentOpen.RecentProject.ToArray());
		}
		
		void AsyncBuildRecentProjectList(object state)
		{
			List<RecentOpenItem> items = new List<RecentOpenItem>();
			foreach (string path in (string[])state) {
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
			if (items.Count > 0) {
				WorkbenchSingleton.SafeThreadAsyncCall(new Action(
					delegate {
						lastProjectsListView.ItemsSource = items;
						lastProjectsListView.Visibility = Visibility.Visible;
					}));
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
				ProjectService.LoadSolutionOrProject(item.Path);
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
			ProjectService.LoadSolutionOrProject(item.Path);
		}
		
		void openSolutionClick(object sender, RoutedEventArgs e)
		{
			new ICSharpCode.SharpDevelop.Project.Commands.LoadSolution().Run();
		}
		
		void newSolutionClick(object sender, RoutedEventArgs e)
		{
			new ICSharpCode.SharpDevelop.Project.Commands.CreateNewSolution().Run();
		}
	}
}

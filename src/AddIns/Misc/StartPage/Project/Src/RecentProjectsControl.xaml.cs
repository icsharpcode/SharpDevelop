// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

using ICSharpCode.Core.Presentation;
using ICSharpCode.SharpDevelop;
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
			List<RecentOpenItem> items = new List<RecentOpenItem>();
			foreach (string path in FileService.RecentOpen.RecentProject) {
				FileInfo file = new FileInfo(path);
				if (file.Exists) {
					items.Add(
						new RecentOpenItem {
							Name = System.IO.Path.GetFileNameWithoutExtension(path),
							LastModification = file.LastWriteTime.ToShortDateString(),
							Path = path
						});
				}
			}
			lastProjectsListView.ItemsSource = items;
			lastProjectsListView.Visibility = items.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
		}
		
		class RecentOpenItem
		{
			public string Name { get; set; }
			public string LastModification { get; set; }
			public string Path { get; set; }
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
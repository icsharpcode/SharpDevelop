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

using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.StartPage
{
	/// <summary>
	/// Interaction logic for StartPageControl.xaml
	/// </summary>
	public partial class StartPageControl : UserControl
	{
		public StartPageControl()
		{
			InitializeComponent();
			
			BuildRecentProjectList();
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
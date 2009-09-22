// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace ICSharpCode.SharpDevelop.Project.Converter
{
	/// <summary>
	/// Interaction logic for UpgradeView.xaml
	/// </summary>
	internal partial class UpgradeView : UserControl
	{
		public UpgradeView(Solution solution)
		{
			InitializeComponent();
			
			listView.ItemsSource = solution.Projects.OfType<IUpgradableProject>().Select(p => new Entry(p)).ToList();
			ListView_SelectionChanged(null, null);
		}
		
		void SelectAllCheckBox_Checked(object sender, RoutedEventArgs e)
		{
			listView.SelectAll();
		}
		
		void SelectAllCheckBox_Unchecked(object sender, RoutedEventArgs e)
		{
			listView.SelectedItems.Clear();
		}
		
		void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (listView.SelectedItems.Count == listView.ItemsSource.Cast<Entry>().Count())
				selectAllCheckBox.IsChecked = true;
			else if (listView.SelectedItems.Count == 0)
				selectAllCheckBox.IsChecked = false;
			else
				selectAllCheckBox.IsChecked = null;
			
			convertButton.IsEnabled = listView.SelectedItems.Count > 0;
		}
		
		internal sealed class Entry : INotifyPropertyChanged
		{
			public readonly IUpgradableProject Project;
			
			public Entry(IUpgradableProject project)
			{
				this.Project = project;
				
				this.compilerVersion = project.CurrentCompilerVersion;
				this.targetFramework = project.CurrentTargetFramework;
			}
			
			public string Name {
				get { return this.Project.Name; }
			}
			
			CompilerVersion compilerVersion;
			TargetFramework targetFramework;
			
			public string CompilerVersionName {
				get { return compilerVersion != null ? compilerVersion.DisplayName : null; }
			}
			
			public string TargetFrameworkName {
				get { return targetFramework != null ? targetFramework.DisplayName : null; }
			}
			
			public event PropertyChangedEventHandler PropertyChanged;
			
			void OnPropertyChanged(string propertyName)
			{
				if (PropertyChanged != null) {
					PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
				}
			}
		}
	}
}
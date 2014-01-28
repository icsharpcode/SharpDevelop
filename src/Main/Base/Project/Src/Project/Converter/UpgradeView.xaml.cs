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
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ICSharpCode.Core.Presentation;
using ICSharpCode.NRefactory.Utils;

namespace ICSharpCode.SharpDevelop.Project.Converter
{
	/// <summary>
	/// Interaction logic for UpgradeView.xaml
	/// </summary>
	internal partial class UpgradeView : UserControl
	{
		readonly ISolution solution;
		readonly List<Entry> entries;
		
		public UpgradeView(ISolution solution)
		{
			if (solution == null)
				throw new ArgumentNullException("solution");
			this.solution = solution;
			InitializeComponent();
			
			this.entries = solution.Projects.OfType<IUpgradableProject>().Select(p => new Entry(p)).ToList();
			listView.ItemsSource = entries;
			SortableGridViewColumn.SetCurrentSortColumn(listView, nameColumn);
			SortableGridViewColumn.SetSortDirection(listView, ColumnSortDirection.Ascending);
			ListView_SelectionChanged(null, null);
		}
		
		public bool UpgradeViewOpenedAutomatically {
			get { return upgradeDescription.Visibility == Visibility.Visible; }
			set { upgradeDescription.Visibility = value ? Visibility.Visible : Visibility.Collapsed; }
		}
		
		public ISolution Solution {
			get { return solution; }
		}
		
		public void Select(IUpgradableProject project)
		{
			foreach (Entry entry in entries) {
				if (entry.Project == project)
					listView.SelectedItem = entry;
			}
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
			
			conversionGroupBox.IsEnabled = listView.SelectedItems.Count > 0;
			UpdateCompilerComboBox();
		}
		
		void UpdateCompilerComboBox()
		{
			if (listView.SelectedItems.Count > 0) {
				// Fetch list of available compiler versions
				HashSet<CompilerVersion> availableVersionsSet = new HashSet<CompilerVersion>();
				HashSet<CompilerVersion> currentVersions = new HashSet<CompilerVersion>();
				foreach (Entry entry in listView.SelectedItems) {
					if (entry.CompilerVersion != null)
						currentVersions.Add(entry.CompilerVersion);
					availableVersionsSet.AddRange(entry.Project.GetAvailableCompilerVersions());
				}
				List<CompilerVersion> availableVersions = availableVersionsSet.OrderBy(n => n.MSBuildVersion).ThenBy(n => n.DisplayName).ToList();
				if (currentVersions.Count != 1) {
					availableVersions.Insert(0, new UnchangedCompilerVersion());
				}
				// Assign available versions to newVersionComboBox
				// Unless the user has already chosen a version, automatically set the selection to the
				// current version of the chosen projects, or to 'do not change' if there are different
				// current versions.
				newCompilerSelectionChangingByCode = true;
				newVersionComboBox.ItemsSource = availableVersions;
				
				CompilerVersion oldSelectedVersion = newVersionComboBox.SelectedValue as CompilerVersion;
				if (!newCompilerSelectionSetByUser || oldSelectedVersion == null) {
					newCompilerSelectionSetByUser = false;
					if (currentVersions.Count == 1)
						newVersionComboBox.SelectedValue = currentVersions.Single();
					else
						newVersionComboBox.SelectedValue = new UnchangedCompilerVersion();
				}
				newCompilerSelectionChangingByCode = false;
				UpdateTargetFrameworkComboBox();
			}
		}
		
		bool newCompilerSelectionChangingByCode;
		bool newCompilerSelectionSetByUser;
		
		void newVersionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (!newCompilerSelectionChangingByCode)
				newCompilerSelectionSetByUser = true;
			UpdateTargetFrameworkComboBox();
		}
		
		void UpdateTargetFrameworkComboBox()
		{
			// Determine the available target frameworks
			bool doNotChangeAllowed;
			CompilerVersion selectedCompiler = newVersionComboBox.SelectedValue as CompilerVersion;
			if (selectedCompiler is UnchangedCompilerVersion)
				selectedCompiler = null;
			
			// Calculate the intersection of available frameworks for all selected projects:
			HashSet<TargetFramework> availableFrameworkSet = null;
			foreach (Entry entry in listView.SelectedItems) {
				var entryFrameworks = entry.Project.GetAvailableTargetFrameworks()
					.Where(fx => (selectedCompiler ?? entry.CompilerVersion).CanTarget(fx));
				if (availableFrameworkSet == null)
					availableFrameworkSet = new HashSet<TargetFramework>(entryFrameworks);
				else
					availableFrameworkSet.IntersectWith(entryFrameworks);
			}
			
			// Allow do not change on target framework if all current frameworks are supported
			// by the new compiler.
			doNotChangeAllowed = true;
			foreach (Entry entry in listView.SelectedItems) {
				doNotChangeAllowed &= availableFrameworkSet.Contains(entry.TargetFramework);
			}
			
			List<TargetFramework> availableFrameworks = availableFrameworkSet.ToList();
			availableFrameworks.Sort((a, b) => a.DisplayName.CompareTo(b.DisplayName));
			if (doNotChangeAllowed) {
				availableFrameworks.Insert(0, new UnchangedTargetFramework());
			}
			
			// detect whether all projects use a single framework
			TargetFramework frameworkUsedByAllProjects = null;
			bool frameworkUsedByAllProjectsInitialized = false;
			foreach (Entry entry in listView.SelectedItems) {
				if (!frameworkUsedByAllProjectsInitialized) {
					frameworkUsedByAllProjects = entry.TargetFramework;
					frameworkUsedByAllProjectsInitialized = true;
				} else {
					if (!object.Equals(frameworkUsedByAllProjects, entry.TargetFramework))
						frameworkUsedByAllProjects = null;
				}
			}
			
			// if projects use different frameworks, preselect "<do not change>", if possible
			if (frameworkUsedByAllProjects == null && doNotChangeAllowed)
				frameworkUsedByAllProjects = availableFrameworks[0];
			
			newFrameworkSelectionChangingByCode = true;
			newFrameworkComboBox.ItemsSource = availableFrameworks;
			
			TargetFramework oldSelectedFramework = newFrameworkComboBox.SelectedValue as TargetFramework;
			if (!newFrameworkSelectionSetByUser || oldSelectedFramework == null) {
				newFrameworkSelectionSetByUser = false;
				if (availableFrameworks.Contains(frameworkUsedByAllProjects))
					newFrameworkComboBox.SelectedValue = frameworkUsedByAllProjects;
			}
			newFrameworkSelectionChangingByCode = false;
			UpdateConvertButtonEnabled();
		}
		
		bool newFrameworkSelectionChangingByCode;
		bool newFrameworkSelectionSetByUser;
		
		void newFrameworkComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (!newFrameworkSelectionChangingByCode)
				newFrameworkSelectionSetByUser = true;
			UpdateConvertButtonEnabled();
		}
		
		void UpdateConvertButtonEnabled()
		{
			CompilerVersion selectedCompiler = newVersionComboBox.SelectedValue as CompilerVersion;
			TargetFramework selectedFramework = newFrameworkComboBox.SelectedValue as TargetFramework;
			
			bool changingCompiler = false;
			bool changingFramework = false;
			if (!(selectedCompiler is UnchangedCompilerVersion)) {
				foreach (Entry entry in listView.SelectedItems) {
					if (!object.Equals(entry.CompilerVersion, selectedCompiler))
						changingCompiler = true;
				}
			}
			if (!(selectedFramework is UnchangedTargetFramework)) {
				foreach (Entry entry in listView.SelectedItems) {
					if (!object.Equals(entry.TargetFramework, selectedFramework))
						changingFramework = true;
				}
			}
			
			convertButton.IsEnabled = selectedCompiler != null && selectedFramework != null && (changingCompiler || changingFramework);
		}
		
		void convertButton_Click(object sender, RoutedEventArgs e)
		{
			SD.AnalyticsMonitor.TrackFeature(GetType(), "convertButton_Click");
			
			CompilerVersion selectedCompiler = newVersionComboBox.SelectedValue as CompilerVersion;
			TargetFramework selectedFramework = newFrameworkComboBox.SelectedValue as TargetFramework;
			if (selectedCompiler is UnchangedCompilerVersion)
				selectedCompiler = null;
			if (selectedFramework != null) {
				// Show dialog for picking target frameworks for portable library.
				// This also handles UnchangedTargetFramework
				selectedFramework = selectedFramework.PickFramework(listView.SelectedItems.Cast<Entry>().Select(entry => entry.Project).ToList());
			}
			
			
			foreach (Entry entry in listView.SelectedItems) {
				try {
					entry.UpgradeProject(selectedCompiler, selectedFramework);
				} catch (ProjectUpgradeException ex) {
					Core.MessageService.ShowError("Cannot upgrade '" + entry.Name + "': " + ex.Message);
					break;
				}
			}
			
			solution.Save();
			UpdateCompilerComboBox();
		}
		
		sealed class UnchangedCompilerVersion : CompilerVersion
		{
			public UnchangedCompilerVersion() : base(new Version(0, 0), Core.StringParser.Parse("${res:ICSharpCode.SharpDevelop.Project.UpgradeView.DoNotChange}"))
			{
			}
			
			public override bool Equals(object obj)
			{
				return obj is UnchangedCompilerVersion;
			}
			
			public override int GetHashCode()
			{
				return 0;
			}
		}
		
		sealed class UnchangedTargetFramework : TargetFramework
		{
			public override string TargetFrameworkVersion {
				get { return string.Empty; }
			}
			
			public override string DisplayName {
				get {
					return Core.StringParser.Parse("${res:ICSharpCode.SharpDevelop.Project.UpgradeView.DoNotChange}");
				}
			}
			
			public override Version Version {
				get { return null; }
			}
			
			public override Version MinimumMSBuildVersion {
				get { return null; }
			}
			
			public override TargetFramework PickFramework(IEnumerable<IUpgradableProject> selectedProjects)
			{
				return null;
			}
			
			public override bool Equals(object obj)
			{
				return obj is UnchangedTargetFramework;
			}
			
			public override int GetHashCode()
			{
				return 0;
			}
		}
		
		internal sealed class Entry : INotifyPropertyChanged
		{
			public readonly IUpgradableProject Project;
			
			public Entry(IUpgradableProject project)
			{
				this.Project = project;
				
				this.CompilerVersion = project.CurrentCompilerVersion;
				this.TargetFramework = project.CurrentTargetFramework;
			}
			
			public string Name {
				get { return this.Project.Name; }
			}
			
			public override string ToString()
			{
				return this.Name;
			}
			
			public CompilerVersion CompilerVersion;
			public TargetFramework TargetFramework;
			
			public string CompilerVersionName {
				get { return CompilerVersion != null ? CompilerVersion.DisplayName : null; }
			}
			
			public string TargetFrameworkName {
				get { return TargetFramework != null ? TargetFramework.DisplayName : null; }
			}
			
			public void UpgradeProject(CompilerVersion newVersion, TargetFramework newFramework)
			{
				this.Project.UpgradeProject(newVersion, newFramework);
				
				this.CompilerVersion = this.Project.CurrentCompilerVersion;
				this.TargetFramework = this.Project.CurrentTargetFramework;
				OnPropertyChanged("CompilerVersionName");
				OnPropertyChanged("TargetFrameworkName");
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

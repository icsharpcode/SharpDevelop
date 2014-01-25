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
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace ICSharpCode.SharpDevelop.Project.PortableLibrary
{
	/// <summary>
	/// Interaction logic for SelectProfileDialog.xaml
	/// </summary>
	internal partial class SelectProfileDialog : Window, INotifyPropertyChanged
	{
		readonly ProfileList profileList;
		readonly List<SupportedFrameworkGroup> viewModels;
		
		public IEnumerable<SupportedFrameworkGroup> SupportedFrameworkGroups {
			get { return viewModels; }
		}
		
		public SelectProfileDialog(ProfileList profileList)
		{
			if (profileList == null)
				throw new ArgumentNullException("profileList");
			InitializeComponent();
			this.profileList = profileList;
			Debug.WriteLine(string.Join(Environment.NewLine, profileList.AllProfiles.Select(p => p.DisplayName)));
			viewModels = profileList.AllFrameworks.GroupBy(fx => fx.DisplayName, (key, group) => new SupportedFrameworkGroup(group)).ToList();
			this.DataContext = this;
			foreach (var vm in viewModels) {
				vm.PropertyChanged += delegate { UpdateSelectedProfile(); };
			}
			UpdateSelectedProfile();
		}
		
		void okButton_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = true;
			Close();
		}
		
		Profile selectedProfile;
		bool isSettingProfile;
		
		public Profile SelectedProfile {
			get { return selectedProfile; }
			set {
				selectedProfile = value;
				isSettingProfile = true;
				foreach (var g in viewModels) {
					if (value == null) {
						g.IsChecked = false;
					} else {
						SupportedFramework version = g.AvailableVersions.Intersect(value.SupportedFrameworks).FirstOrDefault();
						if (version != null) {
							g.IsChecked = true;
							g.SelectedVersion = version;
						} else {
							g.IsChecked = false;
						}
					}
				}
				isSettingProfile = false;
				PropertyChanged(this, new PropertyChangedEventArgs("SelectedProfile"));
			}
		}
		
		public bool HasTwoOrMoreFrameworksSelected {
			get { return viewModels.Count(vm => vm.IsChecked) >= 2; }
		}
		
		void UpdateSelectedProfile()
		{
			PropertyChanged(this, new PropertyChangedEventArgs("HasTwoOrMoreFrameworksSelected"));
			if (isSettingProfile)
				return;
			var requestedFrameworks = (from g in viewModels where g.IsChecked select g.SelectedVersion).ToList();
			var bestProfile = profileList.GetBestProfile(requestedFrameworks);
			if (bestProfile != selectedProfile) {
				selectedProfile = bestProfile;
				PropertyChanged(this, new PropertyChangedEventArgs("SelectedProfile"));
			}
		}
		
		public event PropertyChangedEventHandler PropertyChanged = delegate {};
	}
	
	/// <summary>
	/// View model used in SelectProfileDialog
	/// </summary>
	sealed class SupportedFrameworkGroup : INotifyPropertyChanged
	{
		readonly IList<SupportedFramework> availableVersions;
		bool isChecked;
		SupportedFramework selectedVersion;
		
		public SupportedFrameworkGroup(IEnumerable<SupportedFramework> availableVersions)
		{
			this.availableVersions = availableVersions.OrderBy(v => v.MinimumVersion).ToList();
			this.isChecked = true;
			this.selectedVersion = this.availableVersions.First();
		}
		
		public IList<SupportedFramework> AvailableVersions {
			get { return availableVersions; }
		}
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		private void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null) {
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		
		public string DisplayName {
			get { return availableVersions[0].DisplayName; }
		}
		
		public bool IsChecked {
			get { return isChecked; }
			set {
				if (isChecked != value) {
					isChecked = value;
					OnPropertyChanged("IsChecked");
				}
			}
		}
		
		public SupportedFramework SelectedVersion {
			get { return selectedVersion; }
			set {
				if (selectedVersion != value) {
					selectedVersion = value;
					OnPropertyChanged("SelectedVersion");
				}
			}
		}
	}
}

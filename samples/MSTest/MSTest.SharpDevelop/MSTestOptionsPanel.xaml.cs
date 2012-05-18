// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Widgets;
using Microsoft.Win32;

namespace ICSharpCode.MSTest
{
	public partial class MSTestOptionsPanel : OptionPanel, INotifyPropertyChanged
	{
		string msTestPath;
		bool changed;
		
		public MSTestOptionsPanel()
		{
			InitializeComponent();
			BrowseCommand = new RelayCommand(Browse);
			msTestPath = MSTestOptions.MSTestPath;
			DataContext = this;
		}
		
		public ICommand BrowseCommand { get; private set; }
		
		void Browse()
		{
			var dialog = new OpenFileDialog();
			if (dialog.ShowDialog() ?? false) {
				MSTestPath = dialog.FileName;
			}
		}
		
		public string MSTestPath {
			get { return msTestPath; }
			set {
				msTestPath = value;
				changed = true;
				OnPropertyChanged("MSTestPath");
			}
		}
		
		public override bool SaveOptions()
		{
			if (changed) {
				MSTestOptions.MSTestPath = msTestPath;
			}
			return true;
		}
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		void OnPropertyChanged(string name)
		{
			if (PropertyChanged != null) {
				PropertyChanged(this, new PropertyChangedEventArgs(name));
			}
		}
	}
}
// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Widgets;
using Microsoft.Win32;
using SDCore = ICSharpCode.Core;

namespace ICSharpCode.PythonBinding
{
	public partial class PythonOptionsPanel : OptionPanel, INotifyPropertyChanged
	{
		PythonAddInOptions options = new PythonAddInOptions();
		string pythonFileName;
		string pythonLibraryPath;

		public PythonOptionsPanel()
		{
			InitializeComponent();
			this.pythonFileName = options.PythonFileName;
			this.pythonLibraryPath = options.PythonLibraryPath;
			this.BrowseCommand = new RelayCommand(Browse);
			this.DataContext = this;
		}
		
		public ICommand BrowseCommand { get; private set; }
		
		public string PythonFileName {
			get { return pythonFileName; }
			set {
				pythonFileName = value;
				OnPropertyChanged("PythonFileName");
			}
		}
		
		public string PythonLibraryPath {
			get { return pythonLibraryPath; }
			set { pythonLibraryPath = value; }
		}
		
		void Browse()
		{
			var dialog = new OpenFileDialog();
			dialog.Filter = SDCore.StringParser.Parse("${res:SharpDevelop.FileFilter.ExecutableFiles}|*.exe");
			if (dialog.ShowDialog() ?? false) {
				PythonFileName = dialog.FileName;
			}
		}
		
		public override bool SaveOptions()
		{
			options.PythonFileName = pythonFileName;
			options.PythonLibraryPath = pythonLibraryPath;
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
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
using System.ComponentModel;
using System.Windows.Input;

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Gui.OptionPanels;
using ICSharpCode.SharpDevelop.Project;
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
				base.RaisePropertyChanged(() => PythonFileName);
			}
		}
		
		public string PythonLibraryPath {
			get { return pythonLibraryPath; }
			set { pythonLibraryPath = value; }
		}
		
		void Browse()
		{
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.DefaultExt = "exe";
			dlg.Filter = Core.StringParser.Parse("${res:SharpDevelop.FileFilter.ExecutableFiles}|*.exe");
			if (dlg.ShowDialog() == true) {
				PythonFileName = dlg.FileName;
			}
		}
		
		public override bool SaveOptions()
		{
			options.PythonFileName = pythonFileName;
			options.PythonLibraryPath = pythonLibraryPath;
			return true;
		}
	}
}

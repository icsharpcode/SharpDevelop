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

using ICSharpCode.SharpDevelop.Gui.Dialogs.ReferenceDialog.ServiceReference;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Widgets;
using Microsoft.Win32;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels.ServiceReference
{
	public partial class ServiceReferenceOptionsPanel : OptionPanel
	{
		string svcUtilPath;
		bool changed;
		
		public ServiceReferenceOptionsPanel()
		{
			InitializeComponent();
			BrowseCommand = new RelayCommand(Browse);
			svcUtilPath = ServiceReferenceOptions.SvcUtilPath;
			DataContext = this;
		}
		
		public ICommand BrowseCommand { get; private set; }
		
		void Browse()
		{
			var dialog = new OpenFileDialog();
			if (dialog.ShowDialog() ?? false) {
				SvcUtilPath = dialog.FileName;
			}
		}
		
		
		public string SvcUtilPath {
			get { return svcUtilPath; }
			set {
				svcUtilPath = value;
				changed = true;
				base.RaisePropertyChanged(() => SvcUtilPath);
			}
		}
		
		public override bool SaveOptions()
		{
			if (changed) {
				ServiceReferenceOptions.SvcUtilPath = svcUtilPath;
			}
			return true;
		}
	}
}

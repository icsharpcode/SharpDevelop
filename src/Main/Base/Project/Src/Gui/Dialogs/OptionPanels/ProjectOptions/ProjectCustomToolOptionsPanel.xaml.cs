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
using System.Windows.Controls;

using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
	public partial class ProjectCustomToolOptionsPanel : OptionPanel, ICanBeDirty
	{
		ProjectCustomToolOptions customToolsOptions;
		bool runCustomToolOnBuild;
		string fileNames;
		bool isDirty;
		
		public ProjectCustomToolOptionsPanel()
		{
			this.DataContext = this;
			InitializeComponent();
		}
		
		public bool RunCustomToolOnBuild {
			get { return runCustomToolOnBuild; }
			set {
				runCustomToolOnBuild = value;
				OnChanged();
			}
		}
		
		public string FileNames {
			get { return fileNames; }
			set {
				fileNames = value;
				OnChanged();
			}
		}
		
		void OnChanged()
		{
			IsDirty = OptionsHaveChanged();
		}
		
		bool OptionsHaveChanged()
		{
			return
				(runCustomToolOnBuild != customToolsOptions.RunCustomToolOnBuild) ||
				(fileNames != customToolsOptions.FileNames);
		}
		
		public override void LoadOptions()
		{
			var project = Owner as IProject;
			
			customToolsOptions = new ProjectCustomToolOptions(project);
			RunCustomToolOnBuild = customToolsOptions.RunCustomToolOnBuild;
			FileNames = customToolsOptions.FileNames;
			
			OnPropertyChanged();
		}
		
		public override bool SaveOptions()
		{
			if (OptionsHaveChanged()) {
				customToolsOptions.RunCustomToolOnBuild = runCustomToolOnBuild;
				customToolsOptions.FileNames = fileNames;
			}
			IsDirty = false;
			return true;
		}
		
		void OnPropertyChanged()
		{
			RaisePropertyChanged(null);
		}
		
		public event EventHandler IsDirtyChanged;
		
		void OnIsDirtyChanged()
		{
			if (IsDirtyChanged != null) {
				IsDirtyChanged(this, new EventArgs());
			}
		}
		
		public bool IsDirty {
			get { return isDirty; }
			
			private set {
				if (isDirty != value) {
					isDirty = value;
					OnIsDirtyChanged();
				}
			}
		}
	}
}

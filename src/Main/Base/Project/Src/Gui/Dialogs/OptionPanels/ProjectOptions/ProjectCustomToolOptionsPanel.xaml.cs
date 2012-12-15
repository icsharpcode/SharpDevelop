// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
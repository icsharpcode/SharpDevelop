// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
	partial class BuildEvents : ProjectOptionPanel
	{
		public BuildEvents()
		{
			InitializeComponent();
		}
		
		public ProjectProperty<string> PreBuildEvent {
			get { return GetProperty("PreBuildEvent", "", TextBoxEditMode.EditRawProperty); }
			
		}
		
		public ProjectProperty<string> PostBuildEvent {
			get { return GetProperty("PostBuildEvent", "", TextBoxEditMode.EditRawProperty); }
		}
		
		public ProjectProperty<RunPostBuildEvent> RunPostBuildEvent {
			get { return GetProperty("RunPostBuildEvent", SharpDevelop.Project.RunPostBuildEvent.OnBuildSuccess); }
		}
	}
}

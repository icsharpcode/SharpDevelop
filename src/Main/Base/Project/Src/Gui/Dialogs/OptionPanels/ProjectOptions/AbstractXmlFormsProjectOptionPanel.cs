// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
	/// <summary>
	/// Base class for project option panels that are using the <see cref="ConfigurationGuiHelper"/>.
	/// </summary>
	public abstract class AbstractXmlFormsProjectOptionPanel : XmlFormsOptionPanel, ICanBeDirty
	{
		protected ConfigurationGuiHelper helper;
		protected MSBuildBasedProject project;
		
		protected void InitializeHelper()
		{
			project = (MSBuildBasedProject)Owner;
			baseDirectory = project.Directory;
			helper = new ConfigurationGuiHelper(project, this.ControlDictionary);
		}
		
		public bool IsDirty {
			get { return helper.IsDirty; }
			set { helper.IsDirty = value; }
		}
		
		public event EventHandler IsDirtyChanged {
			add    { helper.IsDirtyChanged += value; }
			remove { helper.IsDirtyChanged -= value; }
		}
		
		public override bool StorePanelContents()
		{
			return helper.Save();
		}
	}
}

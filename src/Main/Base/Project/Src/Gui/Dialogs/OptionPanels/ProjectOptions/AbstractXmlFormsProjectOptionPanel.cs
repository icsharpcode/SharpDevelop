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
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
	/// <summary>
	/// Base class for project option panels that are using the <see cref="ConfigurationGuiHelper"/>.
	/// </summary>
	[Obsolete("XML Forms are obsolete")]
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

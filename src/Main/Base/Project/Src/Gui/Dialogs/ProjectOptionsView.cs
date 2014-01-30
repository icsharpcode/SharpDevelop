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
using System.IO;
using System.Linq;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Workbench;

namespace ICSharpCode.SharpDevelop.Project.Dialogs
{
	/// <summary>
	/// Option view for project options.
	/// </summary>
	public class ProjectOptionsView : AbstractViewContentWithoutFile
	{
		List<IOptionPanelDescriptor> descriptors = new List<IOptionPanelDescriptor>();
		TabbedOptions tabControl = new TabbedOptions();
		IProject project;
		
		public IProject Project {
			get {
				return project;
			}
		}
		
		public override object Control {
			get {
				return tabControl;
			}
		}
		
		public ProjectOptionsView(AddInTreeNode node, IProject project)
		{
			this.project = project;
			this.TitleName = project.Name;
			
			tabControl.IsDirtyChanged += delegate { RaiseIsDirtyChanged(); };
			tabControl.AddOptionPanels(node.BuildChildItems<IOptionPanelDescriptor>(project));
			
			project.Disposed += Project_Disposed;
		}
		
		void Project_Disposed(object sender, EventArgs e)
		{
			if (this.WorkbenchWindow != null)
				WorkbenchWindow.CloseWindow(true);
		}
		
		public override bool IsDirty {
			get { return tabControl.IsDirty; }
		}
		
		public override void Load()
		{
			foreach (IOptionPanel op in tabControl.OptionPanels) {
				op.LoadOptions();
			}
		}
		
		public override void Save()
		{
			try {
				foreach (IOptionPanel op in tabControl.OptionPanels) {
					op.SaveOptions();
				}
			} catch (Exception ex) {
				MessageService.ShowException(ex, "Error saving project options panel");
				return;
			}
			project.Save();
		}
		
		public override void Dispose()
		{
			project.Disposed -= Project_Disposed;
			foreach (IDisposable op in tabControl.OptionPanels.OfType<IDisposable>()) {
				op.Dispose();
			}
			base.Dispose();
		}
	}
}

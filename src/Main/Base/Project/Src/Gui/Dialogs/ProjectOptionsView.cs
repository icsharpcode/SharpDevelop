// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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

// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.IO;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

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
				MessageService.ShowError(ex, "Error saving project options panel");
				return;
			}
			project.Save();
		}
	}
}

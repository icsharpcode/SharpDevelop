// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Project.Dialogs
{
	/// <summary>
	/// Description of ProjectOptionsControl.
	/// </summary>
	public class ProjectOptionsView : AbstractViewContent
	{
		List<IDialogPanelDescriptor> descriptors = new List<IDialogPanelDescriptor>();
		TabControl tabControl = new TabControl();
		IProject   project;
		
		public override string TitleName {
			get {
				return project.Name;
			}
		}
		
		public override Control Control {
			get {
				return tabControl;
			}
		}
		
		public ProjectOptionsView(AddInTreeNode node, IProject project) : base("a", "a")
		{
			this.project    = project;
			base.IsViewOnly = true;
			
//			tabControl.Alignment = TabAlignment.Left;
			
			tabControl.HandleCreated += TabControlHandleCreated;
			AddOptionPanels(node.BuildChildItems(this));
		}
		
		void TabControlHandleCreated(object sender, EventArgs e)
		{
			// I didn't check if this is visual styles related, but
			// docking the controls into the tab pages only works correctly
			// AFTER the tab control has been shown. (.NET 2.0 beta 2)
			// therefore call it after the current winforms event has been processed using BeginInvoke.
			tabControl.HandleCreated -= TabControlHandleCreated;
			tabControl.BeginInvoke(new MethodInvoker(DockControlsInPages));
		}
		
		void DockControlsInPages()
		{
			foreach (TabPage page in tabControl.TabPages) {
				foreach (Control ctl in page.Controls) {
					ctl.Dock = DockStyle.Fill;
				}
			}
		}
		
		void AddOptionPanels(ArrayList dialogPanelDescriptors)
		{
			Properties newProperties = new Properties();
			newProperties.Set("Project", project);
			
			foreach (IDialogPanelDescriptor descriptor in dialogPanelDescriptors) {
				descriptors.Add(descriptor);
				if (descriptor != null && descriptor.DialogPanel != null && descriptor.DialogPanel.Control != null) { // may be null, if it is only a "path"
					descriptor.DialogPanel.CustomizationObject = newProperties;
					descriptor.DialogPanel.ReceiveDialogMessage(DialogMessage.Activated);
					
					TabPage page = new TabPage(descriptor.Label);
					page.Controls.Add(descriptor.DialogPanel.Control);
					tabControl.TabPages.Add(page);
				}
				
				if (descriptor.ChildDialogPanelDescriptors != null) {
					AddOptionPanels(descriptor.ChildDialogPanelDescriptors);
				}
			}
		}
		
		public override void Save(string fileName)
		{
			foreach (IDialogPanelDescriptor pane in descriptors) {
				pane.DialogPanel.ReceiveDialogMessage(DialogMessage.OK);
			}
			ProjectService.SaveSolution();
		}
	}
}

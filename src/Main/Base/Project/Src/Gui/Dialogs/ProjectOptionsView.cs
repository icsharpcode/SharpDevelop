/*
 * Created by SharpDevelop.
 * User: Omnibrain
 * Date: 21.12.2004
 * Time: 11:54
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

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
			tabControl.Dock      = DockStyle.Fill;
			
			AddOptionPanels(node.BuildChildItems(this));			
		}
		
		void AddOptionPanels(ArrayList dialogPanelDescriptors)
		{
			Properties newProperties = new Properties();
			newProperties.Set("Project", project);
				
			foreach (IDialogPanelDescriptor descriptor in dialogPanelDescriptors) {
				descriptors.Add(descriptor);
				if (descriptor != null && descriptor.DialogPanel != null && descriptor.DialogPanel.Control != null) { // may be null, if it is only a "path"
					descriptor.DialogPanel.CustomizationObject = newProperties;
					descriptor.DialogPanel.Control.Dock = DockStyle.Fill;
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

// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui.XmlForms;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// Basic "tabbed" options dialog
	/// </summary>
	public class TabbedOptions : BaseSharpDevelopForm
	{
		ArrayList OptionPanels = new ArrayList();
		
		void AcceptEvent(object sender, EventArgs e)
		{
			foreach (AbstractOptionPanel pane in OptionPanels) {
				if (!pane.ReceiveDialogMessage(DialogMessage.OK)) {
					return;
				}
			}
			DialogResult = DialogResult.OK;
		}
		
		void AddOptionPanels(IEnumerable<IDialogPanelDescriptor> dialogPanelDescriptors)
		{
			foreach (IDialogPanelDescriptor descriptor in dialogPanelDescriptors) {
				if (descriptor != null && descriptor.DialogPanel != null && descriptor.DialogPanel.Control != null) { // may be null, if it is only a "path"
					descriptor.DialogPanel.Control.Dock = DockStyle.Fill;
					descriptor.DialogPanel.ReceiveDialogMessage(DialogMessage.Activated);
					OptionPanels.Add(descriptor.DialogPanel);
					
					TabPage page = new TabPage(descriptor.Label);
					page.UseVisualStyleBackColor = true;
					page.Controls.Add(descriptor.DialogPanel.Control);
					((TabControl)ControlDictionary["optionPanelTabControl"]).TabPages.Add(page);
				}
				
				if (descriptor.ChildDialogPanelDescriptors != null) {
					AddOptionPanels(descriptor.ChildDialogPanelDescriptors);
				}
			}
		}
		
		public TabbedOptions(string dialogName, AddInTreeNode node)
		{
			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("Resources.TabbedOptionsDialog.xfrm"));
			
			this.Text       = dialogName;
			ControlDictionary["okButton"].Click += new EventHandler(AcceptEvent);
			Icon = null;
			Owner = WorkbenchSingleton.MainForm;
			
			AddOptionPanels(node.BuildChildItems<IDialogPanelDescriptor>(this));
		}
	}
}

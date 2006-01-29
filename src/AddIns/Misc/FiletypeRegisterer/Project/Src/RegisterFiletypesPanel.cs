// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Georg Brandl" email="g.brandl@gmx.net"/>
//     <version>$Revision$</version>
// </file>

// created on 16.11.2002 at 21:14

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;
using System.Collections.Generic;

using ICSharpCode.SharpDevelop.Internal.ExternalTool;
using ICSharpCode.Core;
using ICSharpCode.FiletypeRegisterer;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Gui.OptionPanels;

namespace ICSharpCode.FiletypeRegisterer
{
	class RegisterFiletypesPanel : AbstractOptionPanel {
		
		ListView list   = new ListView();
		Label    capLbl = new Label();
		CheckBox regChk = new CheckBox();
		
		Hashtable wasChecked = new Hashtable();
		
		List<FiletypeAssociation> allTypes;
		
		public RegisterFiletypesPanel()
		{
			allTypes = FiletypeAssociationDoozer.GetList();
			
			// Initialize dialog controls
			InitializeComponent();
			
			// Set previous values
			SelectFiletypes(PropertyService.Get(RegisterFiletypesCommand.uiFiletypesProperty, RegisterFiletypesCommand.GetDefaultExtensions(allTypes)));
			regChk.Checked = PropertyService.Get(RegisterFiletypesCommand.uiRegisterStartupProperty, true);
		}
		
		public override bool ReceiveDialogMessage(DialogMessage message)
		{
			if (message == DialogMessage.OK) {
				UnRegisterFiletypes();
				RegisterFiletypesCommand.RegisterFiletypes(allTypes, SelectedFiletypes);
				PropertyService.Set(RegisterFiletypesCommand.uiFiletypesProperty, SelectedFiletypes);
				PropertyService.Set(RegisterFiletypesCommand.uiRegisterStartupProperty, regChk.Checked);
			}
			return true;
		}
		
		string SelectedFiletypes
		{
			get {
				try {
					string ret = "";
					
					foreach(ListViewItem lv in list.Items) {
						if(lv.Checked) ret += (string)lv.Tag + "|";
					}
					return ret;
				} catch {
					return "";
				}
			}
		}
		
		void UnRegisterFiletypes()
		{
			foreach(ListViewItem lv in list.Items) {
				if((!lv.Checked) && wasChecked.Contains((string)lv.Tag)) {
					RegisterFiletypesCommand.UnRegisterFiletype((string)lv.Tag);
				}
			}
		}
		
		void SelectFiletypes(string types) {
			string[] singleTypes = types.Split('|');
			
			foreach(string str in singleTypes) {
				wasChecked[str] = true;
				foreach(ListViewItem lv in list.Items) {
					if(str == (string)lv.Tag) {
						lv.Checked = true;
					}
				}
			}
		}
		
		void InitializeComponent()
		{
			capLbl.Location  = new Point(8, 8);
			capLbl.Size      = new Size(136, 16);
			capLbl.Anchor    = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			capLbl.Text      = StringParser.Parse("${res:ICSharpCode.SharpDevelop.Gui.Dialogs.OptionPanels.RegisterFiletypesPanel.CaptionLabel}");
			
			list.Location    = new Point(8, 30);
			list.Size        = new Size(136, 250);
			list.Anchor      = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			list.View        = View.List;
			list.CheckBoxes  = true;
			
			FillList(list);
			
			regChk.Location  = new Point(8, 300);
			regChk.Size      = new Size(136, 20);
			regChk.Anchor    = capLbl.Anchor;
			regChk.Text      = StringParser.Parse("${res:ICSharpCode.SharpDevelop.Gui.Dialogs.OptionPanels.RegisterFiletypesPanel.RegisterCheckBox}");
			
			this.Controls.AddRange(new Control[] {capLbl, list, regChk});
		}
		
		void FillList(ListView list)
		{
			foreach (FiletypeAssociation type in allTypes) {
				ListViewItem lv;
				lv = new ListViewItem(type.Text + " (." + type.Extension + ")");
				lv.Tag = type.Extension;
				list.Items.Add(lv);
			}
		}
	}
}

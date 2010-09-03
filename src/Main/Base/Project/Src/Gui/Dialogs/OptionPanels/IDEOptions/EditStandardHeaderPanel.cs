// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Forms;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop.Internal.Templates;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
	public class EditStandardHeaderPanel : XmlFormsOptionPanel
	{
		public override void LoadPanelContents()
		{
			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("Resources.EditStandardHeaderPanel.xfrm"));
			
			ControlDictionary["headerTextBox"].Font = WinFormsResourceService.DefaultMonospacedFont;
			foreach (StandardHeader header in StandardHeader.StandardHeaders) {
				((ComboBox)ControlDictionary["headerChooser"]).Items.Add(header);
			}
			((ComboBox)ControlDictionary["headerChooser"]).SelectedIndexChanged += new EventHandler(SelectedIndexChanged);
			((ComboBox)ControlDictionary["headerChooser"]).SelectedIndex = 0;
			((TextBox)ControlDictionary["headerTextBox"]).TextChanged += new EventHandler(TextChangedEvent);
		}
		
		void TextChangedEvent(object sender , EventArgs e)
		{
			((StandardHeader)((ComboBox)ControlDictionary["headerChooser"]).SelectedItem).Header = ControlDictionary["headerTextBox"].Text;
		}
		void SelectedIndexChanged(object sender , EventArgs e)
		{
			((TextBox)ControlDictionary["headerTextBox"]).TextChanged -= new EventHandler(TextChangedEvent);
			int idx =((ComboBox)ControlDictionary["headerChooser"]).SelectedIndex;
			if (idx >= 0) {
				ControlDictionary["headerTextBox"].Text = ((StandardHeader)((ComboBox)ControlDictionary["headerChooser"]).SelectedItem).Header;
				ControlDictionary["headerTextBox"].Enabled = true;
			} else {
				ControlDictionary["headerTextBox"].Text = "";
				ControlDictionary["headerTextBox"].Enabled = false;
			}
			((TextBox)ControlDictionary["headerTextBox"]).TextChanged += new EventHandler(TextChangedEvent);
		}
		
		public override bool StorePanelContents()
		{
			StandardHeader.StoreHeaders();
			return true;
		}
	}
}

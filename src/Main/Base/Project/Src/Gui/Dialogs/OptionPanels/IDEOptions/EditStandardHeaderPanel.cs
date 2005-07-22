// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Internal.ExternalTool;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Internal.Templates;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
	public class EditStandardHeaderPanel : AbstractOptionPanel
	{
		public override void LoadPanelContents()
		{
			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("Resources.EditStandardHeaderPanel.xfrm"));
			
			ControlDictionary["headerTextBox"].Font = new Font("Courier New", 10);
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

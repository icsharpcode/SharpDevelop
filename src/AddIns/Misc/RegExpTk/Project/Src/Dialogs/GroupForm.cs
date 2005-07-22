// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Markus Palme" email="MarkusPalme@gmx.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Windows.Forms;
using System.Text.RegularExpressions;

using ICSharpCode.SharpDevelop.Gui.XmlForms;

namespace Plugins.RegExpTk {

	public class GroupForm : BaseSharpDevelopForm
	{
		public GroupForm(Match match)
		{
			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("Resources.RegExpTkGroupForm.xfrm"));
			
			ListView groupsListView = (ListView)ControlDictionary["GroupsListView"];
			((Button)ControlDictionary["CloseButton"]).Click += new EventHandler(CloseButton_Click);
			foreach(Group group in match.Groups)
			{
				ListViewItem groupItem = groupsListView.Items.Add(group.Value);
				groupItem.SubItems.Add(group.Index.ToString());
				groupItem.SubItems.Add((group.Index + group.Length).ToString());
				groupItem.SubItems.Add(group.Length.ToString());
			}
		}
		
		protected override void SetupXmlLoader()
		{
			xmlLoader.StringValueFilter    = new SharpDevelopStringValueFilter();
			xmlLoader.PropertyValueCreator = new SharpDevelopPropertyValueCreator();
			xmlLoader.ObjectCreator        = new SharpDevelopObjectCreator();
		}
		
		void CloseButton_Click(object sender, EventArgs e)
		{
			Close();
		}
	}

}

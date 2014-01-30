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
using System.Text.RegularExpressions;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Gui.XmlForms;

namespace Plugins.RegExpTk {
	// TODO: remove XmlForms
	#pragma warning disable 618
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
		
		void CloseButton_Click(object sender, EventArgs e)
		{
			Close();
		}
	}

}

/*
 * Created by SharpDevelop.
 * User: itai
 * Date: 20/10/2006
 * Time: 20:41
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop;

namespace ClassDiagram
{
	/// <summary>
	/// Description of MembersList.
	/// </summary>
	public class MembersList<MT> where MT : IMember
	{
		TreeListViewItem titleItem;
		TreeListViewItem addNewMember;
		TreeListView treeListView;
		
		public MembersList(string title, ICollection<MT> members, TreeListView tlv)
		{
			treeListView = tlv;

			
			titleItem = tlv.Items.Add(title);
			
			//tlv.SmallImageList = ClassBrowserIconService.ImageList;
			
			if (members != null && members.Count != 0)
			{
				foreach (IMember member in members)
				{
					int icon = ClassBrowserIconService.GetIcon(member);
					TreeListViewItem methodItem = titleItem.Items.Add(member.Name, icon);
				}
			}
			
			addNewMember = titleItem.Items.Add("[Add]");
		}
		
		private void ItemActivated (object sender, EventArgs e)
		{
			
		}
	}
}

// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Build.Tasks;
using ICSharpCode.SharpDevelop.Gui;
using System;
using System.Windows.Forms;

namespace ICSharpCode.MonoAddIn
{
	public class MonoGacReferencePanel : GacReferencePanel
	{		
		public MonoGacReferencePanel(ISelectReferenceDialog selectDialog) : base(selectDialog)
		{
		}
		
		protected override List<ListViewItem> GetCacheContent()
		{
			List<ListViewItem> itemList = new List<ListViewItem>();
			foreach (MonoAssemblyName assemblyName in MonoGlobalAssemblyCache.GetAssemblyNames()) {
				ListViewItem item = new ListViewItem(new string[] {assemblyName.Name, assemblyName.Version.ToString(), assemblyName.Directory});
				item.Tag = assemblyName.FullName;
				itemList.Add(item);
			}
			return itemList;
		}
	}
}

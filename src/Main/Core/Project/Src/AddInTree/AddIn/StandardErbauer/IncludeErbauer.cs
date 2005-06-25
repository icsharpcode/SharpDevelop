/*
 * Created by SharpDevelop.
 * User: Daniel Grunwald
 * Date: 25.06.2005
 * Time: 15:15
 */

using System;
using System.Collections;

namespace ICSharpCode.Core
{
	/// <summary>
	/// Includes the content of one path into another path.
	/// </summary>
	public class IncludeErbauer : IErbauer
	{
		public object BuildItem(object caller, Codon codon, ArrayList subItems)
		{
			string item = codon.Properties["item"];
			string path = codon.Properties["path"];
			if (item != null && item.Length > 0) {
				// include item
				return AddInTree.BuildItem(item, caller);
			} else if (path != null && path.Length > 0) {
				// include path (=multiple items)
				return new IncludeReturnItem(caller, path);
			} else {
				Console.WriteLine("<Include> requires the attribute 'item' (to include one item) or the attribute 'path' (to include multiple items)");
				return null;
			}
		}
		
		class IncludeReturnItem : IBuildItemsModifier
		{
			string path;
			object caller;
			
			public IncludeReturnItem(object caller, string path)
			{
				this.caller = caller;
				this.path = path;
			}
			
			public void Apply(ArrayList items)
			{
				AddInTreeNode node;
				try {
					node = AddInTree.GetTreeNode(path);
					items.AddRange(node.BuildChildItems(caller));
				} catch (TreePathNotFoundException ex) {
					Console.WriteLine(ex);
				}
			}
		}
	}
}

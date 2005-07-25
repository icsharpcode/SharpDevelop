// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;

namespace ICSharpCode.Core
{
	/// <summary>
	/// Includes the content of one path into another path.
	/// </summary>
	public class IncludeErbauer : IErbauer
	{
		/// <summary>
		/// Gets if the erbauer handles codon conditions on its own.
		/// If this property return false, the item is excluded when the condition is not met.
		/// </summary>
		public bool HandleConditions {
			get {
				return false;
			}
		}
		
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

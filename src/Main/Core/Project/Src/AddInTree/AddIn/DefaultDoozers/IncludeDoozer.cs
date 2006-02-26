// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;

namespace ICSharpCode.Core
{
	/// <summary>
	/// Includes one or multiple items from another location in the addin tree.
	/// You can use the attribute "item" (to include a single item) OR the
	/// attribute "path" (to include all items from the target path).
	/// </summary>
	/// <attribute name="item">
	/// When this attribute is used, the include doozer builds the item that is at the
	/// addin tree location specified by this attribute.
	/// </attribute>
	/// <attribute name="path">
	/// When this attribute is used, the include doozer builds all items inside the
	/// path addin tree location specified by this attribute and returns an
	/// <see cref="IBuildItemsModifier"/> which includes all items in the output list.
	/// </attribute>
	/// <usage>Everywhere</usage>
	/// <returns>
	/// Any object, depending on the included codon(s).
	/// </returns>
	public class IncludeDoozer : IDoozer
	{
		/// <summary>
		/// Gets if the doozer handles codon conditions on its own.
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
				MessageService.ShowMessage("<Include> requires the attribute 'item' (to include one item) or the attribute 'path' (to include multiple items)");
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
			
			public void Apply(IList items)
			{
				AddInTreeNode node;
				try {
					node = AddInTree.GetTreeNode(path);
					foreach (object o in node.BuildChildItems(caller)) {
						items.Add(o);
					}
				} catch (TreePathNotFoundException) {
					MessageService.ShowError("IncludeDoozer: AddinTree-Path not found: " + path);
				}
			}
		}
	}
}

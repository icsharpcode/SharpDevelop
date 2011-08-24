// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;

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
				return true;
			}
		}
		
		public object BuildItem(BuildItemArgs args)
		{
			Codon codon = args.Codon;
			string item = codon.Properties["item"];
			string path = codon.Properties["path"];
			if (item != null && item.Length > 0) {
				// include item
				return AddInTree.BuildItem(item, args.Caller, args.Conditions);
			} else if (path != null && path.Length > 0) {
				// include path (=multiple items)
				return new IncludeReturnItem(args.Caller, path, args.Conditions);
			} else {
				throw new CoreException("<Include> requires the attribute 'item' (to include one item) or the attribute 'path' (to include multiple items)");
			}
		}
		
		sealed class IncludeReturnItem : IBuildItemsModifier
		{
			string path;
			object caller;
			IEnumerable<ICondition> additionalConditions;
			
			public IncludeReturnItem(object caller, string path, IEnumerable<ICondition> additionalConditions)
			{
				this.caller = caller;
				this.path = path;
				this.additionalConditions = additionalConditions;
			}
			
			public void Apply(IList items)
			{
				AddInTreeNode node = AddInTree.GetTreeNode(path, false);
				if (node != null) {
					foreach (object o in node.BuildChildItems<object>(caller, additionalConditions)) {
						items.Add(o);
					}
				} else {
					throw new CoreException("IncludeDoozer: AddinTree-Path not found: " + path);
				}
			}
		}
	}
}

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
				return args.AddInTree.BuildItem(item, args.Parameter, args.Conditions);
			} else if (path != null && path.Length > 0) {
				// include path (=multiple items)
				AddInTreeNode node = args.AddInTree.GetTreeNode(path);
				return new IncludeReturnItem(node, args.Parameter, args.Conditions);
			} else {
				throw new CoreException("<Include> requires the attribute 'item' (to include one item) or the attribute 'path' (to include multiple items)");
			}
		}
		
		sealed class IncludeReturnItem : IBuildItemsModifier
		{
			readonly AddInTreeNode node;
			readonly object parameter;
			readonly IEnumerable<ICondition> additionalConditions;
			
			public IncludeReturnItem(AddInTreeNode node, object parameter, IEnumerable<ICondition> additionalConditions)
			{
				this.node = node;
				this.parameter = parameter;
				this.additionalConditions = additionalConditions;
			}
			
			public void Apply(IList items)
			{
				foreach (object o in node.BuildChildItems<object>(parameter, additionalConditions)) {
					items.Add(o);
				}
			}
		}
	}
}

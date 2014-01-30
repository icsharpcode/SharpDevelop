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

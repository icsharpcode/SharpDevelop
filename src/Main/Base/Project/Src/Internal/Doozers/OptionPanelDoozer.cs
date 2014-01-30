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

using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// Creates DefaultOptionPanelDescriptor objects that are used in option dialogs.
	/// </summary>
	/// <attribute name="class">
	/// Name of the IOptionPanel class. Optional if the page has subpages.
	/// </attribute>
	/// <attribute name="label" use="required">
	/// Caption of the dialog panel.
	/// </attribute>
	/// <children childTypes="OptionPanel">
	/// In the SharpDevelop options, option pages can have subpages by specifying them
	/// as children in the AddInTree.
	/// </children>
	/// <usage>In /SharpDevelop/BackendBindings/ProjectOptions/ and /SharpDevelop/Dialogs/OptionsDialog</usage>
	/// <returns>
	/// A DefaultOptionPanelDescriptor object.
	/// </returns>
	public class OptionPanelDoozer : IDoozer
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
		
		/// <summary>
		/// Creates an item with the specified sub items. And the current
		/// Condition status for this item.
		/// </summary>
		public object BuildItem(BuildItemArgs args)
		{
			string label = args.Codon["label"];
			string id = args.Codon.Id;
			
			var subItems = args.BuildSubItems<IOptionPanelDescriptor>();
			if (subItems.Count == 0) {
				if (args.Codon.Properties.Contains("class")) {
					return new DefaultOptionPanelDescriptor(id, StringParser.Parse(label), args.AddIn, args.Parameter, args.Codon["class"]);
				} else {
					return new DefaultOptionPanelDescriptor(id, StringParser.Parse(label));
				}
			}
			
			return new DefaultOptionPanelDescriptor(id, StringParser.Parse(label), subItems);
		}
	}
}

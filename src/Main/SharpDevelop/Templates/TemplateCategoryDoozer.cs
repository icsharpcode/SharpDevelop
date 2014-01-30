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
using System.Collections.Generic;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.Utils;

namespace ICSharpCode.SharpDevelop.Templates
{
	/// <summary>
	/// Creates code completion bindings that manage code completion for one language.
	/// </summary>
	/// <attribute name="displayName" use="optional">
	/// The display name to use. If not specified, the codon id is used instead.
	/// </attribute>
	/// <attribute name="sortOrder" use="optional">
	/// An integer that is used for sorting; categories with higher values are listed first.
	/// For equal values, the categories will be sorted by the display name.
	/// </attribute>
	/// <children childTypes="TemplateBase">
	/// The &lt;TemplateCategory&gt; may have templates and subcategories as children.
	/// </children>
	/// <usage>In /SharpDevelop/BackendBindings/Templates and its subpaths (for subcategories)</usage>
	/// <returns>
	/// The <see cref="TemplateCategory"/> instance loaded from the template file.
	/// </returns>
	sealed class TemplateCategoryDoozer : IDoozer
	{
		public bool HandleConditions {
			get { return false; }
		}
		
		public object BuildItem(BuildItemArgs args)
		{
			IImage icon = null;
			if (args.Codon.Properties.Contains("icon"))
				icon = SD.ResourceService.GetImage(args.Codon.Properties["icon"]);
			TemplateCategory category = new TemplateCategory(
				args.Codon.Id,
				args.Codon.Properties["displayName"],
				args.Codon.Properties["description"],
				icon
			);
			int sortOrder;
			if (int.TryParse(args.Codon.Properties["sortOrder"], out sortOrder))
				category.SortOrder = sortOrder;
			LoadSubItems(category, args);
			return category;
		}
		
		void LoadSubItems(TemplateCategory category, BuildItemArgs args)
		{
			foreach (TemplateBase item in args.BuildSubItems<TemplateBase>()) {
				if (item is TemplateCategory)
					category.Subcategories.Add((TemplateCategory)item);
				else
					category.Templates.Add(item);
			}
		}
	}
}

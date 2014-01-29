// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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

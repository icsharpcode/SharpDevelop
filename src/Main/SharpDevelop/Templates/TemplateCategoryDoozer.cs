// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Templates
{
	/// <summary>
	/// Creates code completion bindings that manage code completion for one language.
	/// </summary>
	/// <attribute name="displayName" use="optional">
	/// The display name to use. If not specified, the codon id is used instead.
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
			foreach (TemplateBase item in args.BuildSubItems<TemplateBase>()) {
				if (item is TemplateCategory)
					category.Subcategories.Add((TemplateCategory)item);
				else
					category.Templates.Add(item);
			}
			return category;
		}
	}
}

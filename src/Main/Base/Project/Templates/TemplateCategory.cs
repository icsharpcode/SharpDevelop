// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Templates
{
	/// <summary>
	/// A group of file or project templates.
	/// </summary>
	public class TemplateCategory : TemplateBase
	{
		readonly string name;
		readonly string displayName;
		readonly string description;
		readonly IImage icon;
		
		/// <summary>
		/// An integer that is used for sorting; categories with higher values are listed first.
		/// For equal values, the categories will be sorted by the display name.
		/// </summary>
		public int SortOrder { get; set; }
		
		public override string Name {
			get { return name; }
		}
		
		public override string DisplayName {
			get { return displayName; }
		}
		
		public override string Description {
			get { return description; }
		}
		
		public override IImage Icon {
			get { return null; }
		}
		
		public IList<TemplateCategory> Subcategories { get; private set; }
		public IList<TemplateBase> Templates { get; private set; }
		
		public TemplateCategory(string name, string displayName = null, string description = null, IImage icon = null)
		{
			this.name = name;
			if (string.IsNullOrEmpty(displayName))
				this.displayName = name;
			else
				this.displayName = displayName;
			this.description = description;
			this.icon = icon;
			this.Subcategories = new List<TemplateCategory>();
			this.Templates = new List<TemplateBase>();
		}
	}
}

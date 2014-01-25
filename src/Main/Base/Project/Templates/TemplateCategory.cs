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

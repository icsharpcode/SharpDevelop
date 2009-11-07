using System;
using System.Linq;
using System.Collections.Generic;

namespace ICSharpCode.Core.Presentation
{
	/// <summary>
	/// Stores input binding category description
	/// </summary>
	public class InputBindingCategory
	{		
		/// <summary>
		/// Creates new instance of <see cref="InputBindingCategory" />
		/// </summary>
		/// <param name="name">Category name</param>
		/// <param name="parentCategory">Parent category (null - root level category)</param>
		public InputBindingCategory(string path, string text)
		{
			Path = path;
			Text = text;
		}
		
		public string Path
		{
			get; set;
		}
		
		/// <summary>
		/// Category name
		/// </summary>
		public string Text
		{
			get; set;
		}
		
		public override string ToString() {
			return Path;
		}
	}
}

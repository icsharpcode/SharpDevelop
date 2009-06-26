using System;

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
		public InputBindingCategory(string name, InputBindingCategory parentCategory)
		{
			Name = name;
			ParentCategory = parentCategory;
		}
		
		/// <summary>
		/// Category name
		/// </summary>
		public string Name
		{
			get; set;
		}
		
		/// <summary>
		/// Reference to parent category
		/// </summary>
		public InputBindingCategory ParentCategory
		{
			get; set;
		}
		
		/// <summary>
		/// Category path is used to specify hierarchical category position
		/// 
		/// Format:
		/// /category/subcategory 
		/// </summary>
		internal string Path {
			get; set;
		}
	}
}

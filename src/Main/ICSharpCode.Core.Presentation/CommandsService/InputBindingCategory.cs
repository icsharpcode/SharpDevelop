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
			if(path == "/ContextMenus/Tabs")
			{
				
			}
			Path = path;
			Text = text;
		}
		
		/// <summary>
		/// Category name
		/// </summary>
		public string Text
		{
			get; set;
		}
		
		/// <summary>
		/// Reference to parent category
		/// </summary>
		public List<InputBindingCategory> Children
		{
			get
			{
				return CommandManager.GetInputBindingCategoryChildren(Path).ToList();
			}
		}
		
		/// <summary>
		/// Category path is used to specify hierarchical category position
		/// 
		/// Format:
		/// /category/subcategory 
		/// </summary>
		public string Path {
			get; set;
		}
		
		public override string ToString() {
			return Path;
		}
	}
}

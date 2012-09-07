// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using ICSharpCode.SharpDevelop.Internal.Templates;

namespace ICSharpCode.SharpDevelop.Gui
{
	public interface ICategory
	{
		string Name {get; set;}
		int SortOrder {get; set;}
	}
	
	/// <summary>
	/// Sorts categories in the project/file template tree.
	/// </summary>
	public class TemplateCategoryComparer : IComparer
	{		
		public TemplateCategoryComparer()
		{
		}
		
		public int Compare(object x, object y)
		{
			ICategory categoryX = x as ICategory;
			ICategory categoryY = y as ICategory;
			
			if (categoryX.SortOrder != TemplateCategorySortOrderFile.UndefinedSortOrder && categoryY.SortOrder != TemplateCategorySortOrderFile.UndefinedSortOrder) {
				if (categoryX.SortOrder > categoryY.SortOrder) {
					return 1;
				} else if (categoryX.SortOrder < categoryY.SortOrder) {
					return -1;
				}
			} else if (categoryX.SortOrder != TemplateCategorySortOrderFile.UndefinedSortOrder) {
				return -1;
			} else if (categoryY.SortOrder != TemplateCategorySortOrderFile.UndefinedSortOrder) {
				return 1;
			}
			
			return String.Compare(categoryX.Name, categoryY.Name, StringComparison.CurrentCultureIgnoreCase);
		}
	}
}

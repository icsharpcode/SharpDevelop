// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
			
			return String.Compare(categoryX.Name, categoryY.Name);
		}
	}
}

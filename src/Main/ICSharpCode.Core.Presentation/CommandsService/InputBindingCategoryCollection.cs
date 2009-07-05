/*
 * Created by SharpDevelop.
 * User: Administrator
 * Date: 7/4/2009
 * Time: 9:02 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;

namespace ICSharpCode.Core.Presentation
{
	/// <summary>
	/// Description of InputBindingCategoryCollection.
	/// </summary>
	public class InputBindingCategoryCollection : ICollection<InputBindingCategory>
	{
		private List<InputBindingCategory> categories = new List<InputBindingCategory>();
		
		public InputBindingCategoryCollection()
		{
		}
		
		public int Count {
			get {
				return categories.Count;
			}
		}
		
		public bool IsReadOnly {
			get {
				return false;
			}
		}
		
		public void Add(InputBindingCategory category)
		{
			if(category == null) {
				throw new ArgumentException("InputBindingCategory can not be null");
			}
			
			if(!categories.Contains(category)) {
				categories.Add(category);
			}
		}
		
		public void Clear()
		{
			categories.Clear();
		}
		
		public bool Contains(InputBindingCategory category)
		{
			return categories.Contains(category);
		}
		
		public void AddRange(IEnumerable<InputBindingCategory> categories)
		{
			foreach(var category in categories) {
				Add(category);
			}
		}
		
		public void CopyTo(InputBindingCategory[] array, int arrayIndex)
		{
			categories.CopyTo(array, arrayIndex);
		}
		
		public bool Remove(InputBindingCategory category)
		{
			return categories.Remove(category);
		}
		
		public IEnumerator<InputBindingCategory> GetEnumerator()
		{
			return categories.GetEnumerator();
		}
		
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return categories.GetEnumerator();
		}
	}
}

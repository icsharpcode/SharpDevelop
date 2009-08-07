using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;

namespace ICSharpCode.Core.Presentation
{
	/// <summary>
	/// Observable collection containing instances of <see cref="InputBindingCategory" />
	/// </summary>
	public class InputBindingCategoryCollection : IObservableCollection<InputBindingCategory>
	{
		private ObservableCollection<InputBindingCategory> categories = new ObservableCollection<InputBindingCategory>();
		
		/// <summary>
		/// Gets number of elements in this <see cref="InputBindingCategoryCollection" />
		/// </summary>
		public int Count {
			get {
				return categories.Count;
			}
		}
		
		/// <inheritdoc />
		public bool IsReadOnly {
			get {
				return false;
			}
		}
		
		private event NotifyCollectionChangedEventHandler categoriesCollectionChanged;
		
		/// <inheritdoc />
		public event NotifyCollectionChangedEventHandler CollectionChanged
		{
			add {
				categoriesCollectionChanged += value;
				categories.CollectionChanged += value;
			}
			remove {
				categoriesCollectionChanged -= value;
				categories.CollectionChanged -= value;
			}
		}
		
		/// <summary>
		/// Add <see cref="InputBindingCategory" /> to this <see cref="InputBindingCategoryCollection" />
		/// </summary>
		/// <param name="category"></param>
		public void Add(InputBindingCategory category)
		{
			if(category == null) {
				throw new ArgumentException("InputBindingCategory can not be null");
			}
			
			if(!categories.Contains(category)) {
				categories.Add(category);
			}
		}
		
		/// <summary>
		/// Removes all categories from this collection
		/// </summary>
		public void Clear()
		{
			var categoriesBackup = categories;
			
			categories =new ObservableCollection<InputBindingCategory>();
			foreach(NotifyCollectionChangedEventHandler handler in categoriesCollectionChanged.GetInvocationList()) {
				categories.CollectionChanged += handler;
			}
			
			if(categoriesCollectionChanged != null) {
				categoriesCollectionChanged.Invoke(
					this,
					new NotifyCollectionChangedEventArgs(
						NotifyCollectionChangedAction.Remove,
						categoriesBackup));
			}
		}
		
		/// <summary>
		/// Determines whether this collection contains instance of <see cref="InputBindingCategory" />
		/// </summary>
		/// <param name="category">Category to be examined</param>
		/// <returns>Returns <code>true</code> if collection contains provided category; otherwise <code>false</code></returns>
		public bool Contains(InputBindingCategory category)
		{
			return categories.Contains(category);
		}
		
		/// <summary>
		/// Add multiple categories to <see cref="InputBindingCategory" />
		/// </summary>
		/// <param name="categories">Categories to add</param>
		public void AddRange(IEnumerable<InputBindingCategory> categories)
		{
			foreach(var category in categories) {
				Add(category);
			}
		}
		
		/// <inheritdoc /> 
		public void CopyTo(InputBindingCategory[] array, int arrayIndex)
		{
			categories.CopyTo(array, arrayIndex);
		}
		
		/// <summary>
		/// Remove <see cref="InputBindingCategory" /> instance from collection
		/// </summary>
		/// <param name="category">Category to remove</param>
		/// <returns>Returns <code>true</code> if item was removed; otherwise <code>false</code></returns>
		public bool Remove(InputBindingCategory category)
		{
			return categories.Remove(category);
		}
		
		/// <inheritdoc />
		public IEnumerator<InputBindingCategory> GetEnumerator()
		{
			return categories.GetEnumerator();
		}
		
		/// <inheritdoc />
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return categories.GetEnumerator();
		}
	}
}

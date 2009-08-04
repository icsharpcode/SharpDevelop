using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;

namespace ICSharpCode.Core.Presentation
{
	/// <summary>
	/// Description of InputBindingCategoryCollection.
	/// </summary>
	public class InputBindingCategoryCollection : IObservableCollection<InputBindingCategory>
	{
		private ObservableCollection<InputBindingCategory> categories = new ObservableCollection<InputBindingCategory>();
		
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
		
		private event NotifyCollectionChangedEventHandler categoriesCollectionChanged;
		
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

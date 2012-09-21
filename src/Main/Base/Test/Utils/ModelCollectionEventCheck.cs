// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;

namespace ICSharpCode.SharpDevelop.Utils
{
	/// <summary>
	/// Checks the change events of a model collection for consistency
	/// with the collection data.
	/// </summary>
	public class ModelCollectionEventCheck<T>
	{
		readonly IModelCollection<T> modelCollection;
		List<T> list;
		
		public ModelCollectionEventCheck(IModelCollection<T> modelCollection)
		{
			if (modelCollection == null)
				throw new ArgumentNullException("modelCollection");
			this.modelCollection = modelCollection;
			this.list = new List<T>(modelCollection);
			modelCollection.CollectionChanged += OnCollectionChanged;
		}
		
		void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action) {
				case NotifyCollectionChangedAction.Add:
					list.InsertRange(e.NewStartingIndex, e.NewItems.Cast<T>());
					break;
				case NotifyCollectionChangedAction.Remove:
					Assert.AreEqual(list.GetRange(e.OldStartingIndex, e.OldItems.Count), e.OldItems);
					list.RemoveRange(e.OldStartingIndex, e.OldItems.Count);
					break;
				case NotifyCollectionChangedAction.Replace:
					Assert.AreEqual(e.OldStartingIndex, e.NewStartingIndex, "Old and new starting index must be identical for replace action");
					Assert.AreEqual(list.GetRange(e.OldStartingIndex, e.OldItems.Count), e.OldItems);
					list.RemoveRange(e.OldStartingIndex, e.OldItems.Count);
					list.InsertRange(e.NewStartingIndex, e.NewItems.Cast<T>());
					break;
				case NotifyCollectionChangedAction.Move:
					Assert.AreEqual(e.OldItems, e.NewItems);
					Assert.AreEqual(list.GetRange(e.OldStartingIndex, e.OldItems.Count), e.OldItems);
					list.RemoveRange(e.OldStartingIndex, e.OldItems.Count);
					list.InsertRange(e.NewStartingIndex, e.NewItems.Cast<T>());
					break;
				case NotifyCollectionChangedAction.Reset:
					list.Clear();
					list.AddRange(modelCollection);
					break;
				default:
					throw new Exception("Invalid value for NotifyCollectionChangedAction");
			}
			Verify();
		}
		
		public void Verify()
		{
			Assert.AreEqual(list, modelCollection.ToList());
		}
	}
}

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
		
		void OnCollectionChanged(IReadOnlyCollection<T> removedItems, IReadOnlyCollection<T> addedItems)
		{
			foreach (T removed in removedItems) {
				list.Remove(removed);
			}
			foreach (T added in addedItems) {
				list.Add(added);
			}
			Verify();
		}
		
		public void Verify()
		{
			Assert.AreEqual(list, modelCollection.ToList());
		}
	}
}

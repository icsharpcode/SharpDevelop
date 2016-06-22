// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.SharpDevelop.Dom
{
	/// <summary>
	/// A TypeDefinitionModel-collection that holds models for all top-level types in a project content.
	/// </summary>
	sealed class TopLevelTypeDefinitionModelCollection : ITypeDefinitionModelCollection
	{
		readonly ModelCollectionChangedEvent<ITypeDefinitionModel> collectionChangedEvent;
		readonly IEntityModelContext context;
		Dictionary<TopLevelTypeName, TypeDefinitionModel> dict = new Dictionary<TopLevelTypeName, TypeDefinitionModel>();
		
		public TopLevelTypeDefinitionModelCollection(IEntityModelContext context)
		{
			if (context == null)
				throw new ArgumentNullException("context");
			this.context = context;
			collectionChangedEvent = new ModelCollectionChangedEvent<ITypeDefinitionModel>();
		}
		
		public event ModelCollectionChangedEventHandler<ITypeDefinitionModel> CollectionChanged
		{
			add {
				collectionChangedEvent.AddHandler(value);
			}
			remove {
				collectionChangedEvent.RemoveHandler(value);
			}
		}
		
		public IReadOnlyCollection<ITypeDefinitionModel> CreateSnapshot()
		{
			return dict.Values.ToArray();
		}
		
		public int Count {
			get { return dict.Count; }
		}
		
		public ITypeDefinitionModel this[TopLevelTypeName topLevelName] {
			get {
				TypeDefinitionModel model;
				if (dict.TryGetValue(topLevelName, out model))
					return model;
				else
					return null;
			}
		}
		
		public ITypeDefinitionModel this[FullTypeName fullTypeName] {
			get {
				ITypeDefinitionModel model = this[fullTypeName.TopLevelTypeName];
				for (int i = 0; i < fullTypeName.NestingLevel && model != null; i++) {
					string name = fullTypeName.GetNestedTypeName(i);
					int atpc = fullTypeName.GetNestedTypeAdditionalTypeParameterCount(i);
					model = model.GetNestedType(name, atpc);
				}
				return model;
			}
		}
		
		/// <summary>
		/// Updates the parse information.
		/// </summary>
		public void Update(IList<IUnresolvedTypeDefinition> oldFile, IList<IUnresolvedTypeDefinition> newFile)
		{
			List<ITypeDefinitionModel> oldModels = null;
			List<ITypeDefinitionModel> newModels = null;
			bool[] oldTypeDefHandled = null;
			if (oldFile.Count > 0) {
				oldTypeDefHandled = new bool[oldFile.Count];
			}
			foreach (var newPart in newFile) {
				FullTypeName newFullTypeName = newPart.FullTypeName;
				TypeDefinitionModel model;
				if (dict.TryGetValue(newFullTypeName.TopLevelTypeName, out model)) {
					// Existing type changed
					// Find a matching old part:
					IUnresolvedTypeDefinition oldPart = null;
					if (oldTypeDefHandled != null) {
						for (int i = 0; i < oldTypeDefHandled.Length; i++) {
							if (oldTypeDefHandled[i])
								continue;
							if (oldFile[i].FullTypeName == newFullTypeName) {
								oldTypeDefHandled[i] = true;
								oldPart = oldFile[i];
								break;
							}
						}
					}
					model.Update(oldPart, newPart);
				} else {
					// New type added
					model = new TypeDefinitionModel(context, newPart);
					dict.Add(newFullTypeName.TopLevelTypeName, model);
					if (newModels == null)
						newModels = new List<ITypeDefinitionModel>();
					newModels.Add(model);
				}
			}
			// Remove all old parts that weren't updated:
			if (oldTypeDefHandled != null) {
				for (int i = 0; i < oldTypeDefHandled.Length; i++) {
					if (!oldTypeDefHandled[i]) {
						IUnresolvedTypeDefinition oldPart = oldFile[i];
						TopLevelTypeName topLevelTypeName = oldPart.FullTypeName.TopLevelTypeName;
						TypeDefinitionModel model;
						if (dict.TryGetValue(topLevelTypeName, out model)) {
							// Remove the part from the model
							if (model.Parts.Count > 1) {
								model.Update(oldPart, null);
							} else {
								dict.Remove(topLevelTypeName);
								if (oldModels == null)
									oldModels = new List<ITypeDefinitionModel>();
								oldModels.Add(model);
							}
						}
					}
				}
			}
			// Raise the event if necessary:
			if (collectionChangedEvent.ContainsHandlers && (oldModels != null || newModels != null)) {
				IReadOnlyCollection<ITypeDefinitionModel> emptyList = EmptyList<ITypeDefinitionModel>.Instance;
				collectionChangedEvent.Fire(oldModels ?? emptyList, newModels ?? emptyList);
			}
		}
		
		IEnumerator<ITypeDefinitionModel> IEnumerable<ITypeDefinitionModel>.GetEnumerator()
		{
			return dict.Values.GetEnumerator();
		}
		
		IEnumerator IEnumerable.GetEnumerator()
		{
			return dict.Values.GetEnumerator();
		}
	}
}

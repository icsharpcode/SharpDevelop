// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.SharpDevelop.Dom
{
	sealed class NestedTypeDefinitionModelCollection : IModelCollection<ITypeDefinitionModel>
	{
		readonly ModelCollectionChangedEvent<ITypeDefinitionModel> collectionChangedEvent;
		readonly IEntityModelContext context;
		List<TypeDefinitionModel> list = new List<TypeDefinitionModel>();
		
		public NestedTypeDefinitionModelCollection(IEntityModelContext context)
		{
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
			return list.ToArray();
		}
		
		public IEnumerator<ITypeDefinitionModel> GetEnumerator()
		{
			return list.GetEnumerator();
		}
		
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return list.GetEnumerator();
		}
		
		public int Count {
			get { return list.Count; }
		}
		
		public TypeDefinitionModel FindModel(string name, int tpc)
		{
			foreach (var typeDef in list) {
				if (typeDef.Name == name && typeDef.FullTypeName.TypeParameterCount == tpc)
					return typeDef;
			}
			return null;
		}
		
		public void Update(IList<IUnresolvedTypeDefinition> oldTypes, IList<IUnresolvedTypeDefinition> newTypes)
		{
			List<ITypeDefinitionModel> oldModels = null;
			List<ITypeDefinitionModel> newModels = null;
			bool[] oldTypeDefHandled = null;
			if (oldTypes != null) {
				oldTypeDefHandled = new bool[oldTypes.Count];
			}
			if (newTypes != null) {
				foreach (var newPart in newTypes) {
					TypeDefinitionModel model = FindModel(newPart.Name, newPart.TypeParameters.Count);
					if (model != null) {
						// Existing type changed
						// Find a matching old part:
						IUnresolvedTypeDefinition oldPart = null;
						if (oldTypes != null) {
							for (int i = 0; i < oldTypeDefHandled.Length; i++) {
								if (oldTypeDefHandled[i])
									continue;
								if (oldTypes[i].Name == newPart.Name && oldTypes[i].TypeParameters.Count == newPart.TypeParameters.Count) {
									oldTypeDefHandled[i] = true;
									oldPart = oldTypes[i];
									break;
								}
							}
						}
						model.Update(oldPart, newPart);
					} else {
						// New type added
						model = new TypeDefinitionModel(context, newPart);
						list.Add(model);
						if (newModels == null)
							newModels = new List<ITypeDefinitionModel>();
						newModels.Add(model);
					}
				}
			}
			// Remove all old parts that weren't updated:
			if (oldTypes != null) {
				for (int i = 0; i < oldTypeDefHandled.Length; i++) {
					if (!oldTypeDefHandled[i]) {
						IUnresolvedTypeDefinition oldPart = oldTypes[i];
						TypeDefinitionModel model = FindModel(oldPart.Name, oldPart.TypeParameters.Count);
						if (model != null) {
							// Remove the part from the model
							if (model.Parts.Count > 1) {
								model.Update(oldPart, null);
							} else {
								list.Remove(model);
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
	}
}

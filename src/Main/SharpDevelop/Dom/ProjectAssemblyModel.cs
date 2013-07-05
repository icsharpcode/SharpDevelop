// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Parser;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Dom
{
	sealed class ProjectAssemblyModel : IUpdateableAssemblyModel
	{
		IEntityModelContext context;
		TopLevelTypeDefinitionModelCollection typeDeclarations;
		KeyedModelCollection<string, NamespaceModel> namespaces;
		NamespaceModel rootNamespace;
		
		public ProjectAssemblyModel(IEntityModelContext context)
		{
			if (context == null)
				throw new ArgumentNullException("context");
			this.context = context;
			this.rootNamespace = new NamespaceModel(context, null, "");
			this.typeDeclarations = new TopLevelTypeDefinitionModelCollection(context);
			this.typeDeclarations.CollectionChanged += TypeDeclarationsCollectionChanged;
			this.namespaces = new KeyedModelCollection<string, NamespaceModel>(value => value.FullName);
		}
		
		public string Name {
			get {
				return context.Project.AssemblyName;
			}
		}
		
		public ITypeDefinitionModelCollection TopLevelTypeDefinitions {
			get {
				return typeDeclarations;
			}
		}
		
		public IModelCollection<INamespaceModel> Namespaces {
			get {
				return namespaces;
			}
		}
		
		public INamespaceModel RootNamespace {
			get {
				return rootNamespace;
			}
		}
		
		public void Update(IUnresolvedFile oldFile, IUnresolvedFile newFile)
		{
			typeDeclarations.Update(oldFile, newFile);
		}
		
		void TypeDeclarationsCollectionChanged(IReadOnlyCollection<ITypeDefinitionModel> removedItems, IReadOnlyCollection<ITypeDefinitionModel> addedItems)
		{
			NamespaceModel ns;
			foreach (ITypeDefinitionModel addedItem in addedItems) {
				if (!namespaces.TryGetValue(addedItem.Namespace, out ns)) {
					string[] parts = addedItem.Namespace.Split('.');
					int level = 0;
					ns = rootNamespace;
					while (level < parts.Length) {
						var nextNS = ns.ChildNamespaces
							.FirstOrDefault(n => n.Name == parts[level]);
						if (nextNS == null) break;
						ns = nextNS;
						level++;
					}
					while (level < parts.Length) {
						var child = new NamespaceModel(context, ns, parts[level]);
						ns.ChildNamespaces.Add(child);
						ns = child;
						level++;
					}
					if (!namespaces.Contains(ns))
						namespaces.Add(ns);
					ns.Types.Add(addedItem);
				}
			}
			foreach (ITypeDefinitionModel removedItem in removedItems) {
				if (namespaces.TryGetValue(removedItem.Namespace, out ns)) {
					ns.Types.Remove(removedItem);
					if (ns.Types.Count == 0)
						namespaces.Remove(ns);
					while (ns.ParentNamespace != null) {
						if (ns.ChildNamespaces.Count == 0 && ns.Types.Count == 0)
							((NamespaceModel)ns.ParentNamespace).ChildNamespaces.Remove(ns);
						ns = (NamespaceModel)ns.ParentNamespace;
					}
				}
			}
		}
	}
}



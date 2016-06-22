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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Parser;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Dom
{
	sealed class AssemblyModel : IUpdateableAssemblyModel
	{
		IEntityModelContext context;
		TopLevelTypeDefinitionModelCollection typeDeclarations;
		KeyedModelCollection<string, NamespaceModel> namespaces;
		NamespaceModel rootNamespace;
		AssemblyReferencesModel referencesModel;
		
		public AssemblyModel(IEntityModelContext context)
		{
			if (context == null)
				throw new ArgumentNullException("context");
			this.context = context;
			this.rootNamespace = new NamespaceModel(context.Project, null, "");
			this.typeDeclarations = new TopLevelTypeDefinitionModelCollection(context);
			this.typeDeclarations.CollectionChanged += TypeDeclarationsCollectionChanged;
			this.namespaces = new KeyedModelCollection<string, NamespaceModel>(value => value.FullName);
			this.referencesModel = new AssemblyReferencesModel(this);
		}
		
		public string AssemblyName { get; set; }
		public string FullAssemblyName { get; set; }
		
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
		
		public IAssemblyReferencesModel ReferencesModel {
			get {
				return referencesModel;
			}
		}
		
		public INamespaceModel RootNamespace {
			get {
				return rootNamespace;
			}
		}
		
		public IEntityModelContext Context {
			get {
				return context;
			}
		}
		
		public FileName Location {
			get {
				if (context != null && !string.IsNullOrEmpty(context.Location)) {
					return new FileName(context.Location);
				}
				return null;
			}
		}
		
		public void Update(IUnresolvedFile oldFile, IUnresolvedFile newFile)
		{
			IList<IUnresolvedTypeDefinition> old = EmptyList<IUnresolvedTypeDefinition>.Instance;
			IList<IUnresolvedTypeDefinition> @new = EmptyList<IUnresolvedTypeDefinition>.Instance;
			if (oldFile != null)
				old = oldFile.TopLevelTypeDefinitions;
			if (newFile != null)
				@new = newFile.TopLevelTypeDefinitions;
			
			typeDeclarations.Update(old, @new);
		}
		
		public void Update(IList<IUnresolvedTypeDefinition> oldFile, IList<IUnresolvedTypeDefinition> newFile)
		{
			typeDeclarations.Update(oldFile, newFile);
		}
		
		public void UpdateReferences(IReadOnlyList<DomAssemblyName> references)
		{
			referencesModel.Update(references);
		}
		
		void TypeDeclarationsCollectionChanged(IReadOnlyCollection<ITypeDefinitionModel> removedItems, IReadOnlyCollection<ITypeDefinitionModel> addedItems)
		{
			List<IDisposable> batchList = new List<IDisposable>();
			try {
				NamespaceModel ns;
				foreach (ITypeDefinitionModel addedItem in addedItems) {
					if (!namespaces.TryGetValue(addedItem.Namespace, out ns)) {
						string[] parts = addedItem.Namespace.Split(new[] {'.'}, StringSplitOptions.RemoveEmptyEntries);
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
							var child = new NamespaceModel(context.Project, ns, parts[level]);
							batchList.AddIfNotNull(ns.ChildNamespaces.BatchUpdate());
							ns.ChildNamespaces.Add(child);
							ns = child;
							level++;
						}
						if (!namespaces.Contains(ns)) {
							batchList.AddIfNotNull(namespaces.BatchUpdate());
							namespaces.Add(ns);
						}
					}
					batchList.AddIfNotNull(ns.Types.BatchUpdate());
					ns.Types.Add(addedItem);
				}
				foreach (ITypeDefinitionModel removedItem in removedItems) {
					if (namespaces.TryGetValue(removedItem.Namespace, out ns)) {
						batchList.AddIfNotNull(ns.Types.BatchUpdate());
						ns.Types.Remove(removedItem);
						if (ns.Types.Count == 0) {
							batchList.AddIfNotNull(namespaces.BatchUpdate());
							namespaces.Remove(ns);
						}
						while (ns.ParentNamespace != null) {
							var p = ((NamespaceModel)ns.ParentNamespace);
							if (ns.ChildNamespaces.Count == 0 && ns.Types.Count == 0) {
								batchList.AddIfNotNull(p.ChildNamespaces.BatchUpdate());
								p.ChildNamespaces.Remove(ns);
							}
							ns = p;
						}
					}
				}
			} finally {
				batchList.Reverse();
				foreach (IDisposable d in batchList)
					d.Dispose();
			}
		}
		
		public IAssemblyReferencesModel References {
			get { return referencesModel; }
		}
	}	
}

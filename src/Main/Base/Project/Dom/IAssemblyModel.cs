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
using System.Linq;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Parser;

namespace ICSharpCode.SharpDevelop.Dom
{
	/// <summary>
	/// Represents an assembly.
	/// </summary>
	public interface IAssemblyModel
	{
		/// <summary>
		/// Gets the assembly name (short name).
		/// </summary>
		string AssemblyName { get; }
		
		/// <summary>
		/// Gets the full assembly name (including public key token etc.)
		/// </summary>
		string FullAssemblyName { get; }
		
		/// <summary>
		/// Gets a collection of all top-level type definitions.
		/// Top-level means it does not contain nested classes (but classes from nested namespaces!).
		/// </summary>
		ITypeDefinitionModelCollection TopLevelTypeDefinitions { get; }
		
		/// <summary>
		/// Gets a flat list of all namespaces that contain types.
		/// </summary>
		/// <remarks>
		/// If we have N1.T1 and N1.N2.T2, returns { N1, N1.N2 }.
		/// If we only have N1.N2.T2 returns { N1.N2 }.
		/// It may return the namespace without a name, thus it is only empty
		/// if there are no type definitions in the assembly.
		/// </remarks>
		IModelCollection<INamespaceModel> Namespaces { get; }
		
		/// <summary>
		/// Gets the root namespace for this assembly.
		/// </summary>
		/// <remarks>
		/// This always is the namespace without a name - it's unrelated to the 'root namespace' project setting.
		/// </remarks>
		INamespaceModel RootNamespace { get; }
		
		/// <summary>
		/// Gets the <see cref="IEntityModelContext"/> of this assembly model.
		/// </summary>
		IEntityModelContext Context { get; }
		
		/// <summary>
		/// Returns the location of the assembly represented by this model.
		/// </summary>
		FileName Location { get; }
		
		/// <summary>
		/// Returns the assembly references.
		/// </summary>
		IAssemblyReferencesModel References { get; }
	}
	
	/// <summary>
	/// Represents an assembly that can be updated.
	/// </summary>
	public interface IUpdateableAssemblyModel : IAssemblyModel
	{
		/// <summary>
		/// Updates the parse information with the given file.
		/// </summary>
		/// <remarks>
		/// <paramref name="oldFile"/> is null if the file is newly added to the assemly.
		/// <paramref name="newFile"/> is null if the file is removed from the assembly.
		/// </remarks>
		void Update(IUnresolvedFile oldFile, IUnresolvedFile newFile);
		
		/// <summary>
		/// Updates the parse information with the given list of top-level type definitions.
		/// </summary>
		void Update(IList<IUnresolvedTypeDefinition> oldFile, IList<IUnresolvedTypeDefinition> newFile);
		
		/// <summary>
		/// Updates references of this assembly.
		/// </summary>
		/// <param name="references">Names of referenced assemblies</param>
		void UpdateReferences(IReadOnlyList<DomAssemblyName> references);
		
		/// <summary>
		/// Gets the assembly name (short name).
		/// </summary>
		new string AssemblyName { get; set; }
		
		/// <summary>
		/// Gets the full assembly name (including public key token etc.)
		/// </summary>
		new string FullAssemblyName { get; set; }
	}
	
	public sealed class EmptyAssemblyModel : IAssemblyModel
	{
		public readonly static IAssemblyModel Instance = new EmptyAssemblyModel();

		EmptyAssemblyModel()
		{
		}
		
		public string AssemblyName {
			get { return string.Empty; }
		}
		
		public string FullAssemblyName {
			get { return string.Empty; }
		}
		
		public ITypeDefinitionModelCollection TopLevelTypeDefinitions {
			get { return EmptyTypeDefinitionModelCollection.Instance; }
		}

		public IModelCollection<INamespaceModel> Namespaces {
			get { return ImmutableModelCollection<INamespaceModel>.Empty; }
		}

		public INamespaceModel RootNamespace {
			get { return EmptyNamespaceModel.Instance; }
		}

		public IEntityModelContext Context {
			get {
				return null;
			}
		}
		
		public FileName Location {
			get {
				return null;
			}
		}
		
		public IAssemblyReferencesModel References {
			get { return EmptyAssemblyReferencesModel.Instance; }
		}
	}
}

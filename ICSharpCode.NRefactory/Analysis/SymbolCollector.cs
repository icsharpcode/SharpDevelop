// Copyright (c) 2013 AlphaSierraPapa for the SharpDevelop Team
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
using ICSharpCode.NRefactory.TypeSystem;
using System.Linq;

namespace ICSharpCode.NRefactory.Analysis
{
	/// <summary>
	/// The symbol collector collects related symbols that form a group of symbols that should be renamed
	/// when a name of one symbol changes. For example if a type definition name should be changed
	/// the constructors and destructor names should change as well.
	/// </summary>
	public class SymbolCollector
	{
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="ICSharpCode.NRefactory.Analysis.SymbolCollector"/> should include overloads.
		/// </summary>
		/// <value><c>true</c> if overloads should be included; otherwise, <c>false</c>.</value>
		public bool IncludeOverloads { 
			get;
			set;
		}

		public bool GroupForRenaming {
			get;
			set;
		}

		static IEnumerable<ISymbol> CollectTypeRelatedMembers (ITypeDefinition type)
		{
			yield return type;
			foreach (var c in type.GetDefinition ().GetMembers (m => !m.IsSynthetic && (m.SymbolKind == SymbolKind.Constructor || m.SymbolKind == SymbolKind.Destructor), GetMemberOptions.IgnoreInheritedMembers)) {
				yield return c;
			}
		}

		static IEnumerable<ISymbol> CollectOverloads (TypeGraph g, IMethod method)
		{
			return method.DeclaringType
				.GetMethods (m => m.Name == method.Name)
				.Where (m => m != method);
		}

		static IMember SearchMember (ITypeDefinition derivedType, IMember method)
		{
			foreach (var m in derivedType.Members) {
				if (m.ImplementedInterfaceMembers.Contains (method))
					return m;
			}
			return null;
		}

		static IEnumerable<ISymbol> MakeUnique (List<ISymbol> symbols)
		{
			HashSet<ISymbol> taken = new HashSet<ISymbol> ();
			foreach (var sym in symbols) {
				if (taken.Contains (sym))
					continue;
				taken.Add (sym);
				yield return sym;
			}
		}

		/// <summary>
		/// Gets the related symbols.
		/// </summary>
		/// <returns>The related symbols.</returns>
		/// <param name="g">The type graph.</param>
		/// <param name="m">The symbol to search</param>
		/// <param name="includeOverloads">If set to <c>true</c> overloads are included in the rename.</param>
		public IEnumerable<ISymbol> GetRelatedSymbols(TypeGraph g, ISymbol m)
		{
			switch (m.SymbolKind) {
			case SymbolKind.TypeDefinition:
				return CollectTypeRelatedMembers ((ITypeDefinition)m);
	
			case SymbolKind.Field:
			case SymbolKind.Operator:
			case SymbolKind.Variable:
			case SymbolKind.Parameter:
			case SymbolKind.TypeParameter:
				return new ISymbol[] { m };
	
			case SymbolKind.Constructor:
				if (GroupForRenaming)
					return GetRelatedSymbols (g, ((IMethod)m).DeclaringTypeDefinition);
				List<ISymbol> constructorSymbols = new List<ISymbol> ();
				if (IncludeOverloads) {
					foreach (var m3 in CollectOverloads (g, (IMethod)m)) {
						constructorSymbols.Add (m3);
					}
				}
				return constructorSymbols;

			case SymbolKind.Destructor:
				if (GroupForRenaming)
					return GetRelatedSymbols (g, ((IMethod)m).DeclaringTypeDefinition);
				return new ISymbol[] { m };

			case SymbolKind.Indexer:
			case SymbolKind.Event:
			case SymbolKind.Property:
				return new ISymbol[] { m };

			case SymbolKind.Method:
				var method = (IMethod)m;
				List<ISymbol> symbols = new List<ISymbol> ();
				if (method.ImplementedInterfaceMembers.Count > 0) {
					foreach (var m2 in method.ImplementedInterfaceMembers) {
						symbols.AddRange (GetRelatedSymbols (g, m2));
					}
				} else {
					symbols.Add (method);
				}

				if (method.DeclaringType.Kind == TypeKind.Interface) {
					foreach (var derivedType in g.GetNode (method.DeclaringTypeDefinition).DerivedTypes) {
						var member = SearchMember (derivedType.TypeDefinition, method);
						if (member != null)
							symbols.Add (member);
					}
				}


				if (IncludeOverloads) {
					IncludeOverloads = false;
					foreach (var m3 in CollectOverloads (g, method)) {
						symbols.AddRange (GetRelatedSymbols (g, m3));
					}
				}
				return MakeUnique (symbols);
			
			case SymbolKind.Namespace:
				// TODO?
				return new ISymbol[] { m };
			default:
				throw new ArgumentOutOfRangeException ("symbol:"+m.SymbolKind);
			}
		}
	}
}

// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Threading.Tasks;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Editor.Search;
using ICSharpCode.SharpDevelop.Refactoring;

namespace ICSharpCode.XamlBinding
{
	/// <summary>
	/// Description of XamlSymbolSearch.
	/// </summary>
	public class XamlSymbolSearch : ISymbolSearch
	{
		public XamlSymbolSearch(IEntity entity)
		{
		}
		
		public double WorkAmount {
			get {
				throw new NotImplementedException();
			}
		}
		
		public Task FindReferencesAsync(SymbolSearchArgs searchArguments, Action<SearchedFile> callback)
		{
			throw new NotImplementedException();
		}
	}
}

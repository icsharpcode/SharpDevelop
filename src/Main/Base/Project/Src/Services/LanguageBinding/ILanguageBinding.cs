// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Refactoring;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// Provides access to language specific features (independent of files).
	/// </summary>
	public interface ILanguageBinding : IServiceProvider
	{
		/// <summary>
		/// Provides access to the formatting strategy for this language.
		/// </summary>
		IFormattingStrategy FormattingStrategy { 
			get;
		}
		
		/// <summary>
		/// Provides access to the bracket search logic for this language.
		/// </summary>
		IBracketSearcher BracketSearcher {
			get;
		}
		
		/// <summary>
		/// Provides access to the code generator for this language.
		/// </summary>
		CodeGenerator CodeGenerator {
			get;
		}
		
		/// <summary>
		/// Provides access to the <see cref="System.CodeDom.Compiler.CodeDomProvider" /> for this language.
		/// Can be null, if not available.
		/// </summary>
		System.CodeDom.Compiler.CodeDomProvider CodeDomProvider {
			get;
		}
	}
}

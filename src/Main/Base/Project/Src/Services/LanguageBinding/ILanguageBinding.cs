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
using System.Linq;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
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
		
		/// <summary>
		/// Creates a completion binding for a given expression and context.
		/// </summary>
		ICodeCompletionBinding CreateCompletionBinding(string expressionToComplete, ICodeContext context);
	}
}

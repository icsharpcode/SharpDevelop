// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Refactoring;

namespace ICSharpCode.SharpDevelop
{
	public class DefaultLanguageBinding : ILanguageBinding
	{
		public static readonly DefaultLanguageBinding DefaultInstance = new DefaultLanguageBinding();
		
		public virtual IFormattingStrategy FormattingStrategy {
			get {
				return DefaultFormattingStrategy.DefaultInstance;
			}
		}
		
		public virtual IBracketSearcher BracketSearcher {
			get {
				return DefaultBracketSearcher.DefaultInstance;
			}
		}
		
		public virtual ICodeGenerator CodeGenerator {
			get {
				return DefaultCodeGenerator.DefaultInstance;
			}
		}
		
		public virtual System.CodeDom.Compiler.CodeDomProvider CodeDomProvider {
			get {
				return null;
			}
		}
	}
}

// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Refactoring;
using CSharpBinding.FormattingStrategy;
using CSharpBinding.Refactoring;

namespace CSharpBinding
{
	/// <summary>
	/// Description of CSharpLanguageBinding.
	/// </summary>
	public class CSharpLanguageBinding : DefaultLanguageBinding
	{
		public override string Name {
			get { return "C#"; }
		}
		
		public override StringComparer IdentifierComparer {
			get { return StringComparer.Ordinal; }
		}
		
		public override IFormattingStrategy FormattingStrategy {
			get { return new CSharpFormattingStrategy(); }
		}
		
		public override IBracketSearcher BracketSearcher {
			get { return new CSharpBracketSearcher(); }
		}
		
		public override ICodeGenerator CodeGenerator {
			get { return new CSharpCodeGenerator(); }
		}
	}
}

// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.VBNetBinding
{
	/// <summary>
	/// Description of VBNetLanguageBinding.
	/// </summary>
	public class VBNetLanguageBinding : DefaultLanguageBinding
	{
		public override IFormattingStrategy FormattingStrategy {
			get { return new VBNetFormattingStrategy(); }
		}
		
		public override IBracketSearcher BracketSearcher {
			get { return new VBNetBracketSearcher(); }
		}
		
		public override LanguageProperties Properties {
			get { return LanguageProperties.VBNet; }
		}
	}
}

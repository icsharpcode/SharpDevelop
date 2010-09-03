// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.SharpDevelop
{
	public class DefaultLanguageBinding : ILanguageBinding
	{
		public virtual IFormattingStrategy FormattingStrategy {
			get {
				return null;
			}
		}
		
		public virtual LanguageProperties Properties {
			get {
				return null;
			}
		}
		
		public virtual void Attach(ITextEditor editor)
		{
		}
		
		public virtual void Detach()
		{
		}
		
		public virtual IBracketSearcher BracketSearcher {
			get {
				return null;
			}
		}
	}
}

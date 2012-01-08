// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel.Design;
using CSharpBinding.FormattingStrategy;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;

namespace CSharpBinding
{
	/// <summary>
	/// Description of CSharpLanguageBinding.
	/// </summary>
	public class CSharpLanguageBinding : DefaultLanguageBinding
	{
		public override IFormattingStrategy FormattingStrategy {
			get { return new CSharpFormattingStrategy(); }
		}
		
		public override LanguageProperties Properties {
			get { return LanguageProperties.CSharp; }
		}
		
		public override IBracketSearcher BracketSearcher {
			get { return new CSharpBracketSearcher(); }
		}
		
		CodeManipulation codeManipulation;
		
		public override void Attach(ITextEditor editor)
		{
			codeManipulation = new CodeManipulation(editor);
		}
		
		public override void Detach()
		{
			codeManipulation.Dispose();
		}
	}
}

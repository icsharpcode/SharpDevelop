// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="siegfriedpammer@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop.Editor;
using System;
using ICSharpCode.SharpDevelop;

namespace VBNetBinding
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
	}
}

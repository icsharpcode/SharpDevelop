// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="siegfriedpammer@gmail.com" />
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.CppBinding
{
	/// <summary>
	/// Formatting strategy for C++ (only implements insertion of comment tags).
	/// </summary>
	public class CppFormattingStrategy : DefaultFormattingStrategy
	{
		public CppFormattingStrategy()
		{
		}
		
		public override void SurroundSelectionWithComment(ITextEditor editor)
		{
			SurroundSelectionWithSingleLineComment(editor, "//");
		}
	}
}

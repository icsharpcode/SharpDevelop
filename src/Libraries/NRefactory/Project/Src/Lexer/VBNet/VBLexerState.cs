// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="siegfriedpammer@gmail.com" />
//     <version>$Revision: 6083 $</version>
// </file>

using System;
using System.Collections.Generic;

namespace ICSharpCode.NRefactory.Parser.VB
{
	public sealed class VBLexerState : AbstractLexerState
	{
		public bool LineEnd { get; set; }
		public bool IsAtLineBegin { get; set; }
		public bool MisreadExclamationMarkAsTypeCharacter { get; set; }
		public bool EncounteredLineContinuation { get; set; }
		public ExpressionFinderState ExpressionFinder { get; set; }
		public Stack<XmlModeInfo> XmlModeInfoStack { get; set; }
		public bool InXmlMode { get; set; }
	}
}

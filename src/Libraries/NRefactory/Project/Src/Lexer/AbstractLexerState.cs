// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="siegfriedpammer@gmail.com" />
//     <version>$Revision$</version>
// </file>
using System;

namespace ICSharpCode.NRefactory.Parser
{
	public abstract class AbstractLexerState
	{
		public int Line { get; set; }
		public int Column { get; set; }
		public int PrevTokenKind { get; set; }
	}
}

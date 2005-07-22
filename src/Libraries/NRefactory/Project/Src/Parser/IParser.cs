// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.NRefactory.Parser.AST;

namespace ICSharpCode.NRefactory.Parser
{
	/// <summary>
	/// Description of IParser.
	/// </summary>
	public interface IParser : IDisposable
	{
		Errors Errors {
			get;
		}
		
		ILexer Lexer {
			get;
		}
		
		CompilationUnit CompilationUnit {
			get;
		}
		
		bool ParseMethodBodies {
			get; set;
		}
		
		void Parse();
		
		Expression ParseExpression();
	}
}

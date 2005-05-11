/*
 * Created by SharpDevelop.
 * User: Omnibrain
 * Date: 14.09.2004
 * Time: 08:44
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

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

/*
 * Created by SharpDevelop.
 * User: Omnibrain
 * Date: 14.09.2004
 * Time: 08:44
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections;

namespace ICSharpCode.NRefactory.Parser
{
	/// <summary>
	/// Description of IParser.
	/// </summary>
	public interface ILexer : IDisposable
	{
		Errors Errors {
			get;
		}
		
		/// <summary>
		/// The current Token. <seealso cref="ICSharpCode.NRefactory.Parser.Token"/>
		/// </summary>
		Token Token {
			get;
		}
		
		/// <summary>
		/// The next Token (The <see cref="Token"/> after <see cref="NextToken"/> call) . <seealso cref="ICSharpCode.NRefactory.Parser.Token"/>
		/// </summary>
		Token LookAhead {
			get;
		}
		
		/// <summary>
		/// Special comment tags are tags like TODO, HACK or UNDONE which are read by the lexer and stored in <see cref="TagComments"/>.
		/// </summary>
		string[] SpecialCommentTags {
			get;
			set;
		}
		
		/// <summary>
		/// Returns the comments that had been read and containing tag key words.
		/// </summary>
		ArrayList TagComments {
			get;
		}
		
		SpecialTracker SpecialTracker {
			get;
		}
		
		void StartPeek();
		
		/// <summary>
		/// Gives back the next token. A second call to Peek() gives the next token after the last call for Peek() and so on.
		/// </summary>
		/// <returns>An <see cref="Token"/> object.</returns>
		Token Peek();
		
		/// <summary>
		/// Reads the next token and gives it back.
		/// </summary>
		/// <returns>An <see cref="Token"/> object.</returns>
		Token NextToken();
	}
}

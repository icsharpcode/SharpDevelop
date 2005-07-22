// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;

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
		List<TagComment> TagComments {
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
		
		/// <summary>
		/// Skips to the end of the current code block.
		/// For this, the lexer must have read the next token AFTER the token opening the
		/// block (so that Lexer.Token is the block-opening token, not Lexer.LookAhead).
		/// After the call, Lexer.LookAhead will be the block-closing token.
		/// </summary>
		void SkipCurrentBlock();
	}
}

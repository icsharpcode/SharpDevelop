/*
 * Created by SharpDevelop.
 * User: Omnibrain
 * Date: 15.09.2004
 * Time: 10:48
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Text;
using System.Collections;
using System.Diagnostics;

using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Parser.VB;
using ICSharpCode.NRefactory.Parser.AST;

namespace ICSharpCode.NRefactory.PrettyPrinter
{
	/// <summary>
	/// Description of VBNetOutputFormatter.
	/// </summary>
	public sealed class VBNetOutputFormatter : AbstractOutputFormatter
	{
		public VBNetOutputFormatter(VBNetPrettyPrintOptions prettyPrintOptions) : base(prettyPrintOptions)
		{
		}
		
		public override void PrintToken(int token)
		{
			PrintToken(Tokens.GetTokenString(token));
		}
		
		public override void PrintComment(Comment comment)
		{
			switch (comment.CommentType) {
				case CommentType.Block:
					WriteInPreviousLine("'" + comment.CommentText.Replace("\n", "\n'"));
					break;
				case CommentType.Documentation:
					WriteInPreviousLine("'''" + comment.CommentText);
					break;
				default:
					WriteInPreviousLine("'" + comment.CommentText);
					break;
			}
		}
	}
}

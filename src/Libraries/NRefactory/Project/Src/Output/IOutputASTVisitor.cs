/*
 * Created by SharpDevelop.
 * User: Omnibrain
 * Date: 14.09.2004
 * Time: 21:15
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Text;
using System.Collections;
using System.Diagnostics;

using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Parser.CSharp;
using ICSharpCode.NRefactory.Parser.AST;

namespace ICSharpCode.NRefactory.PrettyPrinter
{
	/// <summary>
	/// Description of IOutputASTVisitor.
	/// </summary>
	public interface IOutputASTVisitor : IASTVisitor
	{
		string Text {
			get;
		}
		
		Errors Errors {
			get;
		}
		
		object Options {
			get;
			set;
		}
	}
}

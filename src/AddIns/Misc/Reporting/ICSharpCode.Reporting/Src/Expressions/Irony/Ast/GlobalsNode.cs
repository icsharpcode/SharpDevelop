// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Globalization;
using System.Linq;
using ICSharpCode.Reporting.PageBuilder.ExportColumns;
using Irony.Ast;
using Irony.Interpreter;
using Irony.Interpreter.Ast;
using Irony.Parsing;

namespace ICSharpCode.Reporting.Expressions.Irony.Ast
{
	/// <summary>
	/// Description of GlobalsNode.
	/// </summary>
	public class GlobalsNode: AstNode
	{
		AstNode globalNode;
		
		public override void Init(AstContext context, ParseTreeNode treeNode)
		{
			base.Init(context, treeNode);
			var nodes = treeNode.GetMappedChildNodes();
			globalNode = AddChild("Args", nodes[2]);
		}
		
			
		protected override object DoEvaluate(ScriptThread thread)
		{
			thread.CurrentNode = this;  //standard prolog
			var pi = thread.GetPageInfo();
			
			var test = globalNode.AsString.ToLower();
			if (test.Contains(":")) {
				Console.WriteLine("");
			}
			if ( test == "pagenumber") {
				return pi.PageNumber;
			} else if (test == "pages") {
				return pi.TotalPages;
			} else if (test == "reportname") {
				return pi.ReportName;
			} else if (test == "reportfolder") {
				return pi.ReportFolder;
			} else if (test == "reportfilename") {
				return pi.ReportFileName;
			} 
			
			else {
				return String.Format(CultureInfo.CurrentCulture,"Syntaxerror in Globals <{0}>",globalNode.AsString);
			}
		}
	}
}

// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
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
				return String.Format("Syntaxerror in Globals <{0}>",globalNode.AsString);
			}
		}
	}
}

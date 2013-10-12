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
		}
		
		public override void DoSetValue(ScriptThread thread, object value)
		{
			base.DoSetValue(thread, value);
		}
	}
}

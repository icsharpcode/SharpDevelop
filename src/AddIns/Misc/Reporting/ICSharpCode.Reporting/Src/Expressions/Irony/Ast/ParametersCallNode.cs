// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
using Irony;
using Irony.Ast;
using Irony.Interpreter;
using Irony.Interpreter.Ast;
using Irony.Parsing;

namespace ICSharpCode.Reporting.Expressions.Irony.Ast
{
	/// <summary>
	/// Description of ParametersCallNode.
	/// </summary>
	public class ParametersCallNode: AstNode
	{
		AstNode parameterNode;
		public ParametersCallNode()
		{
		}
		
		public override void Init(AstContext context, ParseTreeNode treeNode)
		{
			base.Init(context, treeNode);
			var nodes = treeNode.GetMappedChildNodes();
			parameterNode = AddChild("Args", nodes[2]);
		}
		
		protected override object DoEvaluate(ScriptThread thread)
		{
			 thread.CurrentNode = this;  //standard prolog
			 
			 var s = thread.App.Globals["param1"];
			return base.DoEvaluate(thread);
		}
	}
}

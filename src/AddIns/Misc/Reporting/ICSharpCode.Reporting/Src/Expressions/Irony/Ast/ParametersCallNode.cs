// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
using ICSharpCode.Reporting.BaseClasses;
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
			BasicParameter result = null;
			 thread.CurrentNode = this;  //standard prolog
			 var dictionary = (ParameterCollection)thread.App.Globals["parameters"];
			 if (dictionary != null) {
			 		result = dictionary.Find(parameterNode.AsString);
			 }
			 return result.ParameterValue;
		}
	}
}

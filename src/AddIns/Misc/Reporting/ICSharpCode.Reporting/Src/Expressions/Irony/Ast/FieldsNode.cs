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
	/// Description of FieldsNode.
	/// </summary>
	public class FieldsNode: AstNode
	{
		AstNode fieldNode;
		
		public override void Init(AstContext context,ParseTreeNode treeNode)
		{
			base.Init(context, treeNode);
			var nodes = treeNode.GetMappedChildNodes();
			fieldNode = AddChild("Args", nodes[2]);
		}
		
		
		protected override object DoEvaluate(ScriptThread thread)
		{
			thread.CurrentNode = this;  //standard prolog
			var container = thread.GetCurrentContainer();
			var column = (ExportText)container.ExportedItems.Where(x => x.Name == fieldNode.AsString).FirstOrDefault();
			return	column.Text;
		}
	}
}

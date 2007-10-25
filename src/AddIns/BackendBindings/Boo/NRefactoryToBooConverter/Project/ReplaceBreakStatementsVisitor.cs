// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using Boo.Lang.Compiler.Ast;

namespace NRefactoryToBooConverter
{
	public class ReplaceBreakStatementsVisitor : DepthFirstVisitor
	{
		string label;
		
		public ReplaceBreakStatementsVisitor(string label)
		{
			this.label = label;
		}
		
		public override void OnForStatement(ForStatement node) { }
		
		public override void OnWhileStatement(WhileStatement node) { }
		
		public override void OnUnlessStatement(UnlessStatement node) { }
		
		public override void OnBreakStatement(BreakStatement node)
		{
			GotoStatement gotoStatement = new GotoStatement(node.LexicalInfo);
			gotoStatement.Label = new ReferenceExpression(node.LexicalInfo, label);
			node.ReplaceBy(gotoStatement);
		}
	}
}

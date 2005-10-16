// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.Ast.Visitors;

namespace NRefactoryToBooConverter
{
	public class ReplaceBreakStatementsVisitor : DepthFirstVisitor
	{
		string label;
		
		public ReplaceBreakStatementsVisitor(string label)
		{
			this.label = label;
		}
		
		public override void OnGivenStatement(GivenStatement node) { }
		
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

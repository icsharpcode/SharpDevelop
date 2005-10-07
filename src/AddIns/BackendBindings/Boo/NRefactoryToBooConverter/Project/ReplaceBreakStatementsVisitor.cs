#region license
// Copyright (c) 2005, Daniel Grunwald (daniel@danielgrunwald.de)
// All rights reserved.
//
// NRefactoryToBoo is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// NRefactoryToBoo is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with NRefactoryToBoo; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
#endregion

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

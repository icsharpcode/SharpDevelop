// CompilationUnit.cs
// Copyright (C) 2003 Mike Krueger (mike@icsharpcode.net)
// 
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.

using System;
using System.Threading;
using System.Diagnostics;
using System.Collections;

namespace ICSharpCode.NRefactory.Parser.AST
{
	public class CompilationUnit : AbstractNode
	{
		// TODO: check if the LookUpTable here and the specials are still needed
		// TODO: Are the childs still needed? Or should we put them in there own fields?
		// Childs in C#: UsingAliasDeclaration, UsingDeclaration, AttributeSection, NamespaceDeclaration
		// Childs in VB: OptionStatements, ImportsStatement, AttributeSection, NamespaceDeclaration
		
		Stack blockStack = new Stack();
		INode lastChild = null;
		ArrayList lookUpTable = new ArrayList(); // [VariableDeclaration]
		
		public CompilationUnit()
		{
			blockStack.Push(this);
		}
		
		public void BlockStart(INode block)
		{
			blockStack.Push(block);
		}
		
		public void BlockEnd()
		{
			lastChild = (INode)blockStack.Pop();
		}
		
		public INode TakeBlock()
		{
			return (INode)blockStack.Pop();
		}
		
		public override void AddChild(INode childNode)
		{
			if (childNode != null) {
				INode parent = (INode)blockStack.Peek();
				parent.Children.Add(childNode);
				childNode.Parent = parent;
				lastChild = childNode;
				if (childNode is LocalVariableDeclaration) {
					AddToLookUpTable((LocalVariableDeclaration)childNode);
				}
			}
		}
		
		public void AddToLookUpTable(LocalVariableDeclaration v)
		{
			v.Block = (BlockStatement)blockStack.Peek();
			lookUpTable.Add(v);
		}
					
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		
		public override string ToString()
		{
			return String.Format("[CompilationUnit]");
		}
	}
}

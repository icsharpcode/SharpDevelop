// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Threading;
using System.Diagnostics;
using System.Collections;

namespace ICSharpCode.NRefactory.Parser.AST
{
	public class CompilationUnit : AbstractNode
	{
		// Children in C#: UsingAliasDeclaration, UsingDeclaration, AttributeSection, NamespaceDeclaration
		// Children in VB: OptionStatements, ImportsStatement, AttributeSection, NamespaceDeclaration
		
		Stack blockStack = new Stack();
		
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
			blockStack.Pop();
		}
		
		public override void AddChild(INode childNode)
		{
			if (childNode != null) {
				INode parent = (INode)blockStack.Peek();
				parent.Children.Add(childNode);
				childNode.Parent = parent;
			}
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

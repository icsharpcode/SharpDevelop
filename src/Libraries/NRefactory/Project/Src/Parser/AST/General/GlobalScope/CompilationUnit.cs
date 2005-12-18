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

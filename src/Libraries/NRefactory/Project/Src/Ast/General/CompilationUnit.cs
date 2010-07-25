// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;

namespace ICSharpCode.NRefactory.Ast
{
	public class CompilationUnit : AbstractNode
	{
		// Children in C#: UsingAliasDeclaration, UsingDeclaration, AttributeSection, NamespaceDeclaration
		// Children in VB: OptionStatements, ImportsStatement, AttributeSection, NamespaceDeclaration
		
		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitCompilationUnit(this, data);
		}
		
		public override string ToString()
		{
			return String.Format("[CompilationUnit]");
		}
	}
}

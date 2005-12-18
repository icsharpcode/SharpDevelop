// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.Collections;

namespace ICSharpCode.NRefactory.Parser.AST
{
	public class NamespaceDeclaration : AbstractNode
	{
		string    name;
		
		// Children in C#: UsingAliasDeclaration, UsingDeclaration, NamespaceDeclaration,
		// AttributeSection, TypeDeclaration, DelegateDeclaration
		// Children in BV: TypeDeclaration, DelegateDeclaration
		
		public string Name {
			get {
				return name;
			}
			set {
				name = value == null ? String.Empty : value;
			}
		}
		
		public NamespaceDeclaration(string name)
		{
			this.Name = name;
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		
		public override string ToString()
		{
			return String.Format("[NamespaceDeclaration: Name={0}]", name);
		}
	}
}

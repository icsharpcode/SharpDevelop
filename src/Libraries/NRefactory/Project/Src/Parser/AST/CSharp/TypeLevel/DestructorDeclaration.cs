// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace ICSharpCode.NRefactory.Parser.AST
{
	public class DestructorDeclaration : AttributedNode
	{
		string         name;
		BlockStatement body;
		
		public string Name {
			get {
				return name;
			}
			set {
				name = value == null ? String.Empty : value;
			}
		}
		
		public BlockStatement Body {
			get {
				return body;
			}
			set {
				body = BlockStatement.CheckNull(value);;
			}
		}
		
		public DestructorDeclaration(string name, Modifier modifier, List<AttributeSection> attributes) : base(modifier, attributes)
		{
			this.Name = name;
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		
		public override string ToString()
		{
			return String.Format("[DestructorDeclaration: Name = {0}, Attributes = {1}, Modifier = {2}, Body = {3}]",
			                     Name,
			                     GetCollectionString(Attributes),
			                     Modifier,
			                     Body);
		}
	}
}

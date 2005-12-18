// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;

namespace ICSharpCode.NRefactory.Parser.AST
{
	public class ConstructorDeclaration : ParametrizedNode
	{
		ConstructorInitializer constructorInitializer = ConstructorInitializer.Null; // only for C#, can be null if no initializer is present
		BlockStatement         blockStatement         = BlockStatement.Null;
		
		public ConstructorInitializer ConstructorInitializer {
			get {
				return constructorInitializer;
			}
			set {
				constructorInitializer = value == null ? ConstructorInitializer.Null : value;
			}
		}
		
		public BlockStatement Body {
			get {
				return blockStatement;
			}
			set {
				blockStatement = BlockStatement.CheckNull(value);;
			}
		}
		
		public ConstructorDeclaration(string name, Modifier modifier, List<ParameterDeclarationExpression> parameters, List<AttributeSection> attributes) : base(modifier, attributes, name, parameters)
		{
		}
		
		public ConstructorDeclaration(string name, Modifier modifier, List<ParameterDeclarationExpression> parameters, ConstructorInitializer constructorInitializer, List<AttributeSection> attributes) : base(modifier, attributes, name, parameters)
		{
			this.ConstructorInitializer = constructorInitializer;
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		public override string ToString()
		{
			return String.Format("[ConstructorDeclaration: Name={0}, Modifier={1}, Parameters={2}, Attributes={3}, Body={4}]",
			                     Name,
			                     Modifier,
			                     GetCollectionString(Parameters),
			                     GetCollectionString(Attributes),
			                     Body);
		}
	}
}

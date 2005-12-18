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
	public class VariableDeclaration : AbstractNode
	{
		// TODO: move to TypeLevel (used from FieldDeclaration)
		string     name;
		Expression initializer;
		TypeReference typeReference; // VB.NET only
		
		public string Name {
			get {
				return name;
			}
			set {
				name = value == null ? String.Empty : value;
			}
		}
		
		public Expression Initializer {
			get {
				return initializer;
			}
			set {
				initializer = Expression.CheckNull(value);
			}
		}
		
		public TypeReference TypeReference {
			get {
				return typeReference;
			}
			set {
				typeReference = TypeReference.CheckNull(value);
			}
		}
		
		
		public VariableDeclaration(string name) : this(name, null)
		{
		}
		
		public VariableDeclaration(string name, Expression initializer) : this(name, initializer, null)
		{
		}
		
		public VariableDeclaration(string name, Expression initializer, TypeReference typeReference)
		{
			this.Name          = name;
			this.Initializer   = initializer;
			this.TypeReference = typeReference;
		}
		
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		
		public override string ToString()
		{
			return String.Format("[VariableDeclaration: Name={0}, Initializer={1}, TypeReference={2}]", name, initializer, typeReference);
		}
	}
}

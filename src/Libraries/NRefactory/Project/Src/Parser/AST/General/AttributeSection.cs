// AttributeDeclaration.cs
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
using System.Diagnostics;
using System.Collections.Generic;

namespace ICSharpCode.NRefactory.Parser.AST
{
	public class NamedArgumentExpression : Expression
	{
		string     name;
		Expression expression;
		
		public string Name {
			get {
				return name;
			}
			set {
				name = value == null ? String.Empty : value;
			}
		}
		
		public Expression Expression {
			get {
				return expression;
			}
			set {
				expression = Expression.CheckNull(value);
			}
		}
		
		public NamedArgumentExpression(string name, Expression expression)
		{
			this.Name       = name;
			this.Expression = expression;
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		
		public override string ToString()
		{
			return String.Format("[NamedArgumentExpression: Name = {0}, Expression = {1}]",
			                     Name,
			                     Expression);
		}
	}
	
	public class Attribute : AbstractNode
	{
		string name = "";
		List<Expression> positionalArguments;
		List<NamedArgumentExpression> namedArguments;
		
		public Attribute(string name, List<Expression> positionalArguments, List<NamedArgumentExpression> namedArguments)
		{
			Debug.Assert(name != null);
			Debug.Assert(positionalArguments != null);
			Debug.Assert(namedArguments != null);
			
			this.name = name;
			this.positionalArguments = positionalArguments;
			this.namedArguments      = namedArguments;
		}
		
		public string Name {
			get {
				return name;
			}
		}
		
		public List<Expression> PositionalArguments {
			get {
				return positionalArguments;
			}
		}
		
		public List<NamedArgumentExpression> NamedArguments {
			get {
				return namedArguments;
			}
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		
		public override string ToString()
		{
			return String.Format("[Attribute: Name = {0}, PositionalArguments = {1}, NamedArguments = {2}]",
			                     Name,
			                     PositionalArguments,
			                     NamedArguments);
		}
	}
	
	public class AttributeSection : AbstractNode, INullable
	{
		string    attributeTarget = "";
		List<Attribute> attributes;
		static AttributeSection nullSection = new NullAttributeSection();
		
		public virtual bool IsNull {
			get {
				return false;
			}
		}
		
		public static AttributeSection Null {
			get {
				return nullSection;
			}
		}
		
		public static AttributeSection CheckNull(AttributeSection attributeSection)
		{
			return attributeSection == null ? AttributeSection.Null : attributeSection;
		}
		
		public string AttributeTarget {
			get {
				return attributeTarget;
			}
			set {
				attributeTarget = value == null ? String.Empty : value;
			}
		}
		
		public List<Attribute> Attributes {
			get {
				return attributes;
			}
			set {
				attributes = value == null ? new List<Attribute>(1) : value;
			}
		}
		
		public AttributeSection() : this(null, null)
		{
		}
		
		public AttributeSection(string attributeTarget, List<Attribute> attributes)
		{
			this.AttributeTarget = attributeTarget;
			this.Attributes     = attributes;
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		public override string ToString()
		{
			return String.Format("[AttributeSection: AttributeTarget={0}, Attributes={1}]",
			                     AttributeTarget,
			                     Attributes);
		}
	}
	
	public class NullAttributeSection : AttributeSection
	{
		public override bool IsNull {
			get {
				return true;
			}
		}
		public override string ToString()
		{
			return String.Format("[NullAttributeSection]");
		}
	}
}

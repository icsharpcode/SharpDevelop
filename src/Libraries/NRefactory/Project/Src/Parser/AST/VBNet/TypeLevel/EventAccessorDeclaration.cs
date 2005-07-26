// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Markus Palme" email="MarkusPalme@gmx.de"/>
//     <version>$Revision: 230 $</version>
// </file>

using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;

namespace ICSharpCode.NRefactory.Parser.AST
{
	public enum EventAccessorType
	{
		RemoveHandlerAccessor,
		AddHandlerAccessor,
		RaiseEventAccessor
	}
	
	public class EventAccessorDeclaration : AbstractNode
	{
		protected BlockStatement body = BlockStatement.Null;
		protected EventAccessorType type;
		protected List<AttributeSection> attributes;
		protected List<ParameterDeclarationExpression> parameters;
		
		public EventAccessorDeclaration(BlockStatement body, List<ParameterDeclarationExpression> parameters, EventAccessorType type, List<AttributeSection> attributes)
		{
			this.body = body;
			this.parameters = parameters;
			this.type = type;
			this.attributes = attributes;
		}
		
		public List<ParameterDeclarationExpression> Parameters
		{
			get
			{
				return parameters;
			}
			set
			{
				parameters = value == null ? new List<ParameterDeclarationExpression>(1) : value;
			}
		}
		
		public List<AttributeSection> Attributes
		{
			get
			{
				return attributes;
			}
			set
			{
				attributes = value == null ? new List<AttributeSection>(1) : value;
			}
		}
		
		public EventAccessorType Type
		{
			get
			{
				return type;
			}
			set
			{
				type = value;
			}
		}
		
		public BlockStatement Body
		{
			get
			{
				return body;
			}
			set
			{
				body = BlockStatement.CheckNull(value);
			}
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
	}
}

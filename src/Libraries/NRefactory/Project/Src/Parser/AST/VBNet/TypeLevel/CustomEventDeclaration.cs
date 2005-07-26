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
	public class CustomEventDeclaration : AttributedNode
	{
		string name;
		EventAccessorDeclaration addHandlerDeclaration;
		EventAccessorDeclaration removeHandlerDeclaration;
		EventAccessorDeclaration raiseEventDeclaration;
		ArrayList implementsClause = new ArrayList();
		
		public CustomEventDeclaration(Modifier modifier, List<AttributeSection> attributes, string name, EventAccessorDeclaration addHandlerDeclaration, EventAccessorDeclaration removeHandlerDeclaration, EventAccessorDeclaration raiseEventDeclaration, ArrayList implementsClause) : base(modifier, attributes)
		{
			this.modifier = modifier;
			this.attributes = attributes;
			this.name = name;
			this.addHandlerDeclaration = addHandlerDeclaration;
			this.removeHandlerDeclaration = removeHandlerDeclaration;
			this.raiseEventDeclaration = raiseEventDeclaration;
			this.implementsClause = implementsClause;
		}
		
		public ArrayList ImplementsClause
		{
			get
			{
				return implementsClause;
			}
			set
			{
				implementsClause = value;
			}
		}
		
		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				name = value;
			}
		}
		
		public EventAccessorDeclaration AddHandlerDeclaration
		{
			get
			{
				return addHandlerDeclaration;
			}
			set
			{
				addHandlerDeclaration = value;
			}
		}
		
		public EventAccessorDeclaration RemoveHandlerDeclaration
		{
			get
			{
				return removeHandlerDeclaration;
			}
			set
			{
				removeHandlerDeclaration = value;
			}
		}
	
		public EventAccessorDeclaration RaiseEventDeclaration
		{
			get
			{
				return raiseEventDeclaration;
			}
			set
			{
				raiseEventDeclaration = value;
			}
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
	}
}

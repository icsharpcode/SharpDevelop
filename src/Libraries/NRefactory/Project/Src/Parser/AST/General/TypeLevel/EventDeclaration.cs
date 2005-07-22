// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace ICSharpCode.NRefactory.Parser.AST
{
	public class EventDeclaration : ParametrizedNode
	{
		TypeReference   typeReference = TypeReference.Null;
		List<VariableDeclaration> variableDeclarators = new List<VariableDeclaration>(1); 
//		ArrayList     implementsClause = new ArrayList(); // VB only
		ArrayList     implementsClause = new ArrayList(); // VB only
		EventAddRegion addRegion       = EventAddRegion.Null; // only for C#
		EventRemoveRegion removeRegion = EventRemoveRegion.Null; // only for C#
		Point           bodyStart = new Point(-1, -1);
		Point           bodyEnd = new Point(-1, -1);
		
		public TypeReference TypeReference {
			get {
				return typeReference;
			}
			set {
				typeReference = TypeReference.CheckNull(value);
			}
		}
		public List<VariableDeclaration> VariableDeclarators {
			get {
				return variableDeclarators;
			}
			set {
				variableDeclarators = value == null ? new List<VariableDeclaration>(1) : value;
			}
		}
		
		public EventAddRegion AddRegion {
			get {
				return addRegion;
			}
			set {
				addRegion = value == null ? EventAddRegion.Null : value;
			}
		}
		public EventRemoveRegion RemoveRegion {
			get {
				return removeRegion;
			}
			set {
				removeRegion = value == null ? EventRemoveRegion.Null : value;
			}
		}
		
		public bool HasAddRegion {
			get {
				return !addRegion.IsNull;
			}
		}
		
		public bool HasRemoveRegion {
			get {
				return !removeRegion.IsNull;
			}
		}
		public Point BodyStart {
			get {
				return bodyStart;
			}
			set {
				bodyStart = value;
			}
		}
		public Point BodyEnd {
			get {
				return bodyEnd;
			}
			set {
				bodyEnd = value;
			}
		}
		
		public ArrayList ImplementsClause {
			get {
				return implementsClause;
			}
			set {
				implementsClause = value == null ? new ArrayList() : value;
			}
		}
		
		public EventDeclaration(Modifier modifier, List<AttributeSection> attributes) : base(modifier, attributes)
		{
		}
		
		public EventDeclaration(TypeReference typeReference, List<VariableDeclaration> variableDeclarators, Modifier modifier, List<AttributeSection> attributes) : base(modifier, attributes)
		{
			this.TypeReference = typeReference;
			this.VariableDeclarators = variableDeclarators;
		}
		
		public EventDeclaration(TypeReference typeReference, string name, Modifier modifier, List<AttributeSection> attributes)  : base(modifier, attributes, name)
		{
			this.TypeReference = typeReference;
		}
		
		// for VB:
		public EventDeclaration(TypeReference typeReference, Modifier modifier, List<ParameterDeclarationExpression> parameters, List<AttributeSection> attributes, string name, ArrayList implementsClause)  : base(modifier, attributes, name, parameters)
		{
			this.TypeReference    = typeReference;
			this.ImplementsClause = implementsClause;
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		public override string ToString()
		{
			return String.Format("[EventDeclaration: TypeReference={0}, VariableDeclarators={1}, Modifier={2}, Attributes={3}, Name={4}, BodyStart={5}, BodyEnd={6}]",
			                     TypeReference,
			                     GetCollectionString(VariableDeclarators),
			                     Modifier,
			                     GetCollectionString(Attributes),
			                     Name,
			                     BodyStart,
			                     BodyEnd);
		}
		
	}
}

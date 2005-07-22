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

namespace ICSharpCode.NRefactory.Parser.AST
{
	public class IndexerDeclaration : AttributedNode
	{
		TypeReference type = TypeReference.Null;
		string        namespaceName = String.Empty;
		Point         bodyStart = new Point(-1, -1);
		Point         bodyEnd   = new Point(-1, -1);
		
		List<ParameterDeclarationExpression> parameters;
		
		PropertyGetRegion  propertyGetRegion = PropertyGetRegion.Null;
		PropertySetRegion  propertySetRegion = PropertySetRegion.Null;
		
		public TypeReference TypeReference {
			get {
				return type;
			}
			set {
				type = TypeReference.CheckNull(value);
			}
		}
		
		public List<ParameterDeclarationExpression> Parameters {
			get {
				return parameters;
			}
			set {
				parameters = value == null ? new List<ParameterDeclarationExpression>(1) : value;
			}
		}
		
		public bool HasGetRegion {
			get {
				return !propertyGetRegion.IsNull;
			}
		}
		
		public bool HasSetRegion {
			get {
				return !propertySetRegion.IsNull;
			}
		}
		
		public bool IsReadOnly {
			get {
				return HasGetRegion && !HasSetRegion;
			}
		}
		
		public bool IsWriteOnly {
			get {
				return !HasGetRegion && HasSetRegion;
			}
		}
		
		public PropertyGetRegion GetRegion {
			get {
				return propertyGetRegion;
			}
			set {
				propertyGetRegion = value == null ? PropertyGetRegion.Null : value;
			}
		}
		public PropertySetRegion SetRegion {
			get {
				return propertySetRegion;
			}
			set {
				propertySetRegion = value == null ? PropertySetRegion.Null : value;
			}
		}
		
		public string NamespaceName {
			get {
				return namespaceName;
			}
			set {
				namespaceName = value == null ? String.Empty : value;
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
		
		public IndexerDeclaration(Modifier modifier, List<ParameterDeclarationExpression> parameters, List<AttributeSection> attributes) : base(modifier, attributes)
		{
			this.Parameters = parameters;
		}
		
		public IndexerDeclaration(TypeReference typeReference, List<ParameterDeclarationExpression> parameters, Modifier modifier, List<AttributeSection> attributes) : base(modifier, attributes)
		{
			this.TypeReference = typeReference;
			this.Parameters    = parameters;
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		
		public override string ToString()
		{
			return String.Format("[IndexerDeclaration: TypeReference = {0}, Modifier = {1}, Parameters = {2}, Attributes = {3}, NamespaceName = {4}, BodyStart = {5}, BodyEnd = {6}]",
			                     TypeReference,
			                     Modifier,
			                     GetCollectionString(Parameters),
			                     GetCollectionString(Attributes),
			                     NamespaceName,
			                     BodyStart,
			                     BodyEnd);
		}
		
	}
}

// IndexerDeclaration.cs
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
using System.Drawing;
using System.Diagnostics;
using System.Collections;

namespace ICSharpCode.NRefactory.Parser.AST
{
	public class IndexerDeclaration : AttributedNode
	{
		TypeReference type = TypeReference.Null;
		string        namespaceName = String.Empty;
		Point         bodyStart = new Point(-1, -1);
		Point         bodyEnd   = new Point(-1, -1);
		
//		List<ParameterDeclarationExpression> parameters;
		ArrayList parameters;
		
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
		
		public ArrayList Parameters {
			get {
				return parameters;
			}
			set {
				parameters = value == null ? new ArrayList(1) : value;
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
		
		public IndexerDeclaration(Modifier modifier, ArrayList parameters, ArrayList attributes) : base(modifier, attributes)
		{
			this.Parameters = parameters;
		}
		
		public IndexerDeclaration(TypeReference typeReference, ArrayList parameters, Modifier modifier, ArrayList attributes) : base(modifier, attributes)
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

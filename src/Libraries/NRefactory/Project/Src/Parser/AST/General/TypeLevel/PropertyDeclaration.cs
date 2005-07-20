// PropertyDeclaration.cs
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
using System.Collections.Generic;

namespace ICSharpCode.NRefactory.Parser.AST
{
	public class PropertyDeclaration : ParametrizedNode
	{
		TypeReference   typeReference = TypeReference.Null;
		Point           bodyStart = new Point(-1, -1);
		Point           bodyEnd = new Point(-1, -1);
		ArrayList     implementsClause = new ArrayList(); // VB only
		
		PropertyGetRegion  propertyGetRegion = PropertyGetRegion.Null;
		PropertySetRegion  propertySetRegion = PropertySetRegion.Null;
		
		public TypeReference TypeReference {
			get {
				return typeReference;
			}
			set {
				Debug.Assert(value != null);
				typeReference = value;
			}
		}
		
		public PropertyGetRegion GetRegion {
			get {
				return propertyGetRegion;
			}
			set {
				if (value == null) {
					propertyGetRegion = PropertyGetRegion.Null;
				} else {
					propertyGetRegion = value;
				}
			}
		}
		public PropertySetRegion SetRegion {
			get {
				return propertySetRegion;
			}
			set {
				if (value == null) {
					propertySetRegion = PropertySetRegion.Null;
				} else {
					propertySetRegion = value;
				}
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
		
		public PropertyDeclaration(string name, TypeReference typeReference, Modifier modifier, List<AttributeSection> attributes) : base(modifier, attributes, name)
		{
			Debug.Assert(typeReference != null);
			this.typeReference = typeReference;
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		public override string ToString()
		{
			return String.Format("[PropertyDeclaration: Name = {0}, Modifier = {1}, TypeReference = {2}, Attributes = {3}, GetRegion = {4}, SetRegion = {5}]",
			                     Name,
			                     Modifier,
			                     TypeReference,
			                     GetCollectionString(Attributes),
			                     GetRegion,
			                     SetRegion);
		}
	}
}

// EventDeclaration.cs
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
using System.Collections.Specialized;

namespace ICSharpCode.NRefactory.Parser.AST
{
	public class EventDeclaration : ParametrizedNode
	{
		TypeReference   typeReference = TypeReference.Null;
//		List<VariableDeclaration> variableDeclarators = new List<VariableDeclaration>(1); 
//		ArrayList     implementsClause = new ArrayList(); // VB only
		ArrayList variableDeclarators = new ArrayList(1); 
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
		public ArrayList VariableDeclarators {
			get {
				return variableDeclarators;
			}
			set {
				variableDeclarators = value == null ? new ArrayList(1) : value;
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
		
		public EventDeclaration(Modifier modifier, ArrayList attributes) : base(modifier, attributes)
		{
		}
		
		public EventDeclaration(TypeReference typeReference, ArrayList variableDeclarators, Modifier modifier, ArrayList attributes) : base(modifier, attributes)
		{
			this.TypeReference = typeReference;
			this.VariableDeclarators = variableDeclarators;
		}
		
		public EventDeclaration(TypeReference typeReference, string name, Modifier modifier, ArrayList attributes)  : base(modifier, attributes, name)
		{
			this.TypeReference = typeReference;
		}
		
		// for VB:
		public EventDeclaration(TypeReference typeReference, Modifier modifier, ArrayList parameters, ArrayList attributes, string name, ArrayList implementsClause)  : base(modifier, attributes, name, parameters)
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

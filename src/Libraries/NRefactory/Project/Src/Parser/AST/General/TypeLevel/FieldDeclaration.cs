// FieldDeclaration.cs
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
using System.Collections;
using System.Collections.Generic;

namespace ICSharpCode.NRefactory.Parser.AST
{
	public class FieldDeclaration : AbstractNode
	{
//		List<AttributeSection>    attributes;
		ArrayList                 attributes;
		TypeReference             typeReference = TypeReference.Null;
		Modifier                  modifier      = Modifier.None;
		List<VariableDeclaration> fields        = new List<VariableDeclaration>(1);
		
		public ArrayList Attributes {
			get {
				return attributes;
			}
			set {
				Debug.Assert(value != null);
				attributes = value;
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
		public Modifier Modifier {
			get {
				return modifier;
			}
			set {
				modifier = value;
			}
		}
		public List<VariableDeclaration> Fields {
			get {
				return fields;
			}
			set {
				Debug.Assert(fields != null);
				fields = value;
			}
		}
		
		public TypeReference GetTypeForField(int fieldIndex)
		{
			if (!typeReference.IsNull) {
				return typeReference;
			}
			
			for (int i = fieldIndex; i < Fields.Count;++i) {
				if (!((VariableDeclaration)Fields[i]).TypeReference.IsNull) {
					return ((VariableDeclaration)Fields[i]).TypeReference;
				}
			}
			return TypeReference.Null;
		}
		
		// for enum members
		public FieldDeclaration(ArrayList attributes)
		{
			Debug.Assert(attributes != null);
			this.attributes = attributes;
		}
		
		// for all other cases
		public FieldDeclaration(ArrayList attributes, TypeReference typeReference, Modifier modifier)
		{
			this.attributes    = attributes;
			this.TypeReference = typeReference;
			this.modifier      = modifier;
		}
		
		public VariableDeclaration GetVariableDeclaration(string variableName)
		{
			foreach (VariableDeclaration variableDeclaration in Fields) {
				if (variableDeclaration.Name == variableName) {
					return variableDeclaration;
				}
			}
			return null;
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		public override string ToString()
		{
			return String.Format("[FieldDeclaration: Attributes={0}, TypeReference={1}, Modifier={2}, Fields={3}]",
			                     GetCollectionString(Attributes),
			                     TypeReference,
			                     Modifier,
			                     GetCollectionString(Fields));
		}
		
	}
}

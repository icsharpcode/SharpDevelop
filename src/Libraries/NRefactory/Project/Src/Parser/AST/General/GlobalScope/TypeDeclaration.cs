// TypeDeclaration.cs
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
using System.Collections.Specialized;

namespace ICSharpCode.NRefactory.Parser.AST
{
	public class TypeDeclaration : AttributedNode
	{
		// Children of Struct:    FieldDeclaration, MethodDeclaration, EventDeclaration, ConstructorDeclaration,
		//                        OperatorDeclaration, TypeDeclaration, IndexerDeclaration, PropertyDeclaration, in VB: DeclareDeclaration
		// Childrean of class:    children of struct, DestructorDeclaration
		// Children of Interface: MethodDeclaration, PropertyDeclaration, IndexerDeclaration, EventDeclaration, in VB: TypeDeclaration(Enum) too
		// Children of Enum:      FieldDeclaration
		string name            = "";
		Types type             = Types.Class; // Class | Interface | Struct | Enum
		ArrayList bases = new ArrayList(); // TODO: can be generics too!
		List<TemplateDefinition> templates = new List<TemplateDefinition>();
		
		public string Name {
			get {
				return name;
			}
			set {
				name = (value != null) ? value : "?";
			}
		}
		
		public Types Type {
			get {
				return type;
			}
			set {
				type = value;
			}
		}
		
		public ArrayList BaseTypes {
			get {
				return bases;
			}
			set {
				Debug.Assert(value != null);
				bases = value;
			}
		}
		
		public List<TemplateDefinition> Templates {
			get {
				return templates;
			}
			set {
				Debug.Assert(value != null);
				templates = value;
			}
		}
		
		public TemplateDefinition this[string name] {
			get {
				foreach (TemplateDefinition td in templates) {
					if (td.Name == name) {
						return td;
					}
				}
				return null;
			}
		}
		
		public TypeDeclaration(Modifier modifier, ArrayList attributes) : base(modifier, attributes)
		{
		}
		

		
//		public TypeDeclaration(string name, Modifier modifier, Types type, ArrayList bases, ArrayList attributes)
//		{
//			Debug.Assert(name != null);
//			this.name = name;
//			this.modifier = modifier;
//			this.type = type;
//			Debug.Assert(bases != null);
//			this.bases = bases;
//			Debug.Assert(attributes != null);
//			this.attributes = attributes;
//		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		
		public override string ToString()
		{
			return String.Format("[TypeDeclaration: Name={0}, Modifier={1}, Type={2}, BaseTypes={3}, Attributes={4}, Children={5}]",
			                     name,
			                     modifier,
			                     type,
			                     GetCollectionString(bases),
			                     GetCollectionString(attributes),
			                     GetCollectionString(Children));
		}
	}
	public class TemplateDefinition : AttributedNode
	{
		string name = "";
		List<TypeReference> bases = new List<TypeReference>();
		
		public string Name {
			get {
				return name;
			}
		}
		
		public List<TypeReference> Bases {
			get {
				return bases;
			}
		}
		
		public TemplateDefinition(string name, ArrayList attributes) : base(attributes)
		{
			this.name = name;
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		
		public override string ToString()
		{
			return String.Format("[TypeDeclaration: Name={0}, BaseTypes={1}]",
			                     name,
			                     GetCollectionString(bases));
		}
	}
}

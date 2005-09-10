// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

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
		ClassType type             = ClassType.Class; // Class | Interface | Struct | Enum
		List<TypeReference> baseTypes = new List<TypeReference>();
		List<TemplateDefinition> templates = new List<TemplateDefinition>();
		
		public string Name {
			get {
				return name;
			}
			set {
				name = (value != null) ? value : "?";
			}
		}
		
		public ClassType Type {
			get {
				return type;
			}
			set {
				type = value;
			}
		}
		
		public List<TypeReference> BaseTypes {
			get {
				return baseTypes;
			}
			set {
				Debug.Assert(value != null);
				baseTypes = value;
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
		
		public TypeDeclaration(Modifier modifier, List<AttributeSection> attributes) : base(modifier, attributes)
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
			                     GetCollectionString(baseTypes),
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
		
		public TemplateDefinition(string name, List<AttributeSection> attributes) : base(attributes)
		{
			if (name == null || name.Length == 0)
				name = "?";
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

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

namespace ICSharpCode.NRefactory.Parser.AST
{
	public class FieldDeclaration : AttributedNode
	{
		TypeReference             typeReference = TypeReference.Null;
		List<VariableDeclaration> fields        = new List<VariableDeclaration>(1);
		
		public TypeReference TypeReference {
			get {
				return typeReference;
			}
			set {
				typeReference = TypeReference.CheckNull(value);
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
		public FieldDeclaration(List<AttributeSection> attributes) : base(attributes)
		{
			Debug.Assert(attributes != null);
		}
		
		// for all other cases
		public FieldDeclaration(List<AttributeSection> attributes, TypeReference typeReference, Modifier modifier) : base(modifier, attributes)
		{
			this.TypeReference = typeReference;
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

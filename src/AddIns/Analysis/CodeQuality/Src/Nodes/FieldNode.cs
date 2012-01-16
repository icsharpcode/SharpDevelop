// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.CodeQualityAnalysis
{
	public class FieldNode : INode
	{
		public IField Field { get; private set; }
		public TypeNode DeclaringType { get; private set; }
		public TypeNode ReturnType { get; private set; }
		
		public string Name {
			get { return Field.Name; }
		}
		
		public FieldNode(IField field, TypeNode declaringType, TypeNode returnType)
		{
			this.Field = field;
			this.DeclaringType = declaringType;
			this.ReturnType = returnType;
		}
		
		public IDependency Dependency {
			get { return null; }
		}
		
		public BitmapSource Icon {
			get { return NodeIconService.GetIcon(this); }
		}
		
		public string GetInfo()
		{
			throw new NotImplementedException();
		}
		
		public Relationship GetRelationship(INode node)
		{
			Relationship relationship = new Relationship();
			
			if (node == this) {
				relationship.Relationships.Add(RelationshipType.Same);
				return relationship;
			}
			
			if (node is NamespaceNode) {
				NamespaceNode ns = (NamespaceNode)node;
				
				foreach (var type in this.GetAllTypes()) {
					if (type != null && type.Namespace == ns) {
						relationship.AddRelationship(RelationshipType.UseThis);
					}
				}
			}
			
			if (node is TypeNode) {
				TypeNode type = (TypeNode)node;
				
				foreach (var usedType in this.GetAllTypes()) {
					if (type == usedType) {
						relationship.AddRelationship(RelationshipType.UseThis);
					}
				}
			}
			
			if (node is FieldNode) {
				FieldNode field = (FieldNode)node;
				
				if (this == field) {
					relationship.AddRelationship(RelationshipType.UseThis);
				}
			}
			
			return relationship;
		}
		
		public IEnumerable<TypeNode> GetAllTypes()
		{
			yield return ReturnType;
			yield return DeclaringType;
		}
		
		public override string ToString()
		{
			return Name;
		}
	}
}

// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.CodeQualityAnalysis
{
	public class MethodNode : INode
	{
		public MethodNode(IMethod method, TypeNode declaringType, TypeNode returnType)
		{
			Method = method;
			DeclaringType = declaringType;
			ReturnType = returnType;
			instructions = new List<Instruction>();
			parameters = new List<Parameter>();
			typeUses = new List<TypeNode>();
			methodUses = new List<MethodNode>();
			fieldUses = new List<FieldNode>();
		}
		
		public IMethod Method { get; private set; }
		public TypeNode ReturnType { get; private set; }
		public TypeNode DeclaringType { get; private set; }
		public int CyclomaticComplexity { get; internal set; }
		public int Variables { get; private set; }
		
		internal List<Instruction> instructions;
		
		public IEnumerable<Instruction> Instructions {
			get { return instructions; }
		}
		
		internal List<Parameter> parameters;
		
		public IEnumerable<Parameter> Parameters {
			get { return parameters; }
		}
		
		internal List<TypeNode> typeUses;
		
		public IEnumerable<TypeNode> TypeUses {
			get { return typeUses; }
		}
		
		internal List<MethodNode> methodUses;
		
		public IEnumerable<MethodNode> MethodUses {
			get { return methodUses; }
		}
		
		internal List<FieldNode> fieldUses;
		
		public IEnumerable<FieldNode> FieldUses {
			get { return fieldUses; }
		}
		
		public string Name { get { return Method.Name; } }
		
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
			
			if (node is MethodNode) {
				MethodNode method = (MethodNode)node;
				
				foreach (var usedMethod in this.GetAllMethods()) {
					if (method == usedMethod) {
						relationship.AddRelationship(RelationshipType.UseThis);
					}
				}
			}
			
			if (node is FieldNode) {
				FieldNode field = (FieldNode)node;
				
				foreach (var usedField in this.GetAllFields()) {
					if (field == usedField) {
						relationship.AddRelationship(RelationshipType.UseThis);
					}
				}
			}

			return relationship;
		}
		
		public IEnumerable<TypeNode> GetAllTypes()
		{
			if (DeclaringType != null)
				yield return this.DeclaringType;
			if (ReturnType != null)
				yield return this.ReturnType;
			foreach (var type in this.TypeUses)
				yield return type;
//			foreach (var type in this.GenericReturnTypes)
//				yield return type;
		}

		public IEnumerable<MethodNode> GetAllMethods()
		{
			foreach (var method in this.MethodUses)
				yield return method;
		}

		public IEnumerable<FieldNode> GetAllFields()
		{
			foreach (var field in this.FieldUses)
				yield return field;
		}
		
		public IEnumerable<TypeNode> GetUsedTypes()
		{
			throw new NotImplementedException();
		}
	}
	
	public class Instruction
	{
		public MethodNode DeclaringMethod { get; set; }
		public string Operand { get; set; }
	}
	
	public class Parameter
	{
		
	}
}
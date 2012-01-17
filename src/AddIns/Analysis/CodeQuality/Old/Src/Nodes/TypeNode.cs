// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

using ICSharpCode.CodeQualityAnalysis.Controls;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.CodeQualityAnalysis
{
	public class TypeNode : INode, IDependency
	{
		public string Name { get { return TypeDefinition.Name; } }
		
		NamespaceNode @namespace;
		
		public NamespaceNode Namespace {
			get { return @namespace; }
		}
		
		public IDependency Dependency {
			get { return this; }
		}
		
		public BitmapSource Icon {
			get { return NodeIconService.GetIcon(this); }
		}
		
		public string GetInfo()
		{
			var builder = new StringBuilder();
			builder.AppendLine("Type Summary");
			builder.Append(Environment.NewLine);
			builder.AppendLine(string.Format("Name: {0}", Name));
			builder.AppendLine(string.Format("TypeCode: {0}", TypeDefinition.KnownTypeCode));
			builder.AppendLine(string.Format("Methods: {0}", Methods.Count()));
			builder.AppendLine(string.Format("Fields: {0}", Fields.Count()));
			builder.AppendLine(string.Format("Nested types: {0}", NestedTypes.Count()));
			builder.AppendLine(string.Format("Base types: {0}", BaseTypes.Count()));
			// more to come

			builder.Append(Environment.NewLine);

//			if (TypeDefinition.IsAbstract)
//				builder.AppendLine("IsAbstract");
//			if (TypeDefinition.
//			if (IsInternal)
//				builder.AppendLine("IsInternal");
//			if (IsNestedPrivate)
//				builder.AppendLine("IsNestedPrivate");
//			if (IsNestedProtected)
//				builder.AppendLine("IsNestedProtected");
//			if (IsNestedPublic)
//				builder.AppendLine("IsNestedPublic");
//			if (IsPublic)
//				builder.AppendLine("IsPublic");
//			if (IsSealed)
//				builder.AppendLine("IsSealed");
//			if (IsStruct)
//				builder.AppendLine("IsStruct");
			
			return builder.ToString();
		}
		
		public override string ToString()
		{
			return Name;
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
				
				foreach (var usedType in GetAllTypes()) {
					if (type == usedType) {
						relationship.AddRelationship(RelationshipType.UseThis);
					}
				}
			}
			
			if (node is MethodNode) {
				MethodNode method = (MethodNode)node;
				
				foreach (var usedMethod in GetAllMethods()) {
					if (method == usedMethod) {
						relationship.AddRelationship(RelationshipType.UseThis);
					}
				}
			}
			
			if (node is FieldNode) {
				FieldNode field = (FieldNode)node;
				
				foreach (var usedField in GetAllFields()) {
					if (field == usedField) {
						relationship.AddRelationship(RelationshipType.UseThis);
					}
				}
			}
			
			return relationship;
		}
		
		public IEnumerable<TypeNode> GetAllTypes()
		{
			foreach (var field in Fields) {
				foreach (var type in field.GetAllTypes()) {
					yield return type;
				}
			}
			
			foreach (var method in Methods) {
				foreach (var type in method.GetAllTypes()) {
					yield return type;
				}
			}
			
			if (DeclaringType != null)
				yield return DeclaringType;
			
			foreach (var type in BaseTypes) {
				yield return type;
			}
			
			foreach (var type in NestedTypes) {
				yield return type;
			}
			
			foreach (var type in BaseTypeArguments) {
				yield return type;
			}
		}

		public IEnumerable<MethodNode> GetAllMethods()
		{
			foreach (var method in this.Methods) {
				yield return method;
				foreach (var usedMethod in method.GetAllMethods())
					yield return usedMethod;
			}
		}

		public IEnumerable<FieldNode> GetAllFields()
		{
			foreach (var field in this.Fields) {
				yield return field;
			}

			foreach (var method in this.Methods) {
				foreach (var field in method.GetAllFields()) {
					yield return field;
				}
			}
		}
		
		public TypeNode(ITypeDefinition type, NamespaceNode @namespace)
		{
			TypeDefinition = type;
			this.@namespace = @namespace;
			methods = new List<MethodNode>();
			fields = new List<FieldNode>();
			nestedTypes = new List<TypeNode>();
			baseTypes = new List<TypeNode>();
			baseTypeArguments = new List<TypeNode>();
		}
		
		public ITypeDefinition TypeDefinition { get; private set; }
		
		internal List<MethodNode> methods;
		
		public IEnumerable<MethodNode> Methods {
			get { return methods; }
		}
		
		internal List<FieldNode> fields;
		
		public IEnumerable<FieldNode> Fields {
			get { return fields; }
		}
		
		internal List<TypeNode> nestedTypes;
		
		public IEnumerable<TypeNode> NestedTypes {
			get { return nestedTypes; }
		}
		
		internal List<TypeNode> baseTypes;
		
		public IEnumerable<TypeNode> BaseTypes {
			get { return baseTypes; }
		}
		
		internal List<TypeNode> baseTypeArguments;
		
		public IEnumerable<TypeNode> BaseTypeArguments {
			get { return baseTypeArguments; }
		}
		
		public TypeNode DeclaringType { get; private set; }
		
		public IEnumerable<TypeNode> GetUsedTypes()
		{
			var set = new HashSet<TypeNode>();

			foreach (var method in Methods)
			{
				set.UnionWith(method.typeUses);

//				foreach (var parameter in method.Parameters)
//				{
//					if (parameter.IsGenericInstance)
//						set.UnionWith(parameter.GenericTypes);
//
//					if (parameter.ParameterType != null)
//						set.Add(parameter.ParameterType);
//				}
//
//				if (method.IsReturnTypeGenericInstance)
//					set.UnionWith(method.GenericReturnTypes);

				if (method.ReturnType != null)
					set.Add(method.ReturnType);
			}

			foreach (var field in Fields)
			{
//				if (field.IsGenericInstance)
//					set.UnionWith(field.GenericTypes);

				if (field.ReturnType != null) // if it is null so it is type from outside of this assembly
					set.Add(field.ReturnType); // TODO: find solution to handle them
			}

			set.UnionWith(BaseTypes);
			set.UnionWith(BaseTypeArguments);

			return set;
		}
		
		DependencyGraph cachedGraph;
		
		public DependencyGraph BuildDependencyGraph()
		{
			if (cachedGraph != null)
				return cachedGraph;

			cachedGraph = new DependencyGraph();

			foreach (var method in Methods)
				cachedGraph.AddVertex(new DependencyVertex(method));

			foreach (var field in Fields)
				cachedGraph.AddVertex(new DependencyVertex(field));

			foreach (var method in Methods) {
				foreach (var methodUse in method.MethodUses)
					cachedGraph.AddEdge(new DependencyEdge(new DependencyVertex(method), new DependencyVertex(methodUse)));

				foreach (var fieldUse in method.FieldUses)
					cachedGraph.AddEdge(new DependencyEdge(new DependencyVertex(method), new DependencyVertex(fieldUse)));
			}
			
			return cachedGraph;
		}
	}
}

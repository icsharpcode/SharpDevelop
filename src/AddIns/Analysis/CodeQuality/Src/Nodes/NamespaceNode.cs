// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

using ICSharpCode.CodeQualityAnalysis.Controls;

namespace ICSharpCode.CodeQualityAnalysis
{
	public class NamespaceNode : INode, IDependency
	{
		public NamespaceNode(AssemblyNode assembly, string name)
		{
			Name = name;
			Assembly = assembly;
			types = new List<TypeNode>();
		}
		
		public string Name { get; private set; }
		public AssemblyNode Assembly { get; private set; }
		
		public IDependency Dependency {
			get { return this; }
		}
		
		public BitmapSource Icon {
			get {
				return NodeIconService.GetIcon(this);
			}
		}
		
		public string GetInfo()
		{
			var builder = new StringBuilder();
			builder.AppendLine("Namespace Summary");
			builder.Append(Environment.NewLine);
			builder.AppendLine(String.Format("Name: {0}", Name));
			builder.AppendLine(String.Format("Methods: {0}", Types.Sum(type => type.Methods.Count())));
			builder.AppendLine(String.Format("Fields: {0}", Types.Sum(type => type.Fields.Count())));
			builder.AppendLine(String.Format("Types: {0}", Types.Count()));
			// more to come
			
			return builder.ToString();
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
				
				foreach (var type in GetAllTypes()) {
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
			foreach (var type in Types) {
				yield return type;
				foreach (var usedType in type.GetAllTypes()) {
					yield return usedType;
				}
			}
		}

		public IEnumerable<MethodNode> GetAllMethods()
		{
			foreach (var type in Types) {
				foreach (var method in type.GetAllMethods()) {
					yield return method;
				}
			}
		}

		public IEnumerable<FieldNode> GetAllFields()
		{
			foreach (var type in Types) {
				foreach (var field in type.GetAllFields()) {
					yield return field;
				}
			}
		}
		
		internal List<TypeNode> types;
		
		public IEnumerable<TypeNode> Types {
			get { return types; }
		}
		
		DependencyGraph cachedGraph;
		
		public DependencyGraph BuildDependencyGraph()
		{
			if (cachedGraph != null)
				return cachedGraph;
			
			this.cachedGraph = new DependencyGraph();
			
			foreach (var t in types) {
				cachedGraph.AddVertex(new DependencyVertex(t));
			}
			
			foreach (var t in types) {
				foreach (var dependType in t.GetUsedTypes()) {
					if (dependType != t && dependType.Namespace == t.Namespace)
						cachedGraph.AddEdge(new DependencyEdge(new DependencyVertex(t), new DependencyVertex(dependType)));
				}
			}
			
			return cachedGraph;
		}
		
		public override string ToString()
		{
			return Name;
		}
	}
}

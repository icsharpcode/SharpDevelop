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
	/// <summary>
	/// Description of AssemblyNode.
	/// </summary>
	public class AssemblyNode : INode, IDependency
	{
		public AssemblyNode(string name)
		{
			Name = name;
		}
		
		public string Name { get; private set; }
		internal IEnumerable<NamespaceNode> namespaces;
		
		public IEnumerable<NamespaceNode> Namespaces {
			get { return namespaces; }
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
			builder.AppendLine("Assembly Summary");
			builder.Append(Environment.NewLine);
			builder.AppendLine(String.Format("Name: {0}", Name));
			builder.AppendLine(String.Format("Methods: {0}", MethodsCount));
			builder.AppendLine(String.Format("Fields: {0}", FieldsCount));
			builder.AppendLine(String.Format("Types: {0}", TypesCount));
			builder.AppendLine(String.Format("Namespaces: {0}", Namespaces.Count()));
			// more to come

			return builder.ToString();
		}
		
		public Relationship GetRelationship(INode node)
		{
			return new Relationship();
		}
		
		public int FieldsCount {
			get {
				return Namespaces.SelectMany(ns => ns.Types).Sum(t => t.Fields.Count());
			}
		}
		
		public int MethodsCount {
			get {
				return Namespaces.SelectMany(ns => ns.Types).Sum(t => t.Methods.Count());
			}
		}
		
		public int TypesCount {
			get {
				return Namespaces.Sum(t => t.Types.Count());
			}
		}
		
		DependencyGraph cachedGraph;
		
		public DependencyGraph BuildDependencyGraph()
		{
			if (cachedGraph != null)
				return cachedGraph;

			cachedGraph = new DependencyGraph();

			foreach (var ns in Namespaces) {
				cachedGraph.AddVertex(new DependencyVertex(ns));

				foreach (var type in ns.Types) {
					var types = type.GetUsedTypes();

					foreach (var dependType in types) {
						if (dependType != type && dependType.Namespace.Assembly == type.Namespace.Assembly)
							cachedGraph.AddEdge(new DependencyEdge(new DependencyVertex(type.Namespace),
							                                       new DependencyVertex(dependType.Namespace)));
					}
				}
			}
			
			return cachedGraph;
		}
	}
}

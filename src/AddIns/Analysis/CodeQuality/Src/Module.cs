using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

using ICSharpCode.CodeQualityAnalysis.Controls;
using QuickGraph;

namespace ICSharpCode.CodeQualityAnalysis
{
	public class Module : IDependency, INode
	{
		private DependencyGraph _graphCache;
		
		/// <summary>
		/// Namespaces within module
		/// </summary>
		public ISet<Namespace> Namespaces { get; private set; }

		/// <summary>
		/// Name of module
		/// </summary>
		public string Name { get; set; }

		public Module()
		{
			Namespaces = new HashSet<Namespace>();

			Dependency = this;
		}

		public DependencyGraph BuildDependencyGraph()
		{
			if (_graphCache != null)
				return _graphCache;

			var g = new DependencyGraph();

			foreach (var ns in Namespaces)
			{
				g.AddVertex(new DependencyVertex(ns));
			}

			foreach (var ns in Namespaces)
			{
				foreach (var type in ns.Types)
				{
					var types = type.GetUses();

					foreach (var dependType in types)
					{
						if (dependType != type && dependType.Namespace.Module == type.Namespace.Module)
							g.AddEdge(new DependencyEdge(new DependencyVertex(type.Namespace),
							                             new DependencyVertex(dependType.Namespace)));
					}
				}
			}

			_graphCache = g;
			return g;
		}
		
		public Relationship GetRelationship(INode node)
		{
			Relationship relationship = new Relationship();
			return relationship;
		}

		public override string ToString()
		{
			return Name;
		}

		public IDependency Dependency { get; set; }
		
		public int MethodsCount
		{
			get {
				return Namespaces.Sum(ns => ns.Types.Sum(type => type.Methods.Count));
			}
		}
		
		public int FieldsCount
		{
			get {
				return Namespaces.Sum(ns => ns.Types.Sum(type => type.Fields.Count));
			}
		}
		
		public int TypesCount
		{
			get {
				return Namespaces.Sum(ns => ns.Types.Count);
			}
		}

		public string GetInfo()
		{
			var builder = new StringBuilder();
			builder.AppendLine("Module Summary");
			builder.Append(Environment.NewLine);
			builder.AppendLine(String.Format("Name: {0}", Name));
			builder.AppendLine(String.Format("Methods: {0}", MethodsCount));
			builder.AppendLine(String.Format("Fields: {0}", FieldsCount));
			builder.AppendLine(String.Format("Types: {0}", TypesCount));
			builder.AppendLine(String.Format("Namespaces: {0}", Namespaces.Count));
			// more to come

			return builder.ToString();
		}
		
		public BitmapSource Icon { get { return NodeIconService.GetIcon(this); } }
	}
}

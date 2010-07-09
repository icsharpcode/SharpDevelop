using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.CodeQualityAnalysis.Controls;
using QuickGraph;

namespace ICSharpCode.CodeQualityAnalysis
{
    public class Namespace : IDependency, INode
    {
        private DependencyGraph _graphCache;

        /// <summary>
        /// Types within namespace
        /// </summary>
        public ISet<Type> Types { get; set; }

        /// <summary>
        /// Name of namespace
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Module where is namespace located
        /// </summary>
        public Module Module { get; set; }

        public Namespace()
        {
            Types = new HashSet<Type>();

            Dependency = this;
        }

        public DependencyGraph BuildDependencyGraph()
        {
            if (_graphCache != null)
                return _graphCache;

            var g = new DependencyGraph();

            foreach (var type in Types)
            {
                g.AddVertex(new DependencyVertex(type));
            }

            foreach (var type in Types)
            {
                var types = type.GetUses();

                foreach (var dependType in types)
                {
                    if (dependType != type && dependType.Namespace == type.Namespace)
                        g.AddEdge(new DependencyEdge(new DependencyVertex(type), new DependencyVertex(dependType)));
                }
            }

            _graphCache = g;
            return g;
        }

        public override string ToString()
        {
            return Name;
        }

        public IDependency Dependency { get; set; }

        public string GetInfo()
        {
            var builder = new StringBuilder();
            builder.AppendLine("Namespace Summary");
            builder.Append(Environment.NewLine);
            builder.AppendLine(String.Format("Name: {0}", Name));
            builder.AppendLine(String.Format("Methods: {0}", Types.Sum(type => type.Methods.Count)));
            builder.AppendLine(String.Format("Fields: {0}", Types.Sum(type => type.Fields.Count)));
            builder.AppendLine(String.Format("Types: {0}", Types.Count));
            // more to come
            
            return builder.ToString();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuickGraph;

namespace ICSharpCode.CodeQualityAnalysis
{
    public class Module : IDependency
    {
        /// <summary>
        /// Namespaces within module
        /// </summary>
        public ISet<Namespace> Namespaces { get; set; }

        /// <summary>
        /// Name of module
        /// </summary>
        public string Name { get; set; }

        public Module()
        {
            Namespaces = new HashSet<Namespace>();
        }

        public BidirectionalGraph<object, IEdge<object>> BuildDependencyGraph()
        {
            var g = new BidirectionalGraph<object, IEdge<object>>();

            foreach (var ns in Namespaces)
            {
                g.AddVertex(ns.Name);
            }

            foreach (var ns in Namespaces)
            {
                foreach (var type in ns.Types)
                {
                    var types = type.GetUses();

                    foreach (var dependType in types)
                    {
                        if (dependType != type && dependType.Namespace.Module == type.Namespace.Module)
                            g.AddEdge(new Edge<object>(type.Namespace.Name, dependType.Namespace.Name));
                    }
                }
            }

            return g;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}

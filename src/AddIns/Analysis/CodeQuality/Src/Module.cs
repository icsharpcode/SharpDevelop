using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.CodeQualityAnalysis.Controls;
using QuickGraph;

namespace ICSharpCode.CodeQualityAnalysis
{
    public class Module : IDependency, INode
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

            Dependency = this;
        }

        public DependencyGraph BuildDependencyGraph()
        {
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

            return g;
        }

        public override string ToString()
        {
            return Name;
        }

        public IDependency Dependency { get; set; }

        public string GetInfo()
        {
            return this.ToString();
        }
    }
}
